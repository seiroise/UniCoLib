using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using Seiro.Scripts.Graphics;

namespace Seiro.Scripts.Geometric.Polygon.Convex {

	/// <summary>
	/// 凸多角形
	/// </summary>
	public class ConvexPolygon {

		//回転方向
		public enum Rotation {
			CW,     //Clockwise
			CCW     //Counter Clockwise
		}

		private List<LineSegment> edges;    //辺
		private List<Vector2> vertices;     //頂点
		public Rotation rotation { get; set; }

		#region Constructors

		//verticesには凸多角形の頂点座標が順番に格納してあるものとする
		public ConvexPolygon(List<Vector2> points) {
			Initialize(points);
		}

		public ConvexPolygon(List<Vector3> points) {
			List<Vector2> p = new List<Vector2>();
			for(int i = 0; i < points.Count; ++i) p.Add(points[i]);
			Initialize(p);
		}

		#endregion

		#region Function

		/// <summary>
		/// 初期化
		/// </summary>
		private void Initialize(List<Vector2> points) {
			int size = points.Count;

			//角数が3未満の場合はエラー
			if(size < 3) {
				throw new ArgumentException();
			}
			this.vertices = points;

			//凸性判定
			float baseCCW = 0f;
			for(int i = 0; i < size; ++i) {
				Vector2 p0 = points[i];
				Vector2 p1 = points[(i + 1) % size];
				Vector2 p2 = points[(i + 2) % size];

				//CCW値の計算
				float ccw = GeomUtil.CCW(p0, p1, p2);
				if(baseCCW == 0f && ccw != 0f) {
					baseCCW = ccw;
				}
				if(ccw * baseCCW < 0) {
					throw new ArgumentException("Polygon is not convex.");
				}
			}
			if(baseCCW > 0f) {
				rotation = Rotation.CCW;
			} else {
				rotation = Rotation.CW;
			}

			//線分の登録
			edges = new List<LineSegment>();
			for(int i = 0; i < size; ++i) {
				Vector2 p1 = points[i];
				Vector2 p2 = points[(i + 1) % size]; //p1の次の頂点

				//2つ頂点から辺の線分を作成して登録
				edges.Add(new LineSegment(p1, p2));
			}
		}

		/// <summary>
		/// 指定位置の頂点を取得
		/// </summary>
		public Vector2 GetVertex(int index) {
			return vertices[index];
		}

		/// <summary>
		/// 指定位置の辺を取得
		/// </summary>
		public LineSegment GetEdge(int index) {
			return edges[index];
		}

		/// <summary>
		/// 辺の数(=角数)を取得
		/// </summary>
		public int GetEdgeCount() {
			return edges.Count;
		}

		/// <summary>
		/// 点の包含判定
		/// </summary>
		public bool Contains(Vector2 p) {
			//多角形のy座標範囲を求める
			Range yRange = GetYRange();

			//yが最小値-最大値の範囲外の場合はfalseを返す
			if(p.y <= yRange.min || yRange.max <= p.y) {
				return false;
			}

			//与えられた座標を始点とし，右方向に十分長く伸びる擬似的な半直線を作成
			LineSegment halfLine = new LineSegment(p.x, p.y, p.x + 10000000f, p.y);
			int count = 0;
			foreach(var e in edges) {
				//半直線が辺の終点とちょうど重なる場合，次の辺の始点とも交差が検出され，
				//二重にカウントされてしまうため，カウントをスキップする
				if(e.p2.y == p.y) {
					continue;
				}
				if(e.Intersects(halfLine)) {
					++count;
				}
			}

			//交差回数が奇数の場合は点を包含
			return count % 2 == 1;
		}

		/// <summary>
		/// 多角形のX座標範囲を取得
		/// </summary>
		public Range GetXRange() {
			float min = float.MaxValue;
			float max = float.MinValue;

			for(int i = 0; i < vertices.Count; ++i) {
				min = Mathf.Min(min, vertices[i].x);
				max = Mathf.Max(max, vertices[i].x);
			}

			return new Range(min, max);
		}

		/// <summary>
		/// 凸多角形のx座標最小，最大値の番号を取得
		/// </summary>
		public void GetXMinMaxindex(ref int minIndex, ref int maxIndex) {
			float min = float.MaxValue;
			float max = float.MinValue;

			for(int i = 0; i < vertices.Count; ++i) {
				float x = vertices[i].x;
				if(x < min) {
					min = x;
					minIndex = i;
				}
				if(x > max) {
					max = x;
					maxIndex = i;
				}
			}
		}

		/// <summary>
		/// 凸多角形のY座標範囲を取得
		/// </summary>
		public Range GetYRange() {
			float min = float.MaxValue;
			float max = float.MinValue;

			for(int i = 0; i < vertices.Count; ++i) {
				min = Mathf.Min(min, vertices[i].y);
				max = Mathf.Max(max, vertices[i].y);
			}

			return new Range(min, max);
		}

