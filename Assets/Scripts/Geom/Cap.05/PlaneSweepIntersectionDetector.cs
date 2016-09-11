using System.Collections.Generic;
using Seiro.Scripts.Geometric;

public class PlaneSweepIntersectionDetector : IntersectionDetector {

	public ICollection<Intersection> Execute (List<LineSegment> segments) {
		/*
		//イベントキュー
		TreeSet<DetectorEvent> eventQueue = new TreeSet<DetectorEvent> (null);

		foreach (LineSegment s in segments) {
			//線分の端点のうち上にある方を始点，下にある方を終点としてイベントを登録
			//線分が水平な場合は左の端点を始点とする
			if (s.p1.y < s.p2.y || (s.p1.y == s.p2.y && s.p1.x < s.p2.x)) {
				//始点
				eventQueue.Add (new DetectorEvent (DetectorEvent.Type.SEGMENT_START, new Vector2 (s.p1.x, s.p1.y), s, null));
				//終点
				eventQueue.Add (new DetectorEvent (DetectorEvent.Type.SEGMENT_END, new Vector2 (s.p2.x, s.p2.y), s, null));
			} else {
				//始点
				eventQueue.Add (new DetectorEvent (DetectorEvent.Type.SEGMENT_START, new Vector2 (s.p2.x, s.p2.y), s, null));
				//終点
				eventQueue.Add (new DetectorEvent (DetectorEvent.Type.SEGMENT_END, new Vector2 (s.p1.x, s.p1.y), s, null));
			}
		}
		*/

		//走査線
		SweepLineBasedComparator sweepComparator = new SweepLineBasedComparator ();
		//ステータスを作成。要素の順序関係はsweepComparatorに従う
		//TreeSet<LineSegment> status = new TreeSet<LineSegment> (sweepComparator);

		//今回の実装では同一の交点が複数回検出される可能性があるため，HashSetを使って重複を防ぐ
		//同一の交点が複数検出されるパターン -> 複数の交点が重なっている場合
		ICollection<Intersection> result = new HashSet<Intersection> ();
		return result;
		/*
		DetectorEvent e;
		//キューから先頭のイベントを取り出す
		while ((e = eventQueue.pollFirst ()) != null) {
			eventQueue.
			float sweepY = e.y;
			switch (e.type) {
			case EventDetector.Type.SEGMENT_START:  //始点イベントの場合
				sweepComparator.setY (sweepY); //走査線を更新

				LineSegment newSegment = event.segment1;
				status.add(newSegment); //ステータスに線分を追加

				LineSegment left = status.lower (newSegment);
				LineSegment right = status.lower (newSegment);

				//左右の線分との交差を調べる
				checkIntersection (left, newSegment, sweepY, eventQueue);
				checkIntersection (newSegment, right, sweepY, eventQueue);

				break;
		}
		*/
	}
}