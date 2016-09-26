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

		private float width;
		private Color color;

		[SerializeField]
		private List<Vector2> verts;    //頂点
		[SerializeField]
		private List<Edge> edges;       //辺
		private float totalRange = 0f;  //線の総延長

		private EasyMesh cache = null;  //キャッシュ
		public EasyMesh Cache {
			get {
				updatedCache = false;
				return cache;
			}
		}
		private bool updatedCache = false;      //キャッシュの更新
		public bool UpdatedCache { get { return updatedCache; } }
		private bool autoUpdateCache = true;    //キャッシュの自動更新
		public bool AutpUpdateCahce { get { return autoUpdateCache; } set { autoUpdateCache = value; } }

		#endregion

		#region Constructor

		public PolyLine2D(float width, Color color) {
			verts = new List<Vector2>();
			edges = new List<Edge>();

			this.width = width;
			this.color = color;
		}

		#endregion

		#region Function

		/// <summary>
		/// 頂点の追加
		/// </summary>
		public void AddVertex(Vector2 point) {
			verts.Add(point);
			if(verts.Count >= 2) {
				Edge e = new Edge(verts[verts.Count - 2], point);
				totalRange += e.range;
				edges.Add(e);

				//キャッシュの更新
				if(autoUpdateCache) {
					UpdateCache();
				}
			}
		}

		/// <summary>
		/// 頂点の挿入
		/// </summary>
		public void InsertVertex(int index, Vector2 point) {
			//頂点数が0の時には挿入できない
			if(index < 0 || verts.Count <= index) return;

			//頂点の挿入
			verts.Insert(index, point);

			//辺の挿入
			if(index == 0) {
				edges.Insert(index, new Edge(point, verts[index + 1]));
			} else {
				//1辺を削除して2辺を挿入
				totalRange -= edges[index - 1].range;
				edges.RemoveAt(index - 1);
				Edge e = new Edge(verts[index - 1], verts[index]);
				totalRange += e.range;
				edges.Insert(index - 1, e);
				e = new Edge(verts[index], verts[index + 1]);
				totalRange += e.range;
				edges.Insert(index, e);
			}

			//キャッシュの更新
			if(autoUpdateCache) {
				UpdateCache();
			}
		}

		/// <summary>
		/// 頂点の削除
		/// </summary>
		public void RemoveVertex(int index) {
			//範囲確認
			if(index < 0 || verts.Count <= index) return;

			//頂点の削除
			verts.RemoveAt(index);
			//Debug.Log(vertices.Count + " : " + index);
			if(verts.Count < 2) {
				edges.Clear();
				totalRange = 0f;
				cache = null;
				updatedCache = true;
				return;
			}

			//辺の削除
			if(index == 0) {
				totalRange -= edges[index].range;
				edges.RemoveAt(index);
			} else if(index == verts.Count) {
				totalRange -= edges[index - 1].range;
				edges.RemoveAt(index - 1);
			} else {
				//二辺を削除して一辺を挿入
				totalRange -= edges[index].range;
				totalRange -= edges[index - 1].range;
				edges.RemoveAt(index);
				edges.RemoveAt(index - 1);
				Edge e = new Edge(verts[index - 1], verts[index]);
				totalRange += e.range;
				edges.Insert(index - 1, e);
			}

			//キャッシュの更新
			if(autoUpdateCache) {
				UpdateCache();
			}
		}

		/// <summary>
		/// 座標を変更する
		/// </summary>
		public void ChangeVertex(int index, Vector2 point) {

			//範囲確認
			if(index < 0 || verts.Count <= index) return;

			//頂点の変更
			verts[index] = point;

			//辺の変更
			if(index == 0) {
				edges[0] = new Edge(point, verts[1]);
			} else if(index == verts.Count - 1){
				edges[index - 1] = new Edge(verts[verts.Count - 2], point);
			} else {
				edges[index - 1] = new Edge(verts[index - 1], point);
				edges[index] = new Edge(point, verts[index + 1]);
			}

			//キャッシュの更新
			if(autoUpdateCache) {
				UpdateCache();
			}
		}

		/// <summary>
		/// 頂点などのデータを初期化する
		/// </summary>
		public void Clear() {
			verts.Clear();
			edges.Clear();
			totalRange = 0f;
			cache = null;
			updatedCache = true;
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
			float start = startPer * totalRange;
			float end = endPer * totalRange;
			return MakeSubLineRange(start, end, width, color);
		}

		/// <summary>
		/// 指定した番号の頂点を取得する
		/// </summary>
		public bool GetVertex(int index, out Vector2 v) {
			
			//範囲確認
			if(index < 0 || verts.Count <= index) {
				v = Vector2.zero;
				return false;
			} else {
				v = verts[index];
				return true;
			}
		}

		/// <summary>
		/// 頂点数を取得
		/// </summary>
		public int GetVertexCount() {
			return verts.Count;
		}

		/// <summary>
		/// 頂点リストの取得
		/// </summary>
		public List<Vector2> GetVertices() {
			return verts;
		}

		/// <summary>
		/// キャッシュの更新を行う
		/// </summary>
		private void UpdateCache() {
			if(verts.Count < 2 && cache != null) {
				cache = null;
			} else {
				cache = EasyMesh.MakePolyLine2D(verts, width, color);
			}
			updatedCache = true;
		}

		#endregion
	}
}