		/// <summary>
		/// 凸多角形のy座標最小，最大値の番号を取得
		/// </summary>
		public void GetYMinMaxindex(ref int minIndex, ref int maxIndex) {
			float min = float.MaxValue;
			float max = float.MinValue;

			for(int i = 0; i < vertices.Count; ++i) {
				float y = vertices[i].y;
				if(y < min) {
					min = y;
					minIndex = i;
				}
				if(y > max) {
					max = y;
					maxIndex = i;
				}
			}
		}

		/// <summary>
		/// 多角形の面積を取得する
		/// </summary>
		public float GetArea() {
			float crossSum = 0f;    //外積の合計
			int size = vertices.Count;

			//頂点を巡回
			for(int i = 0; i < size; ++i) {
				Vector2 v1 = vertices[i];
				Vector2 v2 = vertices[(i + 1) % size];
				//外積を計算
				float cross = GeomUtil.Cross(v1, v2);
				//外積を加算
				crossSum += cross;
			}
			return Mathf.Abs(crossSum / 2f);
		}

		/// <summary>
		/// 頂点リストのコピーを取得
		/// </summary>
		public List<Vector2> GetVerticesCopy() {
			return new List<Vector2>(vertices);
		}

		/// <summary>
		/// 頂点リストのコピーをList<Vector3>で取得
		/// </summary>
		public List<Vector3> GetVertices3Copy() {
			List<Vector3> verts = new List<Vector3>();
			for(int i = 0; i < vertices.Count; ++i) verts.Add(vertices[i]);
			return verts;
		}

		/// <summary>
		/// Unity上で扱えるメッシュに変換
		/// </summary>
		public Mesh ToMesh() {

			Mesh mesh = new Mesh();
			mesh.name = "Convex Polygon";

			List<Vector3> verts = new List<Vector3>();
			//List<Vector2> uvs = new List<Vector2>();
			List<int> indices = new List<int>();

			int size = vertices.Count;

			for(int i = 0; i < size; ++i) {
				verts.Add(vertices[i]);
			}

			for(int i = 1; i < size; i += 1) {
				if(rotation == Rotation.CW) {
					//時計回り
					indices.Add(0);
					indices.Add(i);
					indices.Add((i + 1) % size);
				} else {
					//反時計回り
					indices.Add(0);
					indices.Add((i + 1) % size);
					indices.Add(i);
				}
			}

			mesh.Clear();
			mesh.SetVertices(verts);
			mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0, true);
			mesh.RecalculateNormals();

			return mesh;
		}

		/// <summary>
		/// Unity上で扱えるメッシュに変換する(indicesの追加方法が異なる)
		/// </summary>
		public Mesh ToAltMesh() {
			Mesh mesh = new Mesh();
			mesh.name = "Convex Polygon";

			List<Vector3> verts = new List<Vector3>();
			List<int> indices = new List<int>();

			//頂点リストの生成
			int size = vertices.Count;
			verts = GetVertices3Copy();

			//インデックス関連の準備
			int minYIndex = 0, maxYIndex = 0;
			GetYMinMaxindex(ref minYIndex, ref maxYIndex);
			int addLeft = rotation == Rotation.CCW ? 1 : -1;    //左回りのインデックス加算量
			int[] index = { maxYIndex, RightIncrement(maxYIndex, size) };   //左,右

			int count = size - 2;   //角数

			for(int i = 0; i <= count; ++i) {
				int mod = i % 2;
				int p = index[mod];
				if(mod == 0) {
					//左進める
					index[mod] = LeftIncrement(index[mod], size);
					if(rotation == Rotation.CCW) {
						indices.Add(p);
						indices.Add(index[(i + 1) % 2]);
						indices.Add(index[mod]);
					} else {
						indices.Add(p);
						indices.Add(index[mod]);
						indices.Add(index[(i + 1) % 2]);
					}
				} else {
					//右進める
					index[mod] = RightIncrement(index[mod], size);
					if(rotation == Rotation.CW) {
						indices.Add(p);
						indices.Add(index[(i + 1) % 2]);
						indices.Add(index[mod]);
					} else {
						indices.Add(p);
						indices.Add(index[mod]);
						indices.Add(index[(i + 1) % 2]);
					}
				}
			}

			mesh.SetVertices(verts);
			mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0, true);
			mesh.RecalculateNormals();

