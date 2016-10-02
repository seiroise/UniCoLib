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
		public static float Cross(Vector2 v1, Vector2 v2) {
			return v1.x * v2.y - v2.x * v1.y;
		}

		/// <summary>
		/// 内積
		/// </summary>
		public static float Dot(Vector2 v1, Vector2 v2) {
			return v1.x * v2.x + v1.y * v2.y;
		}

		/// <summary>
		/// p1 -> p2 -> p3の時計/反時計周り判定
		/// </summary>
		public static float CCW(Vector2 p1, Vector2 p2, Vector2 p3) {
			return Cross(p2 - p1, p3 - p2);
		}

		/// <summary>
		/// 2点の角度を求める(deg)
		/// </summary>
		public static float TwoPointAngle(Vector2 p1, Vector2 p2) {
			float dx = p2.x - p1.x;
			float dy = p2.y - p1.y;
			float deg = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
			return deg < 0f ? deg + 360f : deg;
		}

		/// <summary>
		/// 二つのベクトルがなす角を求める(deg)
		/// </summary>
		public static float TwoVectorAngle(Vector2 from, Vector2 to1, Vector2 to2) {
			Vector2 v0 = to1 - from, v1 = to2 - from;
			return Mathf.Acos(Dot(v0, v1) / (v0.magnitude * v1.magnitude)) * Mathf.Rad2Deg;
		}

		/// <summary>
		/// radian角を二次元ベクトルに変換
		/// </summary>
		public static Vector2 RadToVector2(float rad) {
			return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
		}

		/// <summary>
		/// degree角を二次元ベクトルに変換
		/// </summary>
		public static Vector2 DegToVector2(float deg) {
			return RadToVector2(deg * Mathf.Deg2Rad);
		}

		/// <summary>
		/// 入力平面ベクトルを指定角度だけ回転させる
		/// </summary>
		public static Vector2 RotateVector2(Vector2 input, float deg) {
			Vector2 output = Vector2.zero;
			float rad = deg * Mathf.Deg2Rad;
			output.x = input.x * Mathf.Cos(rad) - input.y * Mathf.Sin(rad);
			output.y = input.x * Mathf.Sin(rad) + input.y * Mathf.Cos(rad);
			return output;
		}

		/// <summary>
		/// 三角形と点の包含判定
		/// </summary>
		public static bool TriangleInPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 p) {
			//やり方に関してはここを参考
			//http://www.sousakuba.com/Programming/gs_hittest_point_triangle.html
			float aCross = Cross(a - c, p - a);
			float bCross = Cross(b - a, p - b);
			float cCross = Cross(c - b, p - c);

			if((aCross >= 0f && bCross >= 0f && cCross >= 0f) || (aCross < 0f && bCross < 0f && cCross < 0f)) {
				return true;
			} else {
				return false;
			}
		}
	}
}