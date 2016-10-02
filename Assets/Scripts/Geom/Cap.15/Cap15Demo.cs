using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;
using Seiro.Scripts.Geometric.Diagram;
using Seiro.Scripts.Geometric.Diagram.Voronoi;
using Seiro.Scripts.Graphics.ChainLine;

/// <summary>
/// チャプター15デモシーン
/// 重み付きボロノイ図
/// </summary>
public class Cap15Demo : MonoBehaviour {

	[Header("Sites")]
	public Transform siteParent;
	private List<WaitedSiteObject> siteObjects;

	[Header("ChainLine")]
	public ChainLineFactory lineFactory;
	private List<ChainLine> lines;

	//Other
	private ConvexPolygon areaPolygon;
	private PseudoHalfPlaneGenerator halfPlaneGenerator;
	private WaitedVoronoiDiagramGenerator voronoiGenerator;

	#region UnityEvent

	private void Start() {

		//サイトの登録
		siteObjects = new List<WaitedSiteObject>();
		foreach(Transform c in siteParent) {
			var site = c.GetComponent<WaitedSiteObject>();
			if(site != null) siteObjects.Add(site);
		}

		lines = new List<ChainLine>();
		areaPolygon = ConvexPolygon.SquarePolygon(10f);
		voronoiGenerator = new WaitedVoronoiDiagramGenerator();
	}

	private void Update() {
		List<WaitedVoronoiSite> sites =
			new List<WaitedVoronoiSite>(siteObjects.Select(elem => new WaitedVoronoiSite(elem.transform.position, elem.Wait)));

		//ボロノイ図の作成
		List<ConvexPolygon> regions = voronoiGenerator.Execute(areaPolygon, sites);

		List<Vector3> vertices;

		for(int i = 0; i < lines.Count; ++i) {
			lineFactory.DeleteLine(lines[i]);
		}
		lines.Clear();

		for(int i = 0; i < regions.Count; ++i) {
			ConvexPolygon region = regions[i];
			vertices = region.GetVertices3Copy();
			vertices.Add(vertices[0]);
			lines.Add(lineFactory.CreateLine(vertices));
		}
	}

	#endregion
}