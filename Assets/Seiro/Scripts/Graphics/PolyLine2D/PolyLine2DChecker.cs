using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Geometric;

namespace Seiro.Scripts.Graphics.PolyLine2D {

	/// <summary>
	/// 二次元ポリラインの確認
	/// </summary>
	public class PolyLine2DChecker : PolyLine2DEditorComponent {

		[Header("Parameter")]
		public bool doublePointRemoval = false;     //連続同一頂点の除去
		public float doublePointThreshold = 0.05f;  //連続同一点の認識閾値
		public bool crossLineRemoval = false;       //交差線分の除去

		/// <summary>
		/// 頂点追加の例外確認。trueなら追加可能
		/// </summary>
		public bool AddCheck(List<Vector2> vertices, Vector2 point) {
			int count = vertices.Count;

			//同一点の検出
			if(doublePointRemoval) {
				if(count > 1) {
					Vector2 prevPoint = vertices[count - 1];
					float dis = (point - prevPoint).magnitude;
					if(dis < doublePointThreshold) {
						return false;
					}
				}
			}

			//線分の交差判定
			if(crossLineRemoval) {
				if(count > 2) {
					//検出線分の作成
					LineSegment line = new LineSegment(vertices[count - 1], point);
					for(int i = 0; i < count - 2; ++i) {
						//比較線分の作成
						LineSegment sample = new LineSegment(vertices[i], vertices[i + 1]);
						if(line.Intersects(sample)) {
							return false;
						}
					}
				}
			}

			return true;
		}
	}
}