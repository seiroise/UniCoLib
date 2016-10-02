using UnityEngine;
using System.Collections;

namespace Seiro.Scripts.Geometric {

	/// <summary>
	/// 二次元線分
	/// </summary>
	public class LineSegment {
		public Vector2 p1;
		public Vector2 p2;

		#region Constructors

		public LineSegment(Vector2 p1, Vector2 p2) {
			this.p1 = p1;
			this.p2 = p2;
		}

		public LineSegment(float x1, float y1, float x2, float y2)
				: this(new Vector2(x1, y1), new Vector2(x2, y2)) { }

		#endregion

		#region Function

		public override string ToString() {
			return "(" + p1.x + "," + p1.y + ") - (" + p2.x + "," + p2.y + ")";
		}

		/// <summary>
		/// 二次元直線に変換
		/// </summary>
		public Line ToLine() {
			return Line.FromPoints(p1, p2);
		}

		/// <summary>
		/// 二次元直線との交差判定
		/// </summary>
		public bool Intersects(Line l) {
			float t1 = l.a * p1.x + l.b * p1.y + l.c;   //端点1の座標を直線の式に代入
			float t2 = l.a * p2.x + l.b * p2.y + l.c;   //端点2の座標を直線の式に代入

			return t1 * t2 <= 0;    //判定
		}

		/// <summary>
		/// 二次元線分との交差判定(包含判定は未実装)
		/// </summary>
		public bool Intersects(LineSegment s) {
			return HostSides(s) && s.HostSides(this);
		}

		/// <summary>
		/// sが自線分の「両側」にあるかどうかを調べる
		/// </summary>
		public bool HostSides(LineSegment s) {
			float ccw1 = GeomUtil.CCW(p1, s.p1, p2);
			float ccw2 = GeomUtil.CCW(p1, s.p2, p2);

			if(ccw1 == 0f && ccw2 == 0f) {
				//sと自線分が一直線上にある場合
				//sのずれか1つの端点が自線分を内分していれば,sは自線分と共有部分を持ちtrueを返す
				return Internal(s.p1) || Internal(s.p2);
			} else {
				//それ以外
				//CCW値の富豪が異なる場合にtrueを返す
				return ccw1 * ccw2 < 0f;
			}
		}

		/// <summary>
		/// 点pが自線分を内分しているかどうかを調べる
		/// </summary>
		public bool Internal(Vector2 p) {
			//pから端点に向かうベクトルの内積が0以下てあれば内分とみなす
			return GeomUtil.Dot(p1 - p, p2 - p) <= 0;
		}

		/// <summary>
		/// 二次元直線との交点を求める
		/// </summary>
		public bool GetIntersectionPoint(Line l, ref Vector2 p) {
			if(!Intersects(l)) {
				return false;   //交差しない場合はfalseを返す
			}
			return l.GetIntersectionPoint(ToLine(), ref p);
		}

		/// <summary>
		/// 二次元線分との交点を求める
		/// </summary>
		public bool GetIntersectionPoint(LineSegment s, ref Vector2 p) {
			if(!Intersects(s)) {
				return false;   //交差しない場合はfalseを返す
			}
			return s.ToLine().GetIntersectionPoint(ToLine(), ref p);
		}

		#endregion
	}
}