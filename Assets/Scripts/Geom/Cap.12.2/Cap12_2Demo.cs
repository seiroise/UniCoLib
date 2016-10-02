using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Seiro.Scripts.Geometric;
using Seiro.Scripts.Geometric.Polygon.Convex;
using Seiro.Scripts.Geometric.Polygon.Operation;
using Seiro.Scripts.Geometric.Diagram;
using Seiro.Scripts.Graphics.ChainLine;

/// <summary>
/// チャプター12.2のデモシーン
/// ボロノイ図の可視化
/// </summary>
public class Cap12_2Demo : MonoBehaviour {

	public ChainLineFactory lineFactory;
	public Transform[] siteTranses;		//ボロノイ母点

	private ConvexPolygon areaPolygon;
	private PseudoHalfPlaneGenerator halfPlaneGenerator;

	private List<ChainLine> lines;

	#region UnityEvent

	private void Start() {
		areaPolygon = ConvexPolygon.SquarePolygon(10f);
		halfPlaneGenerator = new PseudoHalfPlaneGenerator(100f);
		lines = new List<ChainLine>();
	}

	private void Update() {
		List<Vector2> sites = new List<Vector2>(siteTranses.Select(elem => (Vector2)elem.position));
		List<ConvexPolygon> results = new List<ConvexPolygon>();

		//線の削除
		if(lines.Count > 0) {
			for(int i = 0; i < lines.Count; ++i) lineFactory.DeleteLine(lines[i]);
			lines.Clear();
		}

		foreach(Vector2 s1 in sites) {
			ConvexPolygon region = null;	//途中計算結果格納用の領域
			foreach(Vector2 s2 in sites) {
				if(s1 == s2) {
					continue;
				}
				//s1とs2の垂直二等分線を求める
				Line line = Line.PerpendicularBisector(s1, s2);
				//垂直二等分線による半平面のうち，s1を含む方を求める
				ConvexPolygon halfPlane = halfPlaneGenerator.Execute(line, s1);
				if(region == null) {
					//初回計算時
					region = IntersectionOperation.Execute(areaPolygon, halfPlane);
				} else {
					//二回目以降
					region = IntersectionOperation.Execute(region, halfPlane);
				}
			}
			if(region != null) {
				results.Add(region);
			} else {
				Debug.Log("Region is null");
			}
 		}

		for(int i = 0; i < results.Count; ++i) {
			results[i].Scale(sites[i], 0.9f);
			List<Vector3> vertices = results[i].GetVertices3Copy();
			vertices.Add(vertices[0]);
			lines.Add(lineFactory.CreateLine(vertices, Color.green));
		}
	}

	#endregion

	#region Coroutine

	/// <summary>
	/// ボロノイ図生成コルーチン
	/// </summary>
	private IEnumerator Voronoi() {
		

		while(true) {

			List<Vector2> sites = new List<Vector2>(siteTranses.Select(elem => (Vector2)elem.position));
			List<ConvexPolygon> results = new List<ConvexPolygon>();
			List<ChainLine> lines = new List<ChainLine>();

			foreach(Vector2 s1 in sites) {
				ConvexPolygon region = null;	//途中計算結果格納用の領域
				foreach(Vector2 s2 in sites) {
					if(s1 == s2) {
						continue;
					}
					//ウェイト
					yield return StartCoroutine(WaitClick());

					//s1とs2の垂直二等分線を求める
					Line line = Line.PerpendicularBisector(s1, s2);
					//垂直二等分線による半平面のうち，s1を含む方を求める
					ConvexPolygon halfPlane = halfPlaneGenerator.Execute(line, s1);

					//線を描画
					lines.Add(lineFactory.CreateLine(halfPlane.GetVertices3Copy()));

					if(region == null) {
						//初回計算時
						region = IntersectionOperation.Execute(areaPolygon, halfPlane);
					} else {
						//二回目以降
						region = IntersectionOperation.Execute(region, halfPlane);
					}
				}
				results.Add(region);
			}

			for(int i = 0; i < results.Count; ++i) {
				lines.Add(lineFactory.CreateLine(results[i].GetVertices3Copy(), Color.red));
			}
			Debug.Log("Complete");

			yield return StartCoroutine(WaitClick());

			for(int i = 0; i < lines.Count; ++i) lineFactory.DeleteLine(lines[i]);
			lines.Clear();
		}
	}

	/// <summary>
	/// クリックするまで待機
	/// </summary>
	private IEnumerator WaitClick() {
		while(!Input.GetKeyDown(KeyCode.Space)) {
			yield return 0f;
		}
	}

	#endregion
}