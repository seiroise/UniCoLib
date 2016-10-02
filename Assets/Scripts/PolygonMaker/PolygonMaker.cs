using UnityEngine;
using System;
using System.Collections.Generic;
using Seiro.Scripts.Geometric;
using Seiro.Scripts.Graphics.PolyLine2D;
using Seiro.Scripts.Geometric.Polygon.Concave;
using Seiro.Scripts.Graphics;

/// <summary>
/// 多角形作成マン
/// </summary>
public class PolygonMaker : MonoBehaviour {

	[Header("Line")]
	public LineEditor lineEditor;

	[Header("Additional Line")]
	public AdditionalLineProponent addtionalLine;
	public float snapForce = 0.5f;

	//コールバック
	public Action<ConcavePolygon> endCallback;

	#region UnityEvent

	private void Start() {
		//コールバックの設定
		lineEditor.addVertexCallback = OnAddVertex;
		lineEditor.removeVertexCallback = OnRemoveVertex;
		lineEditor.doublePointCallback = OnDoublePointDetection;
		lineEditor.crossLineCallback = OnCrossLineDetection;
	}

	#endregion

	#region Function

	/// <summary>
	/// エディタの有効化
	/// </summary>
	public void EnableEditor() {
		gameObject.SetActive(true);
		SetSnaps(Vector2.zero);
	}

	/// <summary>
	/// エディタの無効化
	/// </summary>
	public void DisableEditor() {
		gameObject.SetActive(false);
	}

	/// <summary>
	/// スナップ(補助線)の設定
	/// </summary>
	private void SetSnaps(Vector2 point) {

		//スナップの追加
		addtionalLine.Clear();
		int vertsCount = lineEditor.PolyLine.GetVertexCount();

		//基本補助線
		BaseSnap(vertsCount);

		//提示補助線
		SmartSnap(vertsCount);

		//描画
		addtionalLine.Draw();
	}

	/// <summary>
	/// 基本補助線の設定
	/// </summary>
	private void BaseSnap(int vertsCount) {
		
		//終了スナップ
		if(vertsCount >= 3) {
			Vector2 startPoint = lineEditor.PolyLine.GetVertex(0);
			addtionalLine.AddSnap(10, new PointSnap(startPoint, snapForce), OnSnapEndPoint);

		}

		//縦横スナップ
		addtionalLine.AddSnap(0, new RadialSnap(Vector2.zero, 4, snapForce));
	}

	/// <summary>
	/// 提示補助線の設定
	/// </summary>
	private void SmartSnap(int vertsCount) {

		PolyLine2D line = lineEditor.PolyLine;

		if(vertsCount >= 3) {
			//平行線スナップの追加
			Vector2 p1, p2, p3;
			Vector2 dir;
			p1 = line.GetVertex(vertsCount - 3);
			p2 = line.GetVertex(vertsCount - 2);
			p3 = line.GetVertex(vertsCount - 1);

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
		} else if(vertsCount == 2) {
			//垂直線スナップの追加
			Vector2 p1, p2;
			Vector2 vertical;
			p1 = line.GetVertex(vertsCount - 1);
			p2 = line.GetVertex(vertsCount - 2);
			vertical = (p1 - p2).normalized * 100f;
			vertical = GeomUtil.RotateVector2(vertical, 90f);
			addtionalLine.AddSnap(0, new LineSegSnap(p1 + vertical, p1 - vertical, snapForce));
		}
	}

	/// <summary>
	/// ポリゴンの作成
	/// </summary>
	private ConcavePolygon MakePolygon() {
		//頂点の取得
		List<Vector2> vertices = lineEditor.FlushVertices();
		//最後の頂点を取り除く
		vertices.RemoveAt(vertices.Count - 1);
		return new ConcavePolygon(vertices);
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
			Vector2 point = lineEditor.PolyLine.GetVertex(count - 1);
			OnAddVertex(point);
		}
	}

	/// <summary>
	/// 終了座標にスナップ
	/// </summary>
	private void OnSnapEndPoint() {
		//コールバック
		if(endCallback != null) {
			endCallback(MakePolygon());
		}
	}

	/// <summary>
	/// 連続同一点の検出
	/// </summary>
	private void OnDoublePointDetection() {
		Debug.LogError("Double Point!");
	}

	/// <summary>
	/// 交差線分の検出
	/// </summary>
	private void OnCrossLineDetection() {
		Debug.LogError("Cross Line!");
	}

	#endregion
}