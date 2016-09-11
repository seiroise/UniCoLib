using UnityEngine;
using Seiro.Scripts.Geometric;

/// <summary>
/// 交差
/// </summary>
public class Intersection {
	public LineSegment segment1;
	public LineSegment segment2;

	//コンストラクタ
	public Intersection (LineSegment s1, LineSegment s2) {
		this.segment1 = s1;
		this.segment2 = s2;
	}
	//交点の取得
	public Vector2 GetIntersectionPoint () {
		Vector2 p = Vector2.zero;
		segment1.GetIntersectionPoint (segment2, ref p);
		return p;
	}

	public override string ToString () {
		return segment1 + " : " + segment2;
	}

	public override bool Equals (object obj) {
		if (obj == this) {
			return true;
		} else if (obj is Intersection) {
			Intersection other = (Intersection)obj;
			//segment1とsegment2を交換しても同値生が変わらないようにする
			if (segment1.Equals (other.segment1) && segment2.Equals (other.segment2)) {
				return true;
			} else if (segment1.Equals (other.segment2) && segment2.Equals (other.segment1)) {
				return true;
			}
		}
		return false;
	}

	public override int GetHashCode () {
		// segment1とsegment2を交換してもハッシュ値が変わらないようにする
		return segment1.GetHashCode () + segment2.GetHashCode ();
	}
}