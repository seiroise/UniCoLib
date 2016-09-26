using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Geometric;
using Seiro.Scripts.Graphics.PolyLine2D;
using Seiro.Scripts.Geometric.Polygon;

/// <summary>
/// 多角形作成エディタ
/// </summary>
public class PolygonEditor : MonoBehaviour {

	public LineEditor lineEditor;
	public AdditionalLineProponent addtionalLine;

	public float snapForce = 0.5f;

	#region UnityEvent

	private void Start() {
		//コールバックの設定
		lineEditor.addVertexCallback = OnAddVertex;
		lineEditor.removeVertexCallback = OnRemoveVertex;
	}

	#endregion

	#region Function

	/// <summary>
	/// スナップの設定
	/// </summary>
	private void SetSnaps(Vector2 point) {
		int count = lineEditor.PolyLine.GetVertexCount();

		//スナップの追加
		addtionalLine.Clear();
		if(count >= 3) {
			Vector2 startPoint;
			lineEditor.PolyLine.GetVertex(0, out startPoint);
			addtionalLine.AddSnap(10, new PointSnap(startPoint, snapForce), OnSnapEndPoint);

		}
		//放射スナップ
		addtionalLine.AddSnap(0, new RadialSnap(point, 8, snapForce));
		//スマートスナップ
		SmartSnap();

		//描画
		addtionalLine.Draw();
	}

	/// <summary>
	/// 頂点の追加履歴からそれっぽい補助線を引く
	/// </summary>
	private void SmartSnap() {

		int count = lineEditor.PolyLine.GetVertexCount();
		PolyLine2D line = lineEditor.PolyLine;

		if(count >= 3) {
			//平行線スナップの追加
			Vector2 p1, p2, p3;
			Vector2 dir;
			line.GetVertex(count - 3, out p1);
			line.GetVertex(count - 2, out p2);
			line.GetVertex(count - 1, out p3);

			dir = (p2 - p1).normalized * 100f;
			addtionalLine.AddSnap(0, new LineSegSnap(p3 + dir, p3 - dir, snapForce));
			Line lineA = Line.FromPoints(p3, p3 + dir);

			dir = (p3 - p2).normalized * 100f;
			addtionalLine.AddSnap(0, new LineSegSnap(p1 + dir, p1 - dir, snapForce));
			Line lineB = Line.FromPoints(p1, p1 + dir);

			Vector2 intersection = Vector2.zero;
			if(lineA.GetIntersectionPoint(lineB, ref intersection)) {
				addtionalLine.AddSnap(5, new PointSnap(intersection, snapForce));
			}
		} else if(count == 2) {
			//垂直線スナップの追加
			Vector2 p1, p2;
			Vector2 vertical;
			line.GetVertex(count - 1, out p1);
			line.GetVertex(count - 2, out p2);
			vertical = (p1 - p2).normalized * 100f;
			vertical = GeomUtil.RotateVector2(vertical, 90f);
			addtionalLine.AddSnap(0, new LineSegSnap(p1 + vertical, p1 - vertical, snapForce));
		}
	}

	#endregion

	#region Callback

	/// <summary>
	/// 頂点追加
	/// </summary>
	private void OnAddVertex(Vector2 point) {
		//スナップの設定
		SetSnaps(point);
	}

	/// <summary>
	/// 頂点の削除
	/// </summary>
	private void OnRemoveVertex() {
		int count = lineEditor.PolyLine.GetVertexCount();

		addtionalLine.Clear();
		if(count != 0) {
			//スナップの追加
			Vector2 point;
			lineEditor.PolyLine.GetVertex(count - 1, out point);
			OnAddVertex(point);
		}
	}

	/// <summary>
	/// 終了座標にスナップ
	/// </summary>
	private void OnSnapEndPoint() {
		List<Vector2> vertices = lineEditor.FlushVertices();
		//最後の頂点を取り除く
		vertices.RemoveAt(vertices.Count - 1);
		ConcavePolygon p = new ConcavePolygon(vertices);
		addtionalLine.Clear();
	}

	#endregion
}