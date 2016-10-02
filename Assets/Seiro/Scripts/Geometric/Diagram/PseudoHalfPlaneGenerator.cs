using UnityEngine;
using System;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;

namespace Seiro.Scripts.Geometric.Diagram {

	/// <summary>
	/// 擬似半平面の生成
	/// </summary>
	public class PseudoHalfPlaneGenerator {
		private Vector2 boundary1;          // 境界点1
		private Vector2 boundary2;          // 境界点2
		private Vector2 boundary3;          // 境界点3
		private LineSegment border1;        // 境界線1
		private LineSegment border2;        // 境界線2
		private LineSegment border3;        // 境界線3
		private double distanceThreshold;   // 交差の場合分けを正確に行うための閾値

		// boundaryの値を大きくすると計算誤差が無視できなくなるので注意！
		public PseudoHalfPlaneGenerator(float boundary) {
			boundary1 = new Vector2(0, boundary);
			boundary2 = new Vector2(-boundary, -boundary);
			boundary3 = new Vector2(boundary, -boundary);
			border1 = new LineSegment(boundary1, boundary2);
			border2 = new LineSegment(boundary2, boundary3);
			border3 = new LineSegment(boundary3, boundary1);
			distanceThreshold = boundary / 1000f;
		}

		/// <summary>
		/// 擬似半平面の作成
		/// </summary>
		public ConvexPolygon Execute(Line line, Vector2 exsample) {
			//lineと各境界線の交差を調べる
			Vector2 p1 = Vector2.zero;
			bool i1 = border1.GetIntersectionPoint(line, ref p1);
			Vector2 p2 = Vector2.zero;
			bool i2 = border2.GetIntersectionPoint(line, ref p2);
			Vector2 p3 = Vector2.zero;
			bool i3 = border3.GetIntersectionPoint(line, ref p3);

			List<Vector2> vertices = new List<Vector2>();
			//lineが境界線1及び2と交差する場合
			if(i1 && i2 && Vector2.Distance(p1, p2) >= distanceThreshold) {
				//lineが境界線1及び2と交差する場合
				if(GeomUtil.CCW(p1, boundary2, p2) * GeomUtil.CCW(p1, exsample, p2) > 0f) {
					//境界線2とexsampleがlineから見て同じ側にあるなら
					//境界点2を含む方の切断後頂点リストを生成
					AddVertices(vertices, p1, boundary2, p2);
				} else {
					//境界点2を含まない方の切断後頂点リストを生成
					AddVertices(vertices, p1, p2, boundary3, boundary1);
				}
			} else if(i2 && i3 && Vector2.Distance(p2, p3) >= distanceThreshold) {
				//lineが境界線2及び3と交差する場合
				if(GeomUtil.CCW(p2, boundary3, p3) * GeomUtil.CCW(p2, exsample, p3) > 0f) {
					AddVertices(vertices, p2, boundary3, p3);
				} else {
					AddVertices(vertices, p2, p3, boundary1, boundary2);
				}
			} else if(i3 && i1 && Vector2.Distance(p3, p1) >= distanceThreshold){
				//lineが境界線3及び1と交差する場合
				if(GeomUtil.CCW(p3, boundary1, p1) * GeomUtil.CCW(p3, exsample, p1) > 0f) {
					AddVertices(vertices, p3, boundary1, p1);
				} else {
					AddVertices(vertices, p3, p1, boundary2, boundary3);
				}
			} else {
				throw new ArgumentException();
			}

			//頂点リストから凸多角形を生成して返す
			return new ConvexPolygon(vertices);
		}

		/// <summary>
		/// listにverticesを追加する。重複を避ける
		/// </summary>
		private void AddVertices(List<Vector2> list, params Vector2[] vertices) {
			foreach(var v in vertices) {
				if(list.Count <= 0) {
					list.Add(v);
				} else {
					Vector2 first = list[0];
					Vector2 last = list[list.Count - 1];
					if (v != first && v != last) {
						list.Add(v);
					}
				}
			}
		}
	}
}