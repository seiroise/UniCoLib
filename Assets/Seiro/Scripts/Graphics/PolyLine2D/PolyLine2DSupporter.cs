using UnityEngine;
using System;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.PolyLine2D.Snap;

namespace Seiro.Scripts.Graphics.PolyLine2D {

	/// <summary>
	/// 二次元ポリラインの補助クラス
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class PolyLine2DSupporter : PolyLine2DEditorComponent {

		/// <summary>
		/// 優先度付きスナップ
		/// </summary>
		public class PrioritySnap {
			public int priority;
			public BaseSnap snap;

			public PrioritySnap(int priority, BaseSnap snap) {
				this.priority = priority;
				this.snap = snap;
			}
		}

		[Header("Parameter")]
		public Color color = Color.white;
		public bool drawDefaultGrid = true;		//有効時に標準グリッドを描画
		public float snapForce = 0.5f;			//スナップ有効範囲

		private MeshFilter mf;
		private List<PrioritySnap> snaps;
		private Dictionary<PrioritySnap, Action> callbackDic;
		private PrioritySnap prevSnap;

		#region UnityEvent

		private void Awake() {
			mf = GetComponent<MeshFilter>();
			snaps = new List<PrioritySnap>();
			callbackDic = new Dictionary<PrioritySnap, Action>();
			prevSnap = null;
		}

		#endregion

		#region VirtualFunction

		public override void Enable() {
			base.Enable();

			//標準グリッドの描画
			if(drawDefaultGrid) {
				AddDefaultSnap();
				Draw();
			}
		}

		public override void Disable() {
			//スナップの消去
			Clear();
			Draw();

			base.Disable();
		}

		#endregion

		#region Function

		/// <summary>
		/// 補助線への座標のスナップ
		/// </summary>
		public bool Snap(Vector2 point, out Vector2 result) {
			PrioritySnap snap = Snap(snaps, point, out result);
			prevSnap = snap;

			if(snap != null) {
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// 座標のスナップ。スナップした要素を返す
		/// </summary>
		private PrioritySnap Snap(List<PrioritySnap> snaps, Vector2 point, out Vector2 result) {
			result = Vector2.zero;

			if(snaps.Count == 0) {
				return null;
			}
			float minDistance = float.MaxValue;
			Vector2 sample;
			PrioritySnap snap = null;
			bool snaped = false;

			for(int i = 0; i < snaps.Count; ++i) {
				PrioritySnap e = snaps[i];
				if(e.snap.Snap(point, out sample)) {
					if(snap == null) {
						result = sample;
						snap = e;
						minDistance = (sample - point).magnitude;
						snaped = true;
					} else if(e.priority > snap.priority) {
						result = sample;
						snap = e;
						minDistance = (sample - point).magnitude;
						snaped = true;
					} else if(snap.priority == e.priority) {
						//距離を測る
						float distance = (sample - point).magnitude;
						if(minDistance > distance) {
							result = sample;
							snap = e;
							minDistance = distance;
							snaped = true;
						}
					}
				}
			}
			return snap;
		}

		/// <summary>
		/// スナップの追加
		/// </summary>
		public void AddSnap(int priority, BaseSnap snap, Action callback = null) {
			PrioritySnap pSnap = new PrioritySnap(priority, snap);
			snaps.Add(pSnap);
			if(callback != null) {
				callbackDic.Add(pSnap, callback);
			}
		}

		/// <summary>
		/// デフォルトスナップの追加
		/// </summary>
		public void AddDefaultSnap() {
			snaps.Add(new PrioritySnap(0, new GridSnap(0.25f, 4, snapForce)));
		}

		/// <summary>
		/// 描画
		/// </summary>
		public void Draw() {
			//要素数確認
			int count = snaps.Count;
			if(count <= 0) return;

			//簡易メッシュ群の作成
			EasyMesh[] eMeshes = new EasyMesh[count];
			for(int i = 0; i < snaps.Count; ++i) {
				eMeshes[i] = snaps[i].snap.GetEasyMesh(color);
			}

			//描画
			mf.mesh = EasyMesh.ToMesh(eMeshes);
		}

		/// <summary>
		/// 補助線の消去
		/// </summary>
		public void Clear() {
			snaps.Clear();
			callbackDic.Clear();

			mf.mesh = null;
		}

		/// <summary>
		/// コールバック呼び出し
		/// </summary>
		public void SnapCallback() {
			if(prevSnap != null) {
				if(callbackDic.ContainsKey(prevSnap)) {
					callbackDic[prevSnap]();
				}
			}
		}

		#endregion

	}
}