using UnityEngine;
using System;

namespace Seiro.Scripts.Geometric.Polygon.Concave {

	/// <summary>
	/// 多角形の頂点
	/// </summary>
	public class PolygonVertex {

		public Vector2 point;   //座標
		public float angle;     //角度
		public int index;       //番号
		public bool enabled;    //有効

		#region Constructor

		public PolygonVertex(Vector2 point, float angle, int index) {
			this.point = point;
			this.angle = angle;
			this.index = index;
			this.enabled = true;
		}

		public PolygonVertex(PolygonVertex v) : this(v.point, v.angle, v.index) { }

		#endregion

	}
}