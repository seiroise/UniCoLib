using UnityEngine;
using System;
using Seiro.Scripts.Geometric;

/// <summary>
/// 検出イベント
/// </summary>
public class DetectorEvent : IComparable<DetectorEvent> {

	//イベントの種類
	public enum Type {
		SEGMENT_START,  //線分の始点
		SEGMENT_END,    //線分の終点
		INTERSECTION    //線分同士の交差
	}

	public Type type;

	public Vector2 p;
	//点に関する線分1
	public LineSegment segment1;
	//点に関する線分2(type == INTERSECTIONのときのみ使用)
	public LineSegment segment2;

	public DetectorEvent (Type type, Vector2 p, LineSegment segment1, LineSegment segment2) {
		this.type = type;
		this.p = p;
		this.segment1 = segment1;
		this.segment2 = segment2;
	}

	/// <summary>
	/// IComparable<DetectorEvent>の実装
	/// </summary>
	public int CompareTo (DetectorEvent e) {
		//DetectorEvent e = (DetectorEvent)obj;
		int c = p.y.CompareTo (e.p.y);  //イベント点のy座標を比較
										//y座標が等しい場合はx座標を比較
		if (c == 0) {
			c = p.x.CompareTo (e.p.x);
		}
		return c;
	}
}