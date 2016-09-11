using UnityEngine;
using System;

namespace Seiro.Scripts.Geometric.Diagram.Voronoi {

	/// <summary>
	/// 重み付きボロノイ母点
	/// </summary>
	public class WaitedVoronoiSite {

		private Vector2 point;  //座標
		public Vector2 Point {
			get { return point; }
			set { point = value; }
		}

		private float wait;     //重み
		public float Wait {
			get { return wait; }
			set { wait = value; }
		}

		public WaitedVoronoiSite(Vector2 point, float wait) {
			this.point = point;
			this.wait = wait;
		}
	}
}