using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Geometric.Polygon.Convex;

namespace Seiro.Scripts.Geometric.Polygon.Operation {

	/// <summary>
	/// 凸多角形の再分割
	/// 主に角度依存で閾値より角度が小さい(鋭角な)場合繰り返し分割を行う
	/// </summary>
	public class AngleSubdivisionOperation {

		#region Static Function

		/// <summary>
		/// 凸多角形の再分割処理
		/// </summary>
		public static ConvexPolygon Execute(ConvexPolygon polygon, float angleThreshold, float t  = 0.5f) {
			//入力がnullならnullを返す
			if(polygon == null) return null;

			//諸々のデータ構造
			List<Vector2> results = new List<Vector2>();	//計算結果
			List<Vector2> temp = new List<Vector2>();		//一時データ
			List<Vector2> vertices = polygon.GetVerticesCopy();

			//はじめに2点追加
			temp.Add(vertices[0]);
			temp.Add(vertices[1]);

			//巡回
			int size = vertices.Count;
			for(int i = 2; i <= size; ++i) {
				//順次処理
				temp.Add(vertices[i % size]);
				Process(temp, results, angleThreshold, t);	//処理
			}
			//0番目の処理
			temp.Add(results[0]);
			Process(temp, results, angleThreshold, t);	//処理

			return new ConvexPolygon(results);
		}

		/// <summary>
		/// 再分割処理
		/// 分割が起こる場合はtrueを返す
		/// </summary>
		private static void Process(List<Vector2> temp, List<Vector2> result, float angleThreshold, float t) {
			//一時データの数が3つ未満の場合は角度を測れないので終了
			while(temp.Count >= 3) {
				//先頭から3つを取り出す
				Vector2 p0 = temp[0];
				Vector2 p1 = temp[1];
				Vector2 p2 = temp[2];

				//角度を計測
				float angle = GeomUtil.TwoVectorAngle(p1, p0, p2);

				//角度が閾値よりも低いか
				if(angle < angleThreshold) {
					//分割する
					//中点を求める
					Vector2 c0 = Vector2.Lerp(p1, p0, 0.5f);
					Vector2 c1 = Vector2.Lerp(p1, p2, 0.5f);

					//新しいp1の座標を求める
					Vector2 cc0 = Vector2.Lerp(c0, c1, 0.5f);
					Vector2 newP1 = Vector2.Lerp(cc0, p1, t);

					//結果に追加。p0, c0, newp1, c1, p2 となるようにする
					temp.RemoveAt(1);
					temp.Insert(1, c1);
					temp.Insert(1, newP1);
					temp.Insert(1, c0);
				} else {
					//分割しない
					result.Add(p1);		//p1を結果に追加する
					temp.RemoveAt(0);	//先頭を削除
				}
			}
		}
		#endregion

	}
}