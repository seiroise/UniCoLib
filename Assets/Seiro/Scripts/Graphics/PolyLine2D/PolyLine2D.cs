using UnityEngine;
using System.Collections.Generic;

namespace Seiro.Scripts.Graphics.PolyLine2D {

	/// <summary>
	/// 二次元のポリライン
	/// </summary>
	public class PolyLine2D : MonoBehaviour {

		#region Inner Class

		/// <summary>
		/// 辺
		/// </summary>
		private class Edge {
			public Vector2 a, b;
			public float range;

			public Edge(Vector2 a, Vector2 b) {
				this.a = a;
				this.b = b;
				this.range = Vector2.Distance(a, b);
			}
		}

		#endregion

		private List<Vector2> vertices;	//頂点
		private List<Edge> edges;		//辺
		private float totalRange = 0f;	//線の総延長

		#region UnityEvent

		private void Awake() {
			vertices = new List<Vector2>();
		}

		#endregion

		#region Function

		/// <summary>
		/// 頂点の追加
		/// </summary>
		public void AddVertex(Vector2 point) {
			vertices.Add(point);
			if(vertices.Count >= 2) {
				Edge e = new Edge(vertices[vertices.Count - 1], point);
				totalRange += e.range;
			}
		}

		/// <summary>
		/// 頂点の削除
		/// </summary>
		public void RemoveVertex(int index) {
			//範囲確認
			if(index < 0 || vertices.Count <= index) return;

			//頂点の削除
			vertices.RemoveAt(index);

			//辺の削除
			if(index == 0) {
				totalRange -= edges[0].range;
				edges.RemoveAt(0);
			} else if(index == vertices.Count) {
				
			}	

		}

		#endregion
	}
}