using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;
using Seiro.Scripts.Geometric.Diagram.Voronoi;

/// <summary>
/// チャプター12のデモシーン
/// ボロノイ図の作成
/// </summary>
public class Cap12Demo : MonoBehaviour {

	public Transform[] sites;   //ボロノイ母点

	private VoronoiDiagramGenerator voronoiDiagram; //ボロノイ図
	private ConvexPolygon areaPolygon;              //範囲用のポリゴン

	#region UnityEvent

	private void Start() {
		areaPolygon = ConvexPolygon.SquarePolygon(10f);
		//areaPolygon.DrawLine(new GameObject().AddComponent<LineRenderer>());

		voronoiDiagram = new VoronoiDiagramGenerator();

		List<Vector2> list = new List<Vector2>(sites.Select(elem => (Vector2)elem.position));

		List<ConvexPolygon> regions = voronoiDiagram.Execute(areaPolygon, list);
		if(regions == null) return;

		foreach(var r in regions) {
			var obj = new GameObject("Region Line");
			obj.transform.SetParent(transform);
			//r.DrawDebugLine(obj.AddComponent<LineRenderer>());
		}
	}
	#endregion
}