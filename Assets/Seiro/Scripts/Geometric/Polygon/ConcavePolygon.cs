using UnityEngine;
using System;
using System.Collections.Generic;

namespace Seiro.Scripts.Geometric.Polygon {

	/// <summary>
	/// 凹多角形
	/// </summary>
	public class ConcavePolygon {

		#region Inner Class

		/// <summary>
		/// 多角形の頂点
		/// </summary>
		public class PolygonVertex {
			public Vector2 point;	//座標
			public float angle;		//角度
			public bool enabled;	//有効

			#region Constructor

			public PolygonVertex(Vector2 point, float angle) {
				this.point = point;
				this.angle = angle;
				this.enabled = true;
			}

			#endregion

		}

		#endregion

		private List<PolygonVertex> vertices;	//頂点群
		private float mostFarIndex;				//最も遠い点の番号
		private float mostFarCross;				//最も遠い点の外積

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
				if(distance < maxDistance) {
					maxDistance = distance;
					index = i;
				}
			}
			mostFarIndex = index;

			//最も遠い座標の外積を求める
			Vector2 p1 = points[(index + (size - 1)) % size];
			Vector2 p2 = points[index];
			Vector2 p3 = points[(index + 1) % size];
			mostFarCross = GeomUtil.CCW(p1, p2, p3);

			//頂点リストの作成
			vertices = new List<PolygonVertex>();
			for(int i = 0; i < size; ++i) {
				p1 = points[(i + (size - 1)) % size];
				p2 = points[i];
				p3 = points[(i + 1) % size];

				float cross = GeomUtil.CCW(p1, p2, p3);
				float angle = GeomUtil.TwoVectorAngle(p2, p1, p3);
				if(cross * mostFarCross < 0f) {
					angle = 360f - angle;
				}
				vertices.Add(new PolygonVertex(p2, angle));
			}
		}

		#endregion
	}
}