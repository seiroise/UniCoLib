using UnityEngine;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;


/// <summary>
/// チャプター6.1のデモシーン
/// 凸多角形の包含判定の可視化
/// </summary>
public class Cap6_1Demo : MonoBehaviour {

	public LineRenderer line;
	public List<Transform> vertices;
	public Transform point;
	private ConvexPolygon polygon;

	#region UnityEvent

	private void Start () {

		List<Vector2> points = new List<Vector2> (vertices.Select (elem => (Vector2)elem.position));
		//DrawLine
		int size = points.Count;
		line.SetVertexCount (size + 1);
		for (int i = 0; i < size; ++i) {
			line.SetPosition (i, vertices [i].position);
		}
		line.SetPosition (size, vertices [0].position);

		//Convex Polygon
		polygon = new ConvexPolygon (points);
	}

	private void Update () {
		if (polygon.Contains (point.position)) {
			line.SetColors (Color.red, Color.red);
		} else {
			line.SetColors (Color.white, Color.white);
		}
	}

	private void OnGUI() {
		if(polygon == null) return;
		StringBuilder sb = new StringBuilder();
		int size = polygon.GetEdgeCount();
		sb.AppendLine("Vertex Count = " + size);
		for(int i = 0; i < size; ++i) {
			sb.AppendLine("[" + i + "] = " + polygon.GetVertex(i));
		}
		int min = 0, max = 0;
		polygon.GetYMinMaxindex(ref min, ref max);
		sb.AppendLine("MinYIndex = " + min + " : MaxYIndex = " + max);

		GUILayout.Label(sb.ToString());
	}

	#endregion
}