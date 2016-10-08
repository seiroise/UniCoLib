using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Utility;

namespace Seiro.Scripts.Graphics.PolyLine2D {

	/// <summary>
	/// 二次元ポリラインの描画
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class PolyLine2DRenderer : PolyLine2DEditorComponent {

		[Header("LineParameter")]
		public float width = 0.1f;
		public Color color = Color.white;

		[Header("Effect")]
		public ParticleSystem effectParticle;
		public float density = 1f;
		public float speed = 0.5f;

		private PolyLine2D mainLine;    //ポリライン
		private PolyLine2D subLine;		//サブライン
		private MeshFilter mf;
		private bool draw;				//描画フラグ

		#region UnityEvent

		private void Update() {
			Draw();
		}

		#endregion

		#region VirtualFunction

		public override void Initialize(PolyLine2DEditor editor) {
			base.Initialize(editor);
			mainLine = new PolyLine2D();
			subLine = new PolyLine2D();
			mf = GetComponent<MeshFilter>();
		}

		#endregion

		#region PrivateFunction

		/// <summary>
		/// 描画
		/// </summary>
		private void Draw() {
			if(!draw) return;
			EasyMesh[] eMeshes = new EasyMesh[2];
			eMeshes[0] = mainLine.MakeLine(width, color);
			eMeshes[1] = subLine.MakeLine(width, color * 0.5f);
			mf.mesh = EasyMesh.ToMesh(eMeshes);
			draw = false;
		}

		/// <summary>
		/// エフェクトの生成
		/// </summary>
		private void EmitEffect(ParticleSystem particle, Vector2 p1, Vector2 p2, float density) {
			if(!particle) return;
			//方向と距離の取得
			Vector2 dir = p2 - p1;
			float dis = dir.magnitude;
			dir.Normalize();

			//エフェクトの生成
			float tempDis = 0f;
			do {
				particle.Emit(
					p1 + dir * tempDis,                             //座標
					FuncBox.RandomVector2() * speed,                //速度
					UnityEngine.Random.Range(width * 0.8f, width),  //大きさ
					UnityEngine.Random.Range(0.2f, 1f),             //時間
					color                                           //色
				);
				tempDis += density;
			} while(tempDis < dis);

			particle.Emit(
				p1 + dir * dis,                                 //座標
				FuncBox.RandomVector2() * speed,                //速度
				UnityEngine.Random.Range(width * 0.8f, width),  //大きさ
				UnityEngine.Random.Range(0.2f, 1f),             //時間
				color                                           //色
			);

		}

		#endregion

		#region MainLine

		/// <summary>
		/// 頂点の設定
		/// </summary>
		public void SetVertices(List<Vector2> vertices) {
			if(vertices == null) return;
			for(int i = 0; i < vertices.Count; ++i) {
				Add(vertices[i]);
			}
			draw = true;
		}

		/// <summary>
		/// 頂点の取得
		/// </summary>
		public List<Vector2> GetVertices() {
			return mainLine.GetVertices();
		}

		/// <summary>
		/// 全ての頂点の消去
		/// </summary>
		public void Clear() {
			int count = mainLine.GetVertexCount();
			for(int i = 0; i < count; ++i) {
				RemoveLast();
			}
			subLine.Clear();
			mf.mesh = null;
		}

		/// <summary>
		/// 頂点の追加
		/// </summary>
		public void Add(Vector2 point) {
			mainLine.Add(point);
			//エフェクトの生成
			int count = mainLine.GetVertexCount();
			if(count > 1) {
				EmitEffect(effectParticle, mainLine.GetVertex(count - 2), point, density);
			}
			draw = true;
		}

		/// <summary>
		/// 頂点の挿入
		/// </summary>
		public void Insert(int index, Vector2 point) {
			mainLine.Insert(index, point);
			draw = true;
		}

		/// <summary>
		/// 頂点の消去
		/// </summary>
		public void Remove(int index) {
			mainLine.Remove(index);
			draw = true;
		}

		/// <summary>
		/// 最後の頂点の消去
		/// </summary>
		public void RemoveLast() {
			//エフェクトの生成
			int count = mainLine.GetVertexCount();
			if(count > 1) {
				EmitEffect(effectParticle, mainLine.GetVertex(count - 2), mainLine.GetVertex(count - 1), density);
			}
			mainLine.Remove(count - 1);
			draw = true;
		}

		/// <summary>
		/// 頂点の変更
		/// </summary>
		public void Change(int index, Vector2 point) {
			mainLine.Change(index, point);
			draw = true;
		}

		/// <summary>
		/// 頂点数
		/// </summary>
		public int GetVertexCount() {
			return mainLine.GetVertexCount();
		}

		/// <summary>
		/// 頂点の取得
		/// </summary>
		public Vector2 GetVertex(int index) {
			return mainLine.GetVertex(index);
		}

		#endregion

		#region SubLine

		/// <summary>
		/// サブの頂点設定
		/// </summary>
		public void SetSubVertices(params Vector2[] vertices) {
			subLine.Clear();
			if(vertices != null) {
				for(int i = 0; i < vertices.Length; ++i) {
					subLine.Add(vertices[i]);
				}
			}
			draw = true;
		}

		/// <summary>
		/// サブの全ての頂点を消去
		/// </summary>
		public void ClearSubLine() {
			subLine.Clear();
			draw = true;
		}

		#endregion
	}
}