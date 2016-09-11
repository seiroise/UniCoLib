using UnityEngine;
using System.Collections;

namespace Seiro.Scripts.Geometric {

	/// <summary>
	/// 直線
	/// </summary>
	public class Line {

		public float a;
		public float b;
		public float c;

		#region Constructors

		public Line(float a, float b, float c) {
			this.a = a;
			this.b = b;
			this.c = c;
		}

		#endregion

		#region Function

		public override string ToString() {
			return "a = " + a + ", b = " + b + ", c = " + c;
		}

		/// <summary>
		/// 直線との交点を求める
		/// </summary>
		public bool GetIntersectionPoint(Line l, ref Vector2 p) {
			float d = a * l.b - l.a * b;
			if(d == 0.0) {
				return false;   //直線が並行の場合はfalseを返す
			}
			float x = (b * l.c - l.b * c) / d;
			float y = (l.a * c - a * l.c) / d;
			p = new Vector2(x, y);
			return true;
		}

		/// <summary>
		/// y成分を求める
		/// </summary>
		public float GetY(float x) {
			return ((-a * x) - c) / b;
		}

		#endregion

		#region Static Function

		/// <summary>
		/// 2点を通る直線を求める
		/// </summary>
		public static Line FromPoints(float x1, float y1, float x2, float y2) {
			float dx = x2 - x1;
			float dy = y2 - y1;
			return new Line(dy, -dx, dx * y1 - dy * x1);
		}

		/// <summary>
		/// 2点を通る直線を求める
		/// </summary>
		public static Line FromPoints(Vector2 p1, Vector2 p2) {
			return FromPoints(p1.x, p1.y, p2.x, p2.y);
		}

		/// <summary>
		/// 2点の垂直二等分線を求める
		/// </summary>
		public static Line PerpendicularBisector(float x1, float y1, float x2, float y2) {
			float cx = (x1 + x2) / 2f;
			float cy = (y1 + y2) / 2f;
			return FromPoints(cx, cy, cx + (y1 - y2), cy + (x2 - x1));
		}

		/// <summary>
		/// 2点の垂直二等分線を求める
		/// </summary>
		public static Line PerpendicularBisector(Vector2 p1, Vector2 p2) {
			return PerpendicularBisector(p1.x, p1.y, p2.x, p2.y);
		}

		/// <summary>
		/// 2点の垂直線を重みから求める
		/// </summary>
		public static Line PerpendicularWaitLine(float x1, float y1, float w1, float x2, float y2, float w2) {
			if(w1 < 0f || w2 < 0f) return null;

			float wait = w1 / (w1 + w2);

			float cx = Mathf.Lerp(x1, x2, wait);
			float cy = Mathf.Lerp(y1, y2, wait);

			return FromPoints(cx, cy, cx + (y1 - y2), cy + (x2 - x1));
		}

		/// <summary>
		/// 2点の垂直線を重みから求める
		/// </summary>
		public static Line PerpendicularWaitLine(Vector2 p1, float w1, Vector2 p2, float w2) {
			return PerpendicularWaitLine(p1.x, p1.y, w1, p2.x, p2.y, w2);
		}

		#endregion
	}
}