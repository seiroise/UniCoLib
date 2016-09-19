using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.ChainLine.StandardUpdater;

namespace Seiro.Scripts.Graphics.ChainLine {

	/// <summary>
	/// 線連結工場
	/// </summary>
	public class ChainLineFactory : MonoBehaviour{

		[SerializeField]
		private Material mat;
		[SerializeField]
		private List<ChainLine> lines;

		#region UnityEvent

		private void Awake() {
			lines = new List<ChainLine>();

		}

		private void Update() {
			DrawLines(lines);
		}

		#endregion

		#region Function

		/// <summary>
		/// 線群を描画する
		/// </summary>
		private void DrawLines(List<ChainLine> lines) {
			if(lines == null) return;
			if(lines.Count <= 0) return;

			List<Vertex> lineVerts;

			Mesh mesh = new Mesh();

			List<Vector3> vertices = new List<Vector3>();
			List<Color> colors = new List<Color>();
			List<int> indices = new List<int>();

			//削除確認
			for(int i = lines.Count - 1; i >= 0; --i) {
				if(lines[i].VertsZeroWithDeath) {
					DeleteLine(lines[i]);
				}
			}

			//更新と描画
			for(int i = 0; i < lines.Count; ++i) {
				//更新
				lineVerts = lines[i].Update();
				if(lineVerts.Count > 1) {
					for(int j = 0; j < lineVerts.Count - 1; ++j) {
						//頂点
						vertices.Add(lineVerts[j].pos);
						//頂点カラー
						colors.Add(lineVerts[j].color);
						//トライアングル
						indices.Add(vertices.Count - 1);
						indices.Add(vertices.Count);
					}
					Vertex v = lineVerts[lineVerts.Count - 1];
					//頂点
					vertices.Add(v.pos);
					//頂点カラー
					colors.Add(v.color);
				}
			}
			mesh.SetVertices(vertices);
			mesh.SetColors(colors);
			mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
			UnityEngine.Graphics.DrawMesh(mesh, Matrix4x4.identity, mat, 0);
		}

		/// <summary>
		/// 線連結を生成する
		/// </summary>
		public ChainLine CreateLine(List<Vector3> vertices) {
			ChainLine line = new ChainLine(vertices);
			lines.Add(line);
			return line;
		}

		/// <summary>
		/// 線連結を生成する
		/// </summary>
		public ChainLine CreateLine(List<Vector3> vertices, Color color) {
			ChainLine line = new ChainLine(vertices, color);
			lines.Add(line);
			return line;
		}

		/// <summary>
		/// 線連結を生成する
		/// </summary>
		public ChainLine CreateLine(List<Vector3> vertices, params IChainLineUpdater[] updaters) {
			ChainLine line = new ChainLine(vertices, Color.white, updaters);
			lines.Add(line);
			return line;
		}

		/// <summary>
		/// 線連結を生成する
		/// </summary>
		public ChainLine CreateLine(List<Vector3> vertices, float liveTime, Gradient gradient) {
			ChainLine line = new ChainLine(vertices, Color.white, new LiveTimeUpdater(liveTime), new LiveTimeGradientUpdater(gradient));
			lines.Add(line);
			return line;
		}

		/// <summary>
		/// 連結線を削除する
		/// </summary>
		public void DeleteLine(ChainLine line) {
			if(lines.Contains(line)) {
				lines.Remove(line);
			}
		}

		#endregion
	}
}