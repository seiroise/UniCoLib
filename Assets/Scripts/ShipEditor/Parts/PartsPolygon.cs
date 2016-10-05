using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Concave;


/// <summary>
/// ゲーム内のパーツ用ポリゴン
/// </summary>
public class PartsPolygon {

	private ConcavePolygon polygon;

	public PartsPolygon(ConcavePolygon polygon) {
		this.polygon = polygon;
	}

	/// <summary>
	/// ランチャーの解析
	/// </summary>
	public List<Vector2> ParseLauncher(float tolerance = 1f) {
		List<PolygonVertex> vertices = polygon.GetPolygonVertices();
		int size = vertices.Count;
		List<Vector2> launchers = new List<Vector2>();
		for(int i = 0; i < size; ++i) {
			PolygonVertex p0 = vertices[i];
			PolygonVertex p1 = vertices[(i + 1) % size];
			float sumAngle = p0.angle + p1.angle;

			if(180f - tolerance < sumAngle && sumAngle < 180f + tolerance) {
				launchers.Add((p1.point - p0.point) * 0.5f + p0.point);
			}
		}
		return launchers;
	}
}