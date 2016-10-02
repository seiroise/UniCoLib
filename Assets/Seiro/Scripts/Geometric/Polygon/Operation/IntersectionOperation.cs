using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.ChainLine;
using Seiro.Scripts.Geometric.Polygon.Convex;

namespace Seiro.Scripts.Geometric.Polygon.Operation {

	/// <summary>
	/// 凸多角形の交差領域抽出
	/// </summary>
	public class IntersectionOperation {

		#region InternalClass

		/// <summary>
		/// 連結辺クラス
		/// </summary>
		public class Edge : IEnumerable {
			public LineSegment segment; //辺の線分
			public Vector2 startPoint;  //辺の始点(巡回順なのでsegmentのp1，p2とは異なる時もある)
			public Vector2 endPoint;    //辺の終点(巡回順なので(ry)
			public Edge next;           //次の辺
			public int index;           //元の多角形内での辺番号

			#region IEnumerable

			public IEnumerator<Edge> GetEnumerator() {
				return new EdgeEnumerator(this);
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return new EdgeEnumerator(this);
			}

			#endregion

			#region Function

			public override string ToString() {
				StringBuilder sb = new StringBuilder();
				foreach(var e in this) {
					sb.AppendLine("[" + e.index + "] = " + e.startPoint + " -> " + e.endPoint);
				}
				return sb.ToString();
			}

			/// <summary>
			/// 他の辺との交差判定
			/// </summary>
			public bool Intersects(Edge other) {
				return segment.Intersects(other.segment);
			}

			/// <summary>
			/// 他の辺との交点を求める
			/// 交差しない場合はNaNを返す
			/// </summary>
			public bool GetIntersectionPoint(Edge other, ref Vector2 p) {
				return segment.GetIntersectionPoint(other.segment, ref p);
			}

			/// <summary>
			/// 直線との交点のx座標を求める
			/// 交差しない場合はNaNを返す
			/// </summary>
			public float GetIntersectionX(Line line) {
				Vector2 p = Vector2.zero;
				return segment.GetIntersectionPoint(line, ref p) ? p.x : startPoint.x;
			}

			#endregion
		}

		/// <summary>
		/// 連結辺クラスの列挙
		/// </summary>
		public class EdgeEnumerator : IEnumerator<Edge> {

			private Edge start;     //開始
			private Edge current;   //現在地

			#region IEnumerator

			public EdgeEnumerator(Edge start) {
				this.start = start;
				this.current = null;
			}

			Edge IEnumerator<Edge>.Current {
				get {
					return current;
				}
			}

			public object Current {
				get {
					return current;
				}
			}

			public bool MoveNext() {
				if(current == null) {
					current = start;
				} else {
					current = current.next;
				}
				return current != null;
			}

			public void Reset() {
				current = null;
			}

			public void Dispose() {
			}

			#endregion
		}

		/// <summary>
		/// 走査線と交差する4つの辺を格納するステータス
		/// </summary>
		private class Status {
			public Edge left1;
			public Edge right1;
			public Edge left2;
			public Edge right2;
		}

		/// <summary>
		/// イベントの処理結果
		/// </summary>
		private class StepResult {
			public float nextSweepY;    //次の走査線位置
			public bool mustContinue;   //これ以降も処理を継続する必要があるかどうか

			public StepResult(float nextSweepY, bool mustContinue) {
				this.nextSweepY = nextSweepY;
				this.mustContinue = mustContinue;
			}
		}

		#endregion

		#region Enum

		/// <summary>
		/// 走査線上での辺の相対位置
		/// </summary>
		private enum EdgePosition {
			NULL,
			LEFT1,
			RIGHT1,
			LEFT2,
			RIGHT2
		}

		#endregion

		#region StaticFunction

		/// <summary>
		/// 凸多角形の交差領域の抽出
		/// </summary>
		public static ConvexPolygon Execute(ConvexPolygon polygon1, ConvexPolygon polygon2) {
			//どちらかがnullならnullを返す
			if(polygon1 == null || polygon2 == null) return null;

			//ステータスなど下準備
			Status status = new Status();
			List<Edge> leftEdge = new List<Edge>();     //左側の計算結果
			List<Edge> rightEdge = new List<Edge>();    //右側の計算結果

			//一回目のイベントを処理
			StepResult result = FirstPass(polygon1, polygon2, status,
										  leftEdge, rightEdge);
			//二回目以降のイベントを処理
			while(result.mustContinue) {
				//二番目以降のイベントを処
				result = SecondPass(status, leftEdge, rightEdge);
			}

			//左側と右側の計算結果を統合するリスト
			List<Edge> totalEdge = new List<Edge>();
			totalEdge.AddRange(leftEdge);
			rightEdge.Reverse();			//反転して連結
			totalEdge.AddRange(rightEdge);

			//交差凸多角形の交点リスト
			List<Vector2> resultPoints = new List<Vector2>();
			Vector2 lastPoint = Vector2.one * float.NaN;
			int totalSize = totalEdge.Count;
			for(int i = 0; i < totalSize; ++i) {
				Edge e1 = totalEdge[i];
				Edge e2 = totalEdge[(i + 1) % totalSize];
				Vector2 p = Vector2.zero;
				if(e1.GetIntersectionPoint(e2, ref p)) {
					resultPoints.Add(p);
				}
			}

			//頂点が3つ以上なら凸多角形を作成
			if(resultPoints.Count >= 3) {
				return new ConvexPolygon(resultPoints);
			} else {
				return null;
			}
		}

