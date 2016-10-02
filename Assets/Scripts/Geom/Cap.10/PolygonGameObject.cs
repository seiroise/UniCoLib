using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;
using Seiro.Scripts.Graphics.ChainLine;

public class PolygonGameObject : MonoBehaviour {

	public List<Transform> vertices;
	private ConvexPolygon polygon;
	public ConvexPolygon Polygon {get {return polygon;}}

	[Header("Line")]
	public ChainLineFactory lineFactory;
	public Color lineColor = Color.gray;
	private ChainLine line = null;

	#region UnityEvent

	private void Update() {
		List<Vector2> vertices = new List<Vector2> (this.vertices.Select (elem => (Vector2)elem.position));
		//Convex Polygon
		polygon = new ConvexPolygon (vertices);

		//DrawLine
		List<Vector3> points = polygon.GetVertices3Copy();
		points.Add(points[0]);	//末尾に先頭を追加
		if(line != null) lineFactory.DeleteLine(line);

		line = lineFactory.CreateLine(points, lineColor);
	}

	#endregion
}