using UnityEngine;
using System;
using System.Collections.Generic;

namespace Seiro.Scripts.Graphics.PolyLine2D {

	/// <summary>
	/// 二次元ポリライン
	/// </summary>
	[Serializable]
	public class PolyLine2D {

		#region Inner Class

		/// <summary>
		/// 辺
		/// </summary>
		[Serializable]
		private class Edge {

			public Vector2 a, b;

			public float range;
			public float angle;
			public Vector2 direction;

			public Edge(Vector2 a, Vector2 b) {
				this.a = a;
				this.b = b;
				this.range = Vector2.Distance(a, b);
				this.angle = Vector2.Angle(a, b);
				this.direction = (b - a).normalized;
			}
		}

		#endregion

		#region Parameter

		[SerializeField]
		private List<Vector2> vertices;     //頂点
		[SerializeField]
		private List<Edge> edges;           //辺
		private float totalDistance = 0f;   //線の総延長
		public float TotalDistance { get { return totalDistance; } }

		#endregion

		#region Constructor

		public PolyLine2D() {
			vertices = new List<Vector2>();
			edges = new List<Edge>();
		}

		#endregion

		#region Function

		/// <summary>
		/// 頂点の追加
		/// </summary>
		public void Add(Vector2 point) {
			vertices.Add(point);
			if(vertices.Count >= 2) {
				Edge e = new Edge(vertices[vertices.Count - 2], point);
				totalDistance += e.range;
				edges.Add(e);
			}
		}

		/// <summary>
		/// 頂点の挿入
		/// </summary>
		public void Insert(int index, Vector2 point) {
			//頂点数が0の時には挿入できない
			if(index < 0 || vertices.Count <= index) return;

			//頂点の挿入
			vertices.Insert(index, point);

			//辺の挿入
			if(index == 0) {
				edges.Insert(index, new Edge(point, vertices[index + 1]));
			} else {
				//1辺を削除して2辺を挿入
				totalDistance -= edges[index - 1].range;
				edges.RemoveAt(index - 1);
				Edge e = new Edge(vertices[index - 1], vertices[index]);
				totalDistance += e.range;
				edges.Insert(index - 1, e);
				e = new Edge(vertices[index], vertices[index + 1]);
				totalDistance += e.range;
				edges.Insert(index, e);
			}
		}

		/// <summary>
		/// 頂点の削除
		/// </summary>
		public void Remove(int index) {
			//範囲確認
			if(index < 0 || vertices.Count <= index) return;

			//頂点の削除
			vertices.RemoveAt(index);
			if(vertices.Count < 2) {
				edges.Clear();
				totalDistance = 0f;
				return;
			}

			//辺の削除
			if(index == 0) {
				totalDistance -= edges[index].range;
				edges.RemoveAt(index);
			} else if(index == vertices.Count) {
				totalDistance -= edges[index - 1].range;
				edges.RemoveAt(index - 1);
			} else {
				//二辺を削除して一辺を挿入
				totalDistance -= edges[index].range;
				totalDistance -= edges[index - 1].range;
				edges.RemoveAt(index);
				edges.RemoveAt(index - 1);
				Edge e = new Edge(vertices[index - 1], vertices[index]);
				totalDistance += e.range;
				edges.Insert(index - 1, e);
			}
		}

		/// <summary>
		/// 最後尾の頂点を削除
		/// </summary>
		public void RemoveLast() {
			Remove(vertices.Count - 1);
		}

		/// <summary>
		/// 座標を変更する
		/// </summary>
		public void Change(int index, Vector2 point) {

			//範囲確認
			if(index < 0 || vertices.Count <= index) return;

			//頂点の変更
			vertices[index] = point;

			//辺の変更
			if(index == 0) {
				edges[0] = new Edge(point, vertices[1]);
			} else if(index == vertices.Count - 1) {
				edges[index - 1] = new Edge(vertices[vertices.Count - 2], point);
			} else {
				edges[index - 1] = new Edge(vertices[index - 1], point);
				edges[index] = new Edge(point, vertices[index + 1]);
			}
		}

		/// <summary>
		/// 頂点を設定する
		/// </summary>
		public void SetVertices(List<Vector2> vertices) {
			Clear();
			for(int i = 0; i < vertices.Count; ++i) {
				Add(vertices[i]);
			}
		}

		/// <summary>
		/// 頂点などのデータを初期化する
		/// </summary>
		public void Clear() {
			vertices.Clear();
			edges.Clear();
			totalDistance = 0f;
		}

		/// <summary>
		/// 部分線の簡易メッシュを作成。(範囲指定)
		/// </summary>
		public EasyMesh MakeSubLineRange(float start, float end, float width, Color color) {

			//値確認
			if(start >= end) return null;

			//下準備
			List<Vector2> points = new List<Vector2>();
			float range = end - start;
			float sumRange = 0f;
			int i = 0;

			//開始位置/終了位置を探す
			int startIndex = 0;
			int endIndex = 0;
			for(; i < edges.Count; ++i) {
				if(sumRange <= start && start <= sumRange + edges[i].range) {
					points.Add(edges[i].direction * (start - sumRange) + edges[i].a);
					startIndex = 0;
					break;
				}
				sumRange += edges[i].range;
			}
			for(; i < edges.Count; ++i) {
				if(sumRange <= end && end <= sumRange + edges[i].range) {
					points.Add(edges[i].direction * (end - sumRange) + edges[i].a);
					break;
				}
				points.Add(edges[i].b);
				sumRange += edges[i].range;
			}

			return EasyMesh.MakePolyLine2D(points, width, color);
		}

		/// <summary>
		/// 部分線の簡易メッシュを作成。(割合指定)
		/// </summary>
		public EasyMesh MakeSubLinePer(float startPer, float endPer, float width, Color color) {
			float start = startPer * totalDistance;
			float end = endPer * totalDistance;
			return MakeSubLineRange(start, end, width, color);
		}

		/// <summary>
		/// 簡易メッシュを作成
		/// </summary>
		public EasyMesh MakeLine(float width, Color color) {
			return EasyMesh.MakePolyLine2D(vertices, width, color);
		}

		/// <summary>
		/// 指定した番号の頂点を取得する
		/// </summary>
		public Vector2 GetVertex(int index) {
			return vertices[index];
		}

		/// <summary>
		/// 頂点数を取得
		/// </summary>
		public int GetVertexCount() {
			return vertices.Count;
		}

		/// <summary>
		/// 頂点リストの取得
		/// </summary>
		public List<Vector2> GetVertices() {
			List<Vector2> temp = new List<Vector2>();
			for(int i = 0; i < vertices.Count; ++i) {
				temp.Add(vertices[i]);
			}
			return temp;
		}

		/// <summary>
		/// 視点から指定距離の線上の座標を取得する
		/// </summary>
		public Vector2 OnLinePoint(float distance) {
			float sumDis = 0f;
			for(int i = 0; i < edges.Count; ++i) {
				Edge e = edges[i];
				if(sumDis + e.range > distance) {
					return e.a + e.direction * (distance - sumDis);
				}
				sumDis += e.range;
			}
			return edges[edges.Count - 1].b;
		}

		#endregion
	}
}