		public static ConvexPolygon Execute(ConvexPolygon polygon1, ConvexPolygon polygon2, ChainLineFactory lineFactory) {
			//どちらかがnullならnullを返す
			if(polygon1 == null || polygon2 == null) return null;

			//ステータスなど下準備
			Status status = new Status();
			List<Edge> leftEdge = new List<Edge>();     //左側の計算結果
			List<Edge> rightEdge = new List<Edge>();    //右側の計算結果

			//一回目のイベントを処理
			StepResult result = FirstPass(polygon1, polygon2, status,
			                              leftEdge, rightEdge);
			//二回目以降のイベントを処理
			while(result.mustContinue) {
				//二番目以降のイベントを処
				result = SecondPass(status, leftEdge, rightEdge);
			}

			//左側と右側の計算結果を統合するリスト
			List<Edge> totalEdge = new List<Edge>();
			totalEdge.AddRange(leftEdge);
			rightEdge.Reverse();			//反転して連結
			totalEdge.AddRange(rightEdge);

			//線の描画
			List<Vector3> vs = new List<Vector3>();
			vs.Add(leftEdge[0].startPoint);
			foreach(var e in leftEdge) vs.Add(e.endPoint);
			lineFactory.CreateLine(vs, Color.red);

			vs = new List<Vector3>();
			vs.Add(rightEdge[0].startPoint);
			foreach(var e in rightEdge) vs.Add(e.endPoint);
			lineFactory.CreateLine(vs, Color.blue);

			//交差凸多角形の交点リスト
			List<Vector2> resultPoints = new List<Vector2>();
			Vector2 lastPoint = Vector2.one * float.NaN;
			int totalSize = totalEdge.Count;
			for(int i = 0; i < totalSize; ++i) {
				Edge e1 = totalEdge[i];
				Edge e2 = totalEdge[(i + 1) % totalSize];
				Vector2 p = Vector2.zero;
				if(e1.GetIntersectionPoint(e2, ref p)) {
					resultPoints.Add(p);
				}
			}

			//頂点が3つ以上なら凸多角形を作成
			if(resultPoints.Count >= 3) {
				return new ConvexPolygon(resultPoints);
			} else {
				return null;
			}
		}

		/// <summary>
		/// 一回目のイベント処理
		/// </summary>
		private static StepResult FirstPass(ConvexPolygon polygon1, ConvexPolygon polygon2,
											Status status, List<Edge> leftEdge, List<Edge> rightEdge) {
			//凸多角形の左右の連結辺を作成
			Edge left1 = CreateEdgeChain(polygon1, true);
			Edge right1 = CreateEdgeChain(polygon1, false);
			Edge left2 = CreateEdgeChain(polygon2, true);
			Edge right2 = CreateEdgeChain(polygon2, false);

			if(left1 == null || right1 == null) {
				polygon1.LogVertexDebugInfo();
			}

			//Edgeチェインの最上点の座標を取得
			Vector2 top1 = left1.startPoint;
			Vector2 top2 = left2.startPoint;

			//２つの凸多角形の最上点がより下にある方のy座標を走査線の初期位置とする
			float sweepY = Mathf.Min(top1.y, top2.y);
			//走査線を作成
			Line sweepLine = Line.FromPoints(0f, sweepY, 1f, sweepY);

			//Edgeチェインの中からはじめに走査線と交わるEdgeを見つけてステータスに設定
			status.left1 = FindFirstEdge(left1, sweepLine);
			status.right1 = FindFirstEdge(right1, sweepLine);
			status.left2 = FindFirstEdge(left2, sweepLine);
			status.right2 = FindFirstEdge(right2, sweepLine);

			//いずれかのEdgeが見つからなければ，交差部分は存在しないので終了
			if(status.left1 == null || status.right1 == null || status.left2 == null || status.right2 == null) {
				return new StepResult(sweepY, false);
			}

			//初回のイベント処理
			if(top1.y < top2.y) {
				if(top1.x < top2.x) {
					Process(status, EdgePosition.LEFT1, sweepLine, leftEdge, rightEdge);
					Process(status, EdgePosition.RIGHT1, sweepLine, leftEdge, rightEdge);
				} else {
					Process(status, EdgePosition.RIGHT1, sweepLine, leftEdge, rightEdge);
					Process(status, EdgePosition.LEFT1, sweepLine, leftEdge, rightEdge);
				}
			} else {
				if(top1.x < top2.x) {
					Process(status, EdgePosition.LEFT2, sweepLine, leftEdge, rightEdge);
					Process(status, EdgePosition.RIGHT2, sweepLine, leftEdge, rightEdge);
				} else {
					Process(status, EdgePosition.RIGHT2, sweepLine, leftEdge, rightEdge);
					Process(status, EdgePosition.LEFT2, sweepLine, leftEdge, rightEdge);
				}
			}

			return new StepResult(sweepY, true);
		}

