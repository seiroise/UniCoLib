using UnityEngine;
using System;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.ChainLine;
using Seiro.Scripts.Geometric.Polygon.Operation;

namespace Seiro.Scripts.Geometric.Diagram.Voronoi {

	/// <summary>
	/// ボロノイ図操作用
	/// </summary>
	public class VoronoiDiagram : MonoBehaviour {

		/// <summary>
		/// ボロノイ図の範囲の種類
		/// </summary>
		public enum AreaType {
			Square,
			ConvexPolygon
		}


		[Header("Site")]
		public Transform siteParent;		//ボロノイサイトの親オブジェクト
		private VoronoiSite[] sites;		//ボロノイサイトオブジェクト
		private List<Vector2> sitePoses;    //サイトの座標

		[Header("Area(Local)")]
		public Vector2 topLeft = new Vector2(-5f, 5f);
		public Vector2 bottomRight = new Vector2(5f, -5f);

		[Header("Line")]
		public bool drawLine = false;
		public ChainLineFactory lineFactory;    //補助線描画用
		public Color lineColor = Color.white;   //補助線の色
		private List<ChainLine> lines;

		//Other
		private VoronoiDiagramGenerator voronoiGenerator;
		private bool update = false;	//更新用ダーティフラグ

		#region UnityEvent

		private void Start() {
			//初期化
			voronoiGenerator = new VoronoiDiagramGenerator();
			sitePoses = new List<Vector2>();
			lines = new List<ChainLine>();

			sites = siteParent.GetComponentsInChildren<VoronoiSite>();

			//サイトを巡回して諸々
			for(int i = 0; i < sites.Length; ++i) {
				sitePoses.Add(sites[i].transform.position);
				sites[i].name = i.ToString();
				sites[i].TransformDetector.positionChangeCallback = OnSitePositionChange;
			}
			//更新フラグを立てる
			update = true;
		}

		private void Update() {
			if(!update) return;
			UpdateDiagram();
			update = false;
		}

		#endregion

		#region Function

		/// <summary>
		/// 図の更新
		/// </summary>
		public void UpdateDiagram() {
			Debug.Log("Update");
			//ボロノイ図の作成
			Vector2 pos = transform.position;
			ConvexPolygon areaPolygon = ConvexPolygon.SquarePolygon(topLeft + pos, bottomRight + pos);
			List<ConvexPolygon> regions = voronoiGenerator.Execute(areaPolygon, sitePoses);

			for(int i = 0; i < lines.Count; ++i) {
				lineFactory.DeleteLine(lines[i]);
			}
			lines.Clear();

			for(int i = 0; i < sites.Length; ++i) {
				ConvexPolygon region = regions[i];

				//線の描画
				if(drawLine) {
					List<Vector3> vertices = region.GetVertices3Copy();
					lines.Add(lineFactory.CreateLine(vertices, lineColor));
				}

				//位置の調整
				region.Translate(-sitePoses[i]);
				sites[i].PolygonObject.Origin = region;
			}
		}

		#endregion

		#region Callback

		/// <summary>
		/// ボロノイサイトの座標変更
		/// </summary>
		private void OnSitePositionChange(Transform trans) {
			int index;
			if(int.TryParse(trans.name, out index)) {
				sitePoses[index] = trans.position;
				update = true;
			}
		}

		#endregion
	}
}