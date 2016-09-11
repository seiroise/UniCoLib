using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Seiro.Scripts.Geometric;

/// <summary>
/// チャプター10のデモシーン
/// </summary>
public class Cap10Demo : MonoBehaviour {

	[Header("Polygon A")]
	public LineRenderer aLine;
	public Transform[] aVertices;

	[Header("Polygon B")]
	public LineRenderer bLine;
	public Transform[] bVertices;

	[Header("Intersection Polygon")]
	public LineRenderer iLine;

	#region UnityEvent

	private void Update() {
		/*
		List<Vector2> aVerts = new List<Vector2>(aVertices.Select(elem => (Vector2)elem.position));
		aLine.SetVertexCount(aVertices.Length + 1);
		for(int i = 0; i < aVerts.Count; ++i) aLine.SetPosition(i, aVerts[i]);
		aLine.SetPosition(aVerts.Count, aVerts[0]);

		List<Vector2> bVerts = new List<Vector2>(bVertices.Select(elem => (Vector2)elem.position));
		bLine.SetVertexCount(bVertices.Length + 1);
		for(int i = 0; i < bVerts.Count; ++i) bLine.SetPosition(i, bVerts[i]);
		bLine.SetPosition(bVerts.Count, bVerts[0]);

		ConvexPolygon polygon = PolygonIntersectionCalculator.Execute(aVerts, bVerts);
		if(polygon != null) {
			int size = polygon.GetEdgeCount();
			iLine.SetVertexCount(size + 1);
			for(int i = 0; i < size; ++i) iLine.SetPosition(i, polygon.GetVertex(i));
			iLine.SetPosition(size, polygon.GetVertex(0));
		}
		*/
	}

	#endregion
}