		/// <summary>
		/// 二回目以降のイベント処理
		/// </summary>
		private static StepResult SecondPass(Status status, List<Edge> leftEdge, List<Edge> rightEdge) {
			//次に処理すべき辺を選択
			Edge next = PickNextEdge(status);
			if(next == null) {
				//見つからなければ終了
				return new StepResult(float.NaN, false);
			}

			//選択された辺の終点を走査線の次の位置とする
			float nextSweepY = next.startPoint.y;
			//次の走査線を作成
			Line sweepLine = Line.FromPoints(0f, nextSweepY, 1f, nextSweepY);

			//選択された辺が，ステータスの中でleft1/right1/left2/right2のうち
			//どれと対応するかを特定する
			EdgePosition edgePos = EdgePosition.NULL;
			if(next == status.left1.next) {
				edgePos = EdgePosition.LEFT1;
				status.left1 = next;
			} else if(next == status.right1.next) {
				edgePos = EdgePosition.RIGHT1;
				status.right1 = next;
			} else if(next == status.left2.next) {
				edgePos = EdgePosition.LEFT2;
				status.left2 = next;
			} else if(next == status.right2.next) {
				edgePos = EdgePosition.RIGHT2;
				status.right2 = next;
			}

			//イベント処理
			Process(status, edgePos, sweepLine, leftEdge, rightEdge);
			return new StepResult(nextSweepY, true);
		}

		/// <summary>
		/// ステータス中でposの位置にある辺の始点をイベント点として処理する
		/// 交差部分の辺を左右別に求め，leftEdgeとrightEdgeに追加する
		/// </summary>
		private static void Process(Status status, EdgePosition pos, Line sweepLine,
									List<Edge> leftEdge, List<Edge> rightEdge) {
			switch(pos) {
			case EdgePosition.LEFT1:
				ProcessLeft(status.left1, status.right1, status.left2, status.right2,
							sweepLine, leftEdge, rightEdge);
				break;
			case EdgePosition.RIGHT1:
				ProcessRight(status.left1, status.right1, status.left2, status.right2,
							sweepLine, leftEdge, rightEdge);
				break;
			case EdgePosition.LEFT2:
				ProcessLeft(status.left2, status.right2, status.left1, status.right1,
							 sweepLine, leftEdge, rightEdge);
				break;
			case EdgePosition.RIGHT2:
				ProcessRight(status.left2, status.right2, status.left1, status.right1,
							sweepLine, leftEdge, rightEdge);
				break;
			}
		}

		/// <summary>
		/// left1の始点をイベント点として処理する
		/// </summary>
		private static void ProcessLeft(Edge left1, Edge right1, Edge left2, Edge right2,
										Line sweepLine, List<Edge> leftEdge, List<Edge> rightEdge) {
			float l1 = left1.GetIntersectionX(sweepLine);
			float l2 = left2.GetIntersectionX(sweepLine);
			float r2 = right2.GetIntersectionX(sweepLine);

			//条件1: left1がleft2とright2の内部から始まる場合
			if(l1 > l2 && l1 < r2) {
				//left1は交差凸多角形の一部
				leftEdge.Add(left1);
			}

			//条件2: left1がright2と交わり，right2よりも右から始まる場合
			if(left1.Intersects(right2) && l1 >= r2) {
				//left1がright2ともに交差凸多角形の一部，かつ
				//必ず上端となるため結果の先頭位置に追加
				leftEdge.Insert(0, left1);
				rightEdge.Insert(0, right2);
			}

			//left1がleft2と交わる場合
			if(left1.Intersects(left2)) {
				if(l1 < l2) {
					//条件3: left1がleft2よりも左から始まるなら
					leftEdge.Add(left1);
				} else {
					//条件4: そうでないなら
					leftEdge.Add(left2);
				}
			}
		}

