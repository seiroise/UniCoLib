using UnityEngine;
using System;

namespace Seiro.Scripts.Geometric {
	/// <summary>
	/// Geom util.
	/// </summary>
	public class GeomUtil {

		/// <summary>
		/// 外積
		/// </summary>
		public static float Cross (Vector2 v1, Vector2 v2) {
			return v1.x * v2.y - v2.x * v1.y;
		}

		/// <summary>
		/// 内積
		/// </summary>
		public static float Dot (Vector2 v1, Vector2 v2) {
			return v1.x * v2.x + v1.y * v2.y;
		}

		/// <summary>
		/// p1 -> p2 -> p3の時計/反時計周り判定
		/// </summary>
		public static float CCW (Vector2 p1, Vector2 p2, Vector2 p3) {
			return Cross (p2 - p1, p3 - p2);
		}

		/// <summary>
		/// 二つのベクトルがなす角を求める
		/// </summary>
		public static float DegAngle (Vector2 from, Vector2 to1, Vector2 to2) {
			Vector2 v0 = to1 - from, v1 = to2 - from;
			return Mathf.Acos(Dot(v0, v1) / (v0.magnitude * v1.magnitude)) * Mathf.Rad2Deg;
		}
	}
}