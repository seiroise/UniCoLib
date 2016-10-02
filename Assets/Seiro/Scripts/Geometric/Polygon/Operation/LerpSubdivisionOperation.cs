using System;
using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;

namespace Seiro.Scripts.Geometric.Polygon.Operation {

	/// <summary>
	/// 凸多角形の再分割
	/// 基本的に頂点を内側に一定値近づけていき分割を行う
	/// </summary>
	public class LerpSubdivisionOperation {

		public static ConvexPolygon Execute(ConvexPolygon polygon, int num, float t = 0.5f) {
			//入力がnullならnullを返す
			if(polygon == null) return null;

			//諸々のデータ構造
			List<Vector2> vertices = polygon.GetVerticesCopy();

			//回数分だけ実行
			for(int i = 0; i < num; ++i) {
				vertices = Process(vertices, t);
			}

			return new ConvexPolygon(vertices);
		}

		/// <summary>
		/// 再分割処理
		/// </summary>
		private static List<Vector2> Process(List<Vector2> vertices, float t) {

			List<Vector2> result = new List<Vector2>();
			result.Add(Vector2.Lerp(vertices[0], vertices[1], 0.5f));

			Vector2 p0, p1, p2, c;

			int size = vertices.Count;
			for(int i = 0; i < size - 1; ++i) {
				p0 = result[i * 2];
				p1 = vertices[i + 1];   //中心
				p2 = Vector2.Lerp(p1, vertices[(i + 2) % size], 0.5f);

				//p0とp2の中点を求める
				c = Vector2.Lerp(p0, p2, 0.5f);

				//p1をcに向かってtだけずらして登録
				result.Add(Vector2.Lerp(p1, c, t));

				//次のループのためにp2を登録
				result.Add(p2);
			}
			//最後の一つ(というか最初の一つ?)
			p0 = result[result.Count - 1];
			p1 = vertices[0];   //中心
			p2 = result[0];

			//p0とp2の中点を求める
			c = Vector2.Lerp(p0, p2, 0.5f);

			//p1をcに向かってtだけずらして登録
			result.Add(Vector2.Lerp(p1, c, t));

			return result;
		}
	}
}