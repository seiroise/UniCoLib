using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Concave;
using Scripts.ShipEditor.Parts.Launcher;
using Seiro.Scripts.Geometric;

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
	public List<Launcher> ParseLauncher(float tolerance = 1f) {
		List<PolygonVertex> vertices = polygon.GetPolygonVertices();
		int size = vertices.Count;
		List<Launcher> launchers = new List<Launcher>();
		for(int i = 0; i < size; ++i) {
			PolygonVertex p1 = vertices[(i + 1) % size];
			PolygonVertex p2 = vertices[(i + 2) % size];
			float sumAngle = p1.angle + p2.angle;

			if(180f - tolerance < sumAngle && sumAngle < 180f + tolerance) {
				PolygonVertex p0 = vertices[i];
				PolygonVertex p3 = vertices[(i + 3) % size];
				//座標
				Vector2 point = (p2.point - p1.point) * 0.5f + p1.point;
				//角度
				float angle = GeomUtil.TwoPointAngle(p0.point, p1.point);
				//砲身長
				float barrel0 = (p1.point - p0.point).magnitude;
				float barrel1 = (p3.point - p2.point).magnitude;
				float barrel = (barrel0 + barrel1) * 0.5f;
				//口径
				Line l1 = Line.FromPoints(p0.point, p1.point);
				Line l2 = Line.FromPoints(p2.point, GeomUtil.RotateVector2(p3.point - p2.point, 90f) + p2.point);	//l1の垂直線
				Vector2 intersection = Vector2.zero;
				l1.GetIntersectionPoint(l2, ref intersection);
				float caliber = (intersection - p2.point).magnitude;

				launchers.Add(new Launcher(point, angle, barrel, caliber));
			}
		}
		return launchers;
	}
}