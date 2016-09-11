using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Seiro.Scripts.Geometric;

public class SweepLineBasedComparator : IComparer<LineSegment> {

	private Line sweepLine;
	private Line belowLine;

	public SweepLineBasedComparator () {
		SetY (0f);
	}

	/// <summary>
	/// 走査線のy座標を更新
	/// </summary>
	public void SetY (float y) {
		//走査線を更新
		sweepLine = Line.FromPoints (new Vector2 (0f, y), new Vector2 (1f, y));
		//走査線の少し下を通る線を作成
		belowLine = Line.FromPoints (new Vector2 (0f, y + 0.1f), new Vector2 (1f, y + 0.1f));
	}

	/// <summary>
	/// s1とs2を,lineとの交点のx座標に基づいて比較
	/// </summary>
	private int CompareByLine (LineSegment s1, LineSegment s2, Line line) {
		Vector2 p1 = Vector2.zero, p2 = Vector2.zero;
		float x1 = s1.ToLine ().GetIntersectionPoint (line, ref p1) ? p1.x : s1.p1.x;
		float x2 = s2.ToLine ().GetIntersectionPoint (line, ref p2) ? p2.x : s2.p1.x;
		return x1.CompareTo (x2);
	}

	/// <summary>
	/// Comparable<LineSegment>の実装
	/// </summary>
	public int Compare (LineSegment s1, LineSegment s2) {
		int c = CompareByLine (s1, s2, sweepLine);
		//走査線上の交点が等しい場合は走査線の少し下の位置で比較
		if (c == 0) {
			c = CompareByLine (s1, s2, belowLine);
		}
		return c;
	}
}