using UnityEngine;
using System;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.Circle;
using Seiro.Scripts.Graphics;
using Seiro.Scripts.EventSystems;

namespace Seiro.Scripts.UI.Module {

	/// <summary>
	/// 円状のUI表示
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class UICircle : MonoBehaviour, ICollisionEventHandler {

		#region Inner Class

		/// <summary>
		/// 断片のメタデータ
		/// </summary>
		[Serializable]
		public class FragmentMeta {

			//パラメータ
			public CircleFragment frag; //断片
			public string text;         //テキスト
			public Color color;         //色
			public bool hide;           //次回の更新終了時の動作

			//親子関係パラメータ
			[NonSerialized]
			public List<FragmentMeta> children; //子
			public int depth;                   //深度

			public FragmentMeta() {
				this.frag = null;
				this.text = "";
				this.color = Color.white;
				this.hide = false;
				this.children = new List<FragmentMeta>();
				this.depth = 0;
			}
		}

		#endregion

		[SerializeField]
		private float innerRadius = 1f;

		[SerializeField]
		private float outerRaadius = 3f;

		[SerializeField]
		private float sectorInterval = 1f;  //トラック内のセクタとの間隔(角度指定)

		[SerializeField]
		private float trackInterval = 1f;   //トラック間の間隔

		[SerializeField]
		private CircleFragment.RangeIndicate rangeIndicate;

		[SerializeField]
		private CircleFragment.RadiusIndicate radiusIndicate;

		[SerializeField, Multiline(8)]
		private string textTree;

		//メッシュ関連
		private MeshFilter mf;
		private MeshCollider mc;

		//描画関連
		private List<FragmentMeta> drawFrags;   //描画が必要な断片群
		private EasyMesh[] drawEMeshes;         //描画された簡易メッシュ
		private Stack<FragmentMeta> drawStack;  //表示メタデータスタック

		//更新関連
		private List<FragmentMeta> updateFrags; //更新が必要な断片群

		//メタデータ関連
		private FragmentMeta rootMeta;          //テキストから作成した断片のルート
		private FragmentMeta pointerOverMeta;   //ポインタの重なっているフラグメントのメタデータ

		#region UnityEvent

		private void Awake() {

			mf = GetComponent<MeshFilter>();
			mc = GetComponent<MeshCollider>();

			drawFrags = new List<FragmentMeta>();
			drawStack = new Stack<FragmentMeta>();
			updateFrags = new List<FragmentMeta>();
		}

		private void Start() {
			rootMeta = MakeMetaTree(textTree);
		}

		private void Update() {
			UpdateFragments();
			DrawFragments();

			//確認用
			if(Input.GetKeyDown(KeyCode.V)) {
				Visible(rootMeta);
			}
		}

		private void OnGUI() {
			GUILayout.Label("DrawFrags = " + drawFrags.Count);
			GUILayout.Label("UpdateFrags = " + updateFrags.Count);
			if(pointerOverMeta != null) {
				GUILayout.Label("Over = " + pointerOverMeta.text);
			}
		}

		#endregion

		#region Private Function

		/// <summary>
		/// 断片群の更新
		/// </summary>
		private void UpdateFragments() {
			if(updateFrags.Count <= 0) return;
			FragmentMeta meta;
			CircleFragment frag;
			for(int i = updateFrags.Count; i > 0;) {
				--i;
				meta = updateFrags[i];
				frag = meta.frag;
				if(!frag.Processing) {
					if(meta.hide) {
						drawFrags.Remove(meta);
					}
					updateFrags.Remove(meta);
				} else {
					frag.Update();
				}
			}
		}

		/// <summary>
		/// 断片群の描画
		/// </summary>
		private void DrawFragments() {
			if(drawFrags.Count > 0) {
				CircleFragment temp;
				EasyMesh[] eMeshes = new EasyMesh[drawFrags.Count];
				for(int i = 0; i < drawFrags.Count; ++i) {
					temp = drawFrags[i].frag;
					eMeshes[i] = temp.Cache;
				}
				//簡易メッシュを変換
				Mesh mesh = EasyMesh.ToMesh(eMeshes);
				mf.mesh = mesh;
				drawEMeshes = eMeshes;
				//当たり判定(メッシュのサイズが小さすぎる場合は設定しない)
				if(mc.sharedMesh != null) {
					mc.sharedMesh.Clear();
				}
				mc.sharedMesh = mesh;
			}
		}

		/// <summary>
		/// 断片の追加
		/// </summary>
		private void AddFragment(FragmentMeta meta) {
			//追加
			if(drawFrags.Contains(meta)) {
				drawFrags.Remove(meta);
			}
			drawFrags.Add(meta);

			if(updateFrags.Contains(meta)) {
				updateFrags.Remove(meta);
			}
			updateFrags.Add(meta);
		}

		/// <summary>
		/// メタデータから断片群を作成
		/// </summary>
		private void MakeFragments(List<FragmentMeta> metas, float inner, float outer) {

			//フラグメントの生成
			float deltaAngle = 360f / metas.Count;

			//間隔調整
			float halfInterval = sectorInterval * 0.5f;
			float startOffset = deltaAngle > 0f ? halfInterval : -halfInterval;
			float endOffset = -startOffset;

			//断片群の生成
			for(int i = 0; i < metas.Count; ++i) {
				//範囲の設定
				float start = deltaAngle * i + startOffset;
				float end = start + deltaAngle + endOffset;
				//断片の作成
				CircleFragment frag = new CircleFragment();
				frag.SetRange(start, end);
				frag.SetRadius(inner, outer);
				frag.SetOptions(10f, 1f, metas[i].color);
				//メタデータに設定
				metas[i].frag = frag;
			}
		}

		/// <summary>
		/// テキストからメタデータツリーを作成
		/// </summary>
		private FragmentMeta MakeMetaTree(string text) {
			//分割
			string[] lines = text.Split('\n');
			if(lines.Length <= 0) return null;
			FragmentMeta root = new FragmentMeta();

			//親子関作成
			Stack<FragmentMeta> parentStack = new Stack<FragmentMeta>();
			FragmentMeta prevMeta = null;

			//断片群の生成
			for(int i = 0; i < lines.Length; ++i) {
				//ヘッダーの解析
				string[] splits = lines[i].Split(' ');
				if(splits.Length >= 0) {
					string header = splits[0];
					int count = 0;
					for(int j = 0; j < header.Length; ++j) {
						if(header[j] == '-') {
							count++;
						} else {
							break;
						}
					}
					//親の位置調整量を計算
					int adjust = count - parentStack.Count;
					if(adjust > 0) {
						if(prevMeta != null) {
							parentStack.Push(prevMeta);
						}
					} else if(adjust < 0) {
						for(int j = adjust; j < 0; ++j) {
							parentStack.Pop();
						}
						prevMeta = null;
					}
					//ヘッダー分短くする
					lines[i] = lines[i].Substring(header.Length);
				}

				//メタデータの解析
				FragmentMeta meta = ParseLine(lines[i]);
				//親子関係の処理
				meta.depth = parentStack.Count + 1;
				//追加
				if(parentStack.Count > 0) {
					parentStack.Peek().children.Add(meta);
				} else {
					root.children.Add(meta);
				}
				//前回のメタデータを設定
				prevMeta = meta;
			}

			return root;
		}

		/// <summary>
		/// テキストからメタデータを作成
		/// </summary>
		private FragmentMeta ParseLine(string line) {
			FragmentMeta meta = new FragmentMeta();
			string[] lineSplits = line.Split(' ');
			foreach(var e in lineSplits) {
				string[] s = e.Split(':');
				if(s.Length != 2) continue;
				string tag = s[0];
				string value = s[1];

				//分岐解析
				switch(tag) {
				case "t":   //テキスト
					meta.text = value;
					break;
				case "c":   //色
					Color c;
					if(ColorUtility.TryParseHtmlString(value, out c)) {
						meta.color = c;
					} else {
						meta.color = Color.white;
					}
					break;
				}
			}
			return meta;
		}

		/// <summary>
		/// 表示
		/// </summary>
		private void Visible(FragmentMeta meta) {

			int adjust = meta.depth - drawStack.Count;

			if(adjust >= 0) {
				drawStack.Push(meta);
				VisibleMeta(meta);
			}
		}

		/// <summary>
		/// 非表示
		/// </summary>
		private void Hide(FragmentMeta meta) {

		}

		/// <summary>
		/// メタデータの子断片群の表示
		/// </summary>
		private void VisibleMeta(FragmentMeta meta) {
			List<FragmentMeta> metas = meta.children;
			if(metas.Count <= 0) return;

			//外径/内径の計算
			float inner = innerRadius + (outerRaadius + trackInterval) * meta.depth;
			MakeFragments(metas, inner, inner + outerRaadius);

			//表示方法を決めて描画/更新リストに突っ込む
			for(int i = 0; i < metas.Count; ++i) {
				metas[i].hide = false;
				metas[i].frag.SetIndicate(CircleFragment.Indicate.Visible, rangeIndicate, radiusIndicate);
				AddFragment(metas[i]);
			}
		}

		/// <summary>
		/// メタデータの子断片群の非表示
		/// </summary>
		private void HideMeta(FragmentMeta meta) {
			List<FragmentMeta> metas = meta.children;
			if(metas.Count <= 0) return;

			//外径/内径の計算
			float inner = innerRadius + (outerRaadius + trackInterval) * meta.depth;
			MakeFragments(metas, inner, inner + outerRaadius);

			//表示方法を決めて描画/更新リストに突っ込む
			for(int i = 0; i < metas.Count; ++i) {
				metas[i].hide = true;
				metas[i].frag.SetIndicate(CircleFragment.Indicate.Hide, rangeIndicate, radiusIndicate);
				AddFragment(metas[i]);
			}
		}

		/// <summary>
		/// 三角形インデックスからどの
		/// </summary>
		private FragmentMeta GetMetaFromIndex(int triangleIndex) {
			int sum = 0;
			int i = 0;
			foreach(var e in drawEMeshes) {
				sum += e.indices.Length / 3;
				if(triangleIndex < sum) {
					return drawFrags[i];
				}
				++i;
			}
			return null;
		}

		#endregion

		#region ICollisionEventHandler

		public void OnPointerEnter(RaycastHit hit) {
			Debug.Log("Enter");
			pointerOverMeta = GetMetaFromIndex(hit.triangleIndex);
		}

		public void OnPointerExit(RaycastHit hit) {
			Debug.Log("Exit");
			pointerOverMeta = null;
		}

		public void OnPointerDown(RaycastHit hit) {
			throw new NotImplementedException();
		}

		public void OnPointerUp(RaycastHit hit) {
			throw new NotImplementedException();
		}

		public void OnPointerClick(RaycastHit hit) {
			Debug.Log("Click " + hit.triangleIndex);
			FragmentMeta meta = GetMetaFromIndex(hit.triangleIndex);
			VisibleMeta(meta);
		}

		#endregion
	}
}