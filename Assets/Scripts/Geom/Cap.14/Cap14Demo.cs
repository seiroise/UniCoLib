using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;
using Seiro.Scripts.Geometric.Diagram;
using Seiro.Scripts.Geometric.Diagram.Voronoi;
using Seiro.Scripts.Graphics.ChainLine;

/// <summary>
/// チャプター14のデモシーン
/// 凸多角形への入力処理
/// </summary>
public class Cap14Demo : MonoBehaviour {

	[Header("Site")]
	public DraggableObject[] siteObjects;   //ボロノイ母点(サイト)
	private List<Vector2> sitePoses;			//ボロノイ母点の座標
	private List<ConvexPolygon> regions;		//ボロノイ領域

	[Header("ChainLine")]
	public ChainLineFactory lineFactory;
	public Gradient gradient;
	private List<ChainLine> lines = new List<ChainLine>();
	public bool drawRegionLine = true;

	[Header("Mesh")]
	public Material mat;
	private Mesh[] meshes;

	private List<MeshCollider> colliders;

	private ConvexPolygon areaPolygon;
	private PseudoHalfPlaneGenerator halfPlaneGenerator;
	private VoronoiDiagramGenerator voronoiGenerator;

	#region UnityEvent

	private void Start() {

		//初期化
		meshes = new Mesh[siteObjects.Length];
		colliders = new List<MeshCollider>();
		sitePoses = new List<Vector2>();
		for(int i = 0; i < siteObjects.Length; ++i) {
			siteObjects[i].MoveCallback = OnSiteMove;			//コールバックの設定
			sitePoses.Add(siteObjects[i].transform.position);	//初期座標の設定
			colliders.Add(gameObject.AddComponent<MeshCollider>());
		}

		//図の更新と描画
		UpdateDiagram(sitePoses);
		DrawDiagram();
	}

	private void Update() {
		DrawDiagram();
	}

	#endregion

	#region Function

	/// <summary>
	/// 図の更新
	/// </summary>
	private void UpdateDiagram(List<Vector2> sites) {
		//ボロノイ図の作成
		areaPolygon = ConvexPolygon.SquarePolygon(10f);
		voronoiGenerator = new VoronoiDiagramGenerator();
		regions = voronoiGenerator.Execute(areaPolygon, sites);

		//線の削除
		for(int i = 0; i < lines.Count; ++i) {
			lineFactory.DeleteLine(lines[i]);
		}
		lines.Clear();

		for(int i = 0; i < regions.Count; ++i) {
			ConvexPolygon region = regions[i];

			//線の描画
			if(drawRegionLine) {
				List<Vector3> vertices = region.GetVertices3Copy();
				vertices.Add(vertices[0]);
				lines.Add(lineFactory.CreateLine(vertices));
			}

			//メッシュ
			meshes[i] = region.Scale(sites[i], 0.5f).ToMesh();//AngleSubdivisionOperation.Execute(region, 170f).Scale(sites[i], 1f).ToMesh();

			//あたり判定の設定
			colliders[i].sharedMesh = meshes[i];
		}
	}

	/// <summary>
	/// 図の描画
	/// </summary>
	private void DrawDiagram() {
		for(int i = 0; i < regions.Count; ++i) {
			//メッシュの描画
			UnityEngine.Graphics.DrawMesh(meshes[i], Matrix4x4.identity, mat, 0);
		}
	}

	#endregion

	#region Callback

	/// <summary>
	/// サイトが動いた時に呼ばれる
	/// </summary>
	private void OnSiteMove(Transform trans) {
		//名前を数値に変換
		int index;
		if(int.TryParse(trans.name, out index)) {
			sitePoses[index] = trans.position;
			//図の更新と描画
			UpdateDiagram(sitePoses);
			DrawDiagram();
		}
	}

	#endregion
}