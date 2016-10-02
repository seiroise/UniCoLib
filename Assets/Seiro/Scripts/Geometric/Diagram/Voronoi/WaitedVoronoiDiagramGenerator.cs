using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;
using Seiro.Scripts.Geometric.Polygon.Operation;

namespace Seiro.Scripts.Geometric.Diagram.Voronoi {

	/// <summary>
	/// 重み付きボロノイ図の生成
	/// </summary>
	public class WaitedVoronoiDiagramGenerator {

		private PseudoHalfPlaneGenerator halfPlaneGenerator;

		#region Constructors

		public WaitedVoronoiDiagramGenerator() {
			halfPlaneGenerator = new PseudoHalfPlaneGenerator(100f);
		}

		public WaitedVoronoiDiagramGenerator(float halfPlaneSize) {
			halfPlaneGenerator = new PseudoHalfPlaneGenerator(halfPlaneSize);
		}

		#endregion

		#region Function

		/// <summary>
		/// 重み付きボロノイ図の生成
		/// </summary>
		public List<ConvexPolygon> Execute(ConvexPolygon area, List<WaitedVoronoiSite> sites) {
			List<ConvexPolygon> result = new List<ConvexPolygon>();
			foreach(var s1 in sites) {
				ConvexPolygon region = null;    //途中計算結果格納用の領域
				foreach(var s2 in sites) {
					if(s1 == s2) {
						continue;
					}
					//s1とs2の垂直線を重み付きで求める
					Line line = Line.PerpendicularWaitLine(s1.Point, s1.Wait, s2.Point, s2.Wait);
					//垂直二等分線による半平面のうち，s1を含む方を求める
					ConvexPolygon halfPlane = halfPlaneGenerator.Execute(line, s1.Point);
					if(region == null) {
						//初回計算時
						region = IntersectionOperation.Execute(area, halfPlane);
					} else {
						//二回目以降
						region = IntersectionOperation.Execute(region, halfPlane);
					}
				}
				//最終的な計算結果をボロノイ領域とする
				result.Add(region);
			}
			return result;
		}

		#endregion
	}
}