		/// <summary>
		/// ritgh1の始点をイベント点として処理する
		/// </summary>
		private static void ProcessRight(Edge left1, Edge right1, Edge left2, Edge right2,
										 Line sweepLine, List<Edge> leftEdge, List<Edge> rightEdge) {
			float r1 = right1.GetIntersectionX(sweepLine);
			float l2 = left2.GetIntersectionX(sweepLine);
			float r2 = right2.GetIntersectionX(sweepLine);

			//条件1: right1がleft2とright2の内部から始まる場合
			if(r1 > l2 && r1 < r2) {
				//left1は交差凸多角形の一部
				rightEdge.Add(right1);
			}

			//条件2: right1がleft2と交わり，left2よりも左から始まる場合
			if(right1.Intersects(left2) && r1 <= l2) {
				//left1がright2ともに交差凸多角形の一部，かつ
				//必ず上端となるため結果の先頭位置に追加
				rightEdge.Insert(0, right1);
				leftEdge.Insert(0, left2);
			}

			//right1がright2と交わる場合
			if(right1.Intersects(right2)) {
				if(r1 > r2) {
					//条件3: right1がright2よりも右から始まるなら
					rightEdge.Add(right1);
				} else {
					//条件4: そうでないなら
					rightEdge.Add(right2);
				}
			}
		}

		/// <summary>
		/// 連結辺の中から，はじめに走査線と交差する辺を見つける
		/// </summary>
		private static Edge FindFirstEdge(Edge edge, Line sweepLine) {
			foreach(var e in edge) {
				if(e.segment.Intersects(sweepLine)) return e;
			}
			return null;
		}

		/// <summary>
		/// 凸多角形の「左側」または「右側」の上から下に向かう連結辺を作成する
		/// </summary>
		public static Edge CreateEdgeChain(ConvexPolygon polygon, bool left) {
			//凸多角形のy座標最小/最大値の頂点番号を取得
			int minYIndex = 0, maxYIndex = 0;
			polygon.GetYMinMaxindex(ref minYIndex, ref maxYIndex);

			//凸多角形の辺配列をを前進するか後退するかを決定
			bool ccw = polygon.rotation == ConvexPolygon.Rotation.CCW;
			bool forward = (ccw && left) || (!ccw && !left);

			//要素の準備
			int size = polygon.GetEdgeCount();
			int nextIndex = 0;
			Edge firstEdge = null;
			Edge lastEdge = null;

			//最上点の位置から開始し，最下点に到達するまで続ける
			for(int i = maxYIndex; i != minYIndex; i = nextIndex) {
				Edge edge = new Edge();
				if(forward) {
					//前進
					nextIndex = (i + 1) % size;
					edge.segment = polygon.GetEdge(i);
					edge.index = i;
				} else {
					//後退
					nextIndex = (i + size - 1) % size;
					edge.segment = polygon.GetEdge(nextIndex);
					edge.index = nextIndex;
				}

				//辺の始点と終点
				edge.startPoint = polygon.GetVertex(i);
				edge.endPoint = polygon.GetVertex(nextIndex);

				//辺の連結処理
				if(firstEdge == null) {
					firstEdge = edge;
				} else {
					lastEdge.next = edge;
				}
				lastEdge = edge;
			}

			return firstEdge;
		}

		/// <summary>
		/// ステータスの中から終点のy座標が最も上にある辺を探し，次に処理すべき辺として返す
		/// </summary>
		private static Edge PickNextEdge(Status status) {
			Edge result = ChooseEdgeWithUpperEndY(status.left1, status.right1);
			result = ChooseEdgeWithUpperEndY(result, status.left2);
			result = ChooseEdgeWithUpperEndY(result, status.right2);
			return result.next;
		}

		/// <summary>
		/// e1とe2のうち，終点のy座標がより上にあるものを選択する
		/// </summary>
		private static Edge ChooseEdgeWithUpperEndY(Edge e1, Edge e2) {
			float y1 = e1 != null ? e1.endPoint.y : float.MinValue;
			float y2 = e2 != null ? e2.endPoint.y : float.MinValue;
			if(y1 == y2) {
				if(e1 != null && e1.next != null) {
					return e1;
				} else {
					return e2;
				}
			} else if(y1 > y2) {
				return e1;
			} else {
				return e2;
			}
		}

		#endregion
	}
}