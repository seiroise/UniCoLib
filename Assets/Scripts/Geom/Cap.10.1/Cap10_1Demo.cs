using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;
using Seiro.Scripts.Geometric.Polygon.Operation;
using Seiro.Scripts.Graphics.ChainLine;

/// <summary>
/// チャプター10.1のデモシーン
/// テスト
/// </summary>
public class Cap10_1Demo : MonoBehaviour {

	[Header("Polygon")]
	public PolygonGameObject polygon1;
	public PolygonGameObject polygon2;

	[Header("Line")]
	public ChainLineFactory lineFactory;
	private ChainLine line = null;
	private ChainLine leftEdge = null;
	private ChainLine rightEdge = null;

	#region UnityEvent

	private void Update() {
		ConvexPolygon p1 = polygon1.Polygon;
		ConvexPolygon p2 = polygon2.Polygon;

		if(p1 == null || p2 == null) return;
		ConvexPolygon polygon = IntersectionOperation.Execute(p1, p2, lineFactory);

		//DrawLine
		if(polygon == null) return;
		List<Vector3> vertices = polygon.GetVertices3Copy();
		vertices.Add(vertices[0]);		//末尾に先頭を追加
		if(line != null) {
			lineFactory.DeleteLine(line);
		}
		//line = lineFactory.CreateLine(vertices, Color.green);

		//DrawEdgeLine
		//DrawEdgeLine(p1, leftEdge);
	}

	#endregion

	#region Function

	private void DrawEdgeLine(ConvexPolygon p, ChainLine line) {
		IntersectionOperation.Edge left = IntersectionOperation.CreateEdgeChain(p, true);
		List<Vector3> vertices = new List<Vector3>();
		vertices.Add(left.startPoint);
		foreach(var e in left) {
			vertices.Add(e.endPoint);
		}
		if(line != null) {
			lineFactory.DeleteLine(line);
		}
		line = lineFactory.CreateLine(vertices, Color.red);
	}

	#endregion
}