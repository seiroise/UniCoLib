using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;
using Seiro.Scripts.Geometric.Polygon.Operation;
using Seiro.Scripts.Geometric.Diagram;
using Seiro.Scripts.Geometric.Diagram.Voronoi;
using Seiro.Scripts.Graphics.ChainLine;

/// <summary>
/// チャプター13のデモシーン
/// 凸多角形の細分割処理
/// </summary>
public class Cap13_Demo : MonoBehaviour {

	[Header("ChainLine")]
	public ChainLineFactory lineFactory;
	public Gradient gradient;
	public Transform[] siteTranses;     //ボロノイ母点
	private List<ChainLine> lines = new List<ChainLine>();
	public bool drawRegionLine = true;

	[Header("Mesh")]
	public Material mat;

	[Header("Subdivision")]
	public bool angled = true;
	[Range(0f, 1f)]
	public float lerpT = 0.5f;

	private ConvexPolygon areaPolygon;
	private PseudoHalfPlaneGenerator halfPlaneGenerator;
	private VoronoiDiagramGenerator voronoiGenerator;

	#region UnityEvent

	private void Update() {
		List<Vector2> sites = new List<Vector2>(siteTranses.Select(elem => (Vector2)elem.position));

		//ボロノイ図の作成
		areaPolygon = ConvexPolygon.SquarePolygon(10f);
		voronoiGenerator = new VoronoiDiagramGenerator();
		List<ConvexPolygon> regions = voronoiGenerator.Execute(areaPolygon, sites);

		List<Vector3> vertices;

		for(int i = 0; i < lines.Count; ++i) {
			lineFactory.DeleteLine(lines[i]);
		}
		//lines.Clear();

		for(int i = 0; i < regions.Count; ++i) {
			ConvexPolygon region = regions[i];
			if(drawRegionLine) {
				vertices = region.GetVertices3Copy();
				vertices.Add(vertices[0]);
				lines.Add(lineFactory.CreateLine(vertices));
			}
			//描画
			Mesh mesh = null;
			if(angled) {
				mesh = AngleSubdivisionOperation.Execute(region, 170f).Scale(sites[i], 0.9f).ToAltMesh();
			} else {
				mesh = LerpSubdivisionOperation.Execute(region, i, lerpT).Scale(sites[i], 0.9f).ToAltMesh();
			}
			UnityEngine.Graphics.DrawMesh(mesh, Matrix4x4.identity, mat, 0);
		}
	}

	#endregion
}