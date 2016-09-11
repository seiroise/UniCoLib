using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Seiro.Scripts.Graphics {
	/// <summary>
	/// 軽量な線の描画
	/// </summary>
	public class ProceduralMeshLine : MonoBehaviour {

		/// <summary>
		/// 線
		/// </summary>
		[System.Serializable]
		public class Line {
			public List<Vector4> verts; //頂点の集合 x, y, zとwには時間が入る
			public Transform target;
			public float time;
			public bool active;

			#region Constructor
			public Line() {
				verts = new List<Vector4>();
				target = null;
				time = 1f;
				active = true;
			}
			public Line(Transform target, float time) : this() {
				this.target = target;
				this.time = time;
			}
			#endregion
			#region Function
			/// <summary>
			/// 更新
			/// </summary>
			public void Update() {
				//新しい頂点の追加
				if(target) {
					AddVertex(target.position);
				}
				//時間の更新
				UpdateTime();
				//active確認
				if(!target && verts.Count <= 0) active = false;
			}
			/// <summary>
			/// 時間の更新
			/// </summary>
			private void UpdateTime() {
				if(target) {
					for(int i = 0; i < verts.Count;) {
						Vector4 v = verts[i];
						v.w += Time.deltaTime;
						if(v.w > time) {
							verts.RemoveAt(i);
						} else {
							verts[i] = v;
							++i;
						}
					}
				} else {
					for(int i = 0; i < verts.Count;) {
						Vector4 v = verts[i];
						v.w = Mathf.Lerp(v.w, time + 0.01f, 1f * Time.deltaTime);
						if(v.w > time) {
							verts.RemoveAt(i);
						} else {
							verts[i] = v;
							++i;
						}
					}
				}
			}
			/// <summary>
			/// 頂点の追加
			/// </summary>
			public void AddVertex(Vector3 position) {
				Vector4 vert = position;
				vert.w = 0f;
				verts.Add(vert);
			}
			#endregion
		}

		public Material mat;
		public Gradient vertexColor;


		[SerializeField]
		private List<Line> lines;
		private Line nowLine;

		#region UnityEvent
		protected void Awake() {
			lines = new List<Line>();
		}
		private void Update() {
			UpdateLine();
		}
		#endregion
		#region Function
		/// <summary>
		/// 線の更新
		/// </summary>
		private void UpdateLine() {
			for(int i = lines.Count - 1; i >= 0; --i) {
				lines[i].Update();
				if(!lines[i].active) lines.RemoveAt(i);
			}
			//描画
			DrawLines(lines);
		}
		/// <summary>
		/// 線群を描画する
		/// </summary>
		private void DrawLines(List<Line> lines) {
			if(lines == null) return;
			if(lines.Count <= 0) return;

			List<Vector4> lineVerts;

			Mesh mesh = new Mesh();

			List<Vector3> vertices = new List<Vector3>();
			List<Color> colors = new List<Color>();
			List<int> indices = new List<int>();

			for(int i = 0; i < lines.Count; ++i) {
				lineVerts = lines[i].verts;
				if(lineVerts.Count > 1) {
					for(int j = 0; j < lineVerts.Count - 1; ++j) {
						//頂点
						vertices.Add(lineVerts[j]);
						//頂点カラー
						colors.Add(vertexColor.Evaluate(lineVerts[j].w / lines[i].time));
						//トライアングル
						indices.Add(vertices.Count - 1);
						indices.Add(vertices.Count);
					}
					//頂点
					vertices.Add(lineVerts[lineVerts.Count - 1]);
					//頂点カラー
					colors.Add(vertexColor.Evaluate(1f));
				}
			}
			mesh.SetVertices(vertices);
			mesh.SetColors(colors);
			mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
			UnityEngine.Graphics.DrawMesh(mesh, Matrix4x4.identity, mat, 0);
		}
		/// <summary>
		/// 軌跡描画用の線の取得
		/// </summary>
		public Line GetLine(Transform target) {
			if(lines == null) lines = new List<Line>();
			Line line = new Line(target, 0.2f);
			lines.Add(line);
			return line;
		}
		/// <summary>
		/// 取得済みの線の解放
		/// </summary>
		public void ReleaseLine(Line line) {
			lines.Remove(line);
		}
		#endregion
	}
} //namespace end