			return mesh;
		}

		/// <summary>
		/// 簡易メッシュに変換
		/// </summary>
		public EasyMesh ToEasyMesh(Color color) {
			List<Vector3> verts = new List<Vector3>();
			List<Color> colors = new List<Color>();
			List<int> indices = new List<int>();

			int size = vertices.Count;

			for(int i = 0; i < size; ++i) {
				verts.Add(vertices[i]);
				colors.Add(color);
			}

			for(int i = 1; i < size; i += 1) {
				if(rotation == Rotation.CW) {
					//時計回り
					indices.Add(0);
					indices.Add(i);
					indices.Add((i + 1) % size);
				} else {
					//反時計回り
					indices.Add(0);
					indices.Add((i + 1) % size);
					indices.Add(i);
				}
			}

			EasyMesh eMesh = new EasyMesh();
			eMesh.verts = verts.ToArray();
			eMesh.colors = colors.ToArray();
			eMesh.indices = indices.ToArray();
			return eMesh;
		}

		/// <summary>
		/// 左回りに一つ進む
		/// </summary>
		private int LeftIncrement(int index, int size) {
			if(rotation == Rotation.CCW) {
				return (index + 1) % size;
			} else {
				return (index + (size - 1)) % size;
			}
		}

		/// <summary>
		/// 右回りに一つ進む
		/// </summary>
		private int RightIncrement(int index, int size) {
			if(rotation == Rotation.CCW) {
				return (index + (size - 1)) % size;
			} else {
				return (index + 1) % size;
			}
		}

		/// <summary>
		/// 頂点に関するデバッグテキストの出力
		/// </summary>
		public void LogVertexDebugInfo() {
			StringBuilder sb = new StringBuilder();
			sb.Append("Vertex : ");
			sb.Append("Count = " + vertices.Count);
			for(int i = 0; i < vertices.Count; ++i) {
				sb.Append(" [" + i + "]" + vertices[i].x + "," + vertices[i].y);
			}
			Debug.Log(sb.ToString());
		}

		/// <summary>
		/// デバッグ用の線の描画
		/// </summary>
		public void DrawDebugLine(LineRenderer line) {
			if(line == null) return;
			int size = vertices.Count;
			line.SetVertexCount(size + 1);
			for(int i = 0; i < size; ++i) {
				line.SetPosition(i, vertices[i]);
			}
			line.SetPosition(size, vertices[0]);
		}

		#endregion

		#region TRS Function

		/// <summary>
		/// 指定した移動量の分だけ移動させる
		/// </summary>
		public ConvexPolygon Translate(Vector2 movement) {
			//変換行列でできたらいいね
			for(int i = 0; i < vertices.Count; ++i) {
				vertices[i] += movement;
			}
			return this;
		}

		/// <summary>
		/// 指定した移動量の分だけ移動させ,コピーした凸多角形を取得する
		/// </summary>
		public ConvexPolygon Translated(Vector2 movement) {
			ConvexPolygon copy = new ConvexPolygon(GetVerticesCopy());
			copy.Translate(movement);
			return copy;
		}

		/// <summary>
		/// 指定したアンカーから拡大縮小を行う
		/// </summary>
		public ConvexPolygon Scale(Vector2 anchor, float scaling) {
			//変換行列でできたらいいのになぁ
			for(int i = 0; i < vertices.Count; ++i) {
				vertices[i] = ((vertices[i] - anchor) * scaling) + anchor;
			}
			return this;
		}

		/// <summary>
		/// 指定したアンカーから拡大縮小し,コピーした凸多角形を取得する
		/// </summary>
		public ConvexPolygon Scaled(Vector2 anchor, float scaling) {
			ConvexPolygon copy = new ConvexPolygon(GetVerticesCopy());
			copy.Scale(anchor, scaling);
			return copy;
		}

		#endregion

		#region Static Function

		/// <summary>
		/// 正方形の凸多角形インスタンスを作成する
		/// </summary>
		public static ConvexPolygon SquarePolygon(Vector2 center , float size = 10f) {
			
			List<Vector2> vertices = new List<Vector2>();
			vertices.Add(new Vector2(-size, size) + center);
			vertices.Add(new Vector2(-size, -size) + center);
			vertices.Add(new Vector2(size, -size) + center);
			vertices.Add(new Vector2(size, size) + center);

			return new ConvexPolygon(vertices);
		}

		/// <summary>
		/// 正方形の凸多角形インスタンスを作成する
		/// </summary>
		public static ConvexPolygon SquarePolygon(float size = 10f) {
			return SquarePolygon(Vector2.zero, size);
		}

		/// <summary>
		/// 正方形の凸多角形インスタンスを作成する
		/// </summary>
		public static ConvexPolygon SquarePolygon(Vector2 topLeft, Vector2 bottomRight) {
			List<Vector2> vertices = new List<Vector2>();
			vertices.Add(topLeft);
			vertices.Add(new Vector2(topLeft.x, bottomRight.y));
			vertices.Add(bottomRight);
			vertices.Add(new Vector2(bottomRight.x, topLeft.y));

			return new ConvexPolygon(vertices);
		}

		#endregion
	}
}