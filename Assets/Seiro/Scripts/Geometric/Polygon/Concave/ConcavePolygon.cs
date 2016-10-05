using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Seiro.Scripts.Graphics;

namespace Seiro.Scripts.Geometric.Polygon.Concave {

	/// <summary>
	/// 凹多角形
	/// </summary>
	public class ConcavePolygon {

		private List<PolygonVertex> vertices;   //頂点群
		private int mostFarIndex;               //最も遠い点の番号
		private float mostFarCross;             //最も遠い点の外積

		#region Constructor

		public ConcavePolygon(List<Vector2> points) {
			Initialize(points);
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

			//最も遠い座標のインデックスを求める
			int index = 0;
			float maxDistance = 0f;
			float distance = 0f;
			for(int i = 0; i < size; ++i) {
				distance = points[i].magnitude;
				if(distance > maxDistance) {
					maxDistance = distance;
					index = i;
				}
			}
			mostFarIndex = index;

			//最も遠い座標の外積を求める
			Vector2 p0 = points[(index + (size - 1)) % size];
			Vector2 p1 = points[index];
			Vector2 p2 = points[(index + 1) % size];
			mostFarCross = GeomUtil.CCW(p0, p1, p2);

			//頂点リストの作成
			vertices = new List<PolygonVertex>();
			for(int i = 0; i < size; ++i) {
				p0 = points[(i + (size - 1)) % size];
				p1 = points[i];
				p2 = points[(i + 1) % size];

				float cross = GeomUtil.CCW(p0, p1, p2);
				float angle = GeomUtil.TwoVectorAngle(p1, p0, p2);
				if(cross * mostFarCross < 0f) {
					angle = 360f - angle;
				}
				vertices.Add(new PolygonVertex(p1, angle, i));
			}
		}

		/// <summary>
		/// 簡易メッシュ変換
		/// </summary>
		public EasyMesh ToEasyMesh(Color color) {
			int size = vertices.Count;
			Vector3[] verts = new Vector3[size];
			Color[] colors = new Color[size];
			int[] indices = new int[(size - 2) * 3];

			//一時データと準備
			List<PolygonVertex> temp = new List<PolygonVertex>();
			for(int i = 0; i < size; ++i) {
				temp.Add(new PolygonVertex(vertices[i]));
				verts[i] = vertices[i].point;
				colors[i] = color;
			}

			//頂点処理ループ
			int index = mostFarIndex;
			int indicesCount = 0;
			Vector2 p0, p1, p2;
			int i0, i1, i2;
			int processNum = 0;
			int maxProcessNum = temp.Count * 2;

			while(temp.Count > 3 && processNum < maxProcessNum) {
				size = temp.Count;

				i0 = (index + size - 1) % size;
				p0 = temp[i0].point;
				i1 = index;
				p1 = temp[i1].point;
				i2 = (index + 1) % size;
				p2 = temp[i2].point;

				//外積の向きを調べる
				float cross = GeomUtil.CCW(p0, p1, p2);
				if(cross * mostFarCross >= 0f) {
					//三角形の中に他の頂点が混じってないか確認
					bool contains = false;
					for(int i = 2; i < size - 1; ++i) {
						int j = (index + i) % size;

						if(GeomUtil.TriangleInPoint(p0, p1, p2, temp[j].point)) {
							contains = true;
							break;
						}
					}
					//インデックスの追加
					if(!contains) {
						indices[indicesCount + 0] = temp[i0].index;
						indices[indicesCount + 1] = temp[i1].index;
						indices[indicesCount + 2] = temp[i2].index;
						indicesCount += 3;
						temp.RemoveAt(i1);
						size--;
					}
				}

				index = (index + 1) % size;
				++processNum;
			}

			if(processNum >= maxProcessNum) {
				throw new ArgumentException("Not Create Mesh");
			}

			//最後の3点を加える
			indices[indicesCount + 0] = temp[0].index;
			indices[indicesCount + 1] = temp[1].index;
			indices[indicesCount + 2] = temp[2].index;

			return new EasyMesh(verts, colors, indices);
		}

		/// <summary>
		/// ポリゴン用頂点リストの取得
		/// </summary>
		public List<PolygonVertex> GetPolygonVertices() {
			return new List<PolygonVertex>(vertices);
		}

		#endregion

		#region Coroutine

		/// <summary>
		/// 簡易メッシュ変換コルーチン
		/// </summary>
		public IEnumerator CoToEasyMesh(Color color, Action<EasyMesh> updateCallback, Action<EasyMesh> endCallback = null, float wait = 0.1f) {
			int size = vertices.Count;
			Vector3[] verts = new Vector3[size];
			Color[] colors = new Color[size];
			int[] indices = new int[(size - 2) * 3];

			//一時データと準備
			List<PolygonVertex> temp = new List<PolygonVertex>();
			for(int i = 0; i < size; ++i) {
				temp.Add(new PolygonVertex(vertices[i]));
				verts[i] = vertices[i].point;
				colors[i] = color;
			}

			//頂点処理ループ
			int index = mostFarIndex;
			int indicesCount = 0;
			Vector2 p0, p1, p2;
			int i0, i1, i2;
			int processNum = 0;
			int maxProcessNum = temp.Count * 2;

			while(temp.Count > 3 && processNum < maxProcessNum) {
				size = temp.Count;

				i0 = (index + size - 1) % size;
				p0 = temp[i0].point;
				i1 = index;
				p1 = temp[i1].point;
				i2 = (index + 1) % size;
				p2 = temp[i2].point;

				//外積の向きを調べる
				float cross = GeomUtil.CCW(p0, p1, p2);
				if(cross * mostFarCross >= 0f) {
					//三角形の中に他の頂点が混じってないか確認
					bool contains = false;
					for(int i = 2; i < size - 1; ++i) {
						int j = (index + i) % size;

						if(GeomUtil.TriangleInPoint(p0, p1, p2, temp[j].point)) {
							contains = true;
							break;
						}
					}
					//インデックスの追加
					if(!contains) {
						indices[indicesCount + 0] = temp[i0].index;
						indices[indicesCount + 1] = temp[i1].index;
						indices[indicesCount + 2] = temp[i2].index;
						indicesCount += 3;
						temp.RemoveAt(i1);
						size--;

						yield return new WaitForSeconds(wait);
						//コールバック
						if(updateCallback != null) updateCallback(new EasyMesh(verts, colors, indices));
					}
				}

				index = (index + 1) % size;
				++processNum;
			}

			if(processNum >= maxProcessNum) {
				throw new ArgumentException("Not create mesh");
			}

			//最後の3点を加える
			indices[indicesCount + 0] = temp[0].index;
			indices[indicesCount + 1] = temp[1].index;
			indices[indicesCount + 2] = temp[2].index;

			yield return new WaitForSeconds(wait);
			//コールバック
			if(endCallback != null) endCallback(new EasyMesh(verts, colors, indices));
		}

		#endregion
	}
}