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
/// チャプター12.1のデモシーン
/// ボロノイ図の生成を可視化
/// </summary>
public class Cap12_1Demo : MonoBehaviour {

	public Transform[] siteTranses;     //ボロノイ母点

	private ConvexPolygon areaPolygon;

	[Header("可視化周り")]
	public ChainLineFactory lineFactory;
	public Gradient lineColor;
	public Color subColor = Color.gray;

	#region UnityEvent

	private void Start() {
		areaPolygon = ConvexPolygon.SquarePolygon(10f);
		StartCoroutine(Voronoi());
	}

	#endregion

	#region Coroutine

	/// <summary>
	/// ボロノイ図生成コルーチン
	/// </summary>
	private IEnumerator Voronoi() {
		PseudoHalfPlaneGenerator halfPlaneGenerator = new PseudoHalfPlaneGenerator(100f);

		while(true) {

			List<Vector2> sites = new List<Vector2>(siteTranses.Select(elem => (Vector2)elem.position));
			List<ConvexPolygon> results = new List<ConvexPolygon>();
			List<ChainLine> lines = new List<ChainLine>();

			for(int i = 0; i < sites.Count; ++i) {
				Vector3 s1 = sites[i];
				ConvexPolygon region = null;    //途中計算結果格納用の領域
				for(int j = 0; j < sites.Count; ++j) {
					Vector3 s2 = sites[j];
					if(i == j) {
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
					Debug.Log(region + " : " + halfPlane);

					//halfPlaneを可視化
					List<Vector3> vertices = halfPlane.GetVertices3Copy();
					vertices.Add(vertices[0]);	//末尾を追加
					ChainLine subl = lineFactory.CreateLine(vertices, subColor);

					//regionを可視化
					vertices = region.GetVertices3Copy();
					vertices.Add(vertices[0]);	//末尾を追加
					ChainLine l = lineFactory.CreateLine(vertices, lineColor.Evaluate((float)j / sites.Count));
					yield return StartCoroutine(Wait());
					lineFactory.DeleteLine(subl);
					lineFactory.DeleteLine(l);
				}
			}
			Debug.Log("Complete");

			yield return StartCoroutine(Wait());
		}
	}

	/// <summary>
	/// 何かするまで待機
	/// </summary>
	private IEnumerator Wait() {
		while(true) {
			yield return 0;
			if(Input.GetKeyUp(KeyCode.Space)) break;
		}
	}

	#endregion
}