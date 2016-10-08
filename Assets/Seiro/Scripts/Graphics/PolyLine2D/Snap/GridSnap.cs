using UnityEngine;
using System;

namespace Seiro.Scripts.Graphics.PolyLine2D.Snap {

	/// <summary>
	/// グリッド状に対してのスナップ処理
	/// </summary>
	public class GridSnap : BaseSnap {

		private Vector2 center;
		private float gridInterval; 		//グリッド間隔
		private int majorGridInterval;		//メジャーグリッド間隔
		private float drawDistance = 100f;	//描画距離
		private float drawWidth = 0.05f;			//幅

		public GridSnap(float gridInterval, int majorGridInterval, float snapForce) : base(snapForce) {
			this.center = Vector2.zero;
			this.gridInterval = gridInterval;
			this.majorGridInterval = majorGridInterval;
		}

		public override bool Snap(Vector2 input, out Vector2 output) {
			//スナップ座標を求める
			int gx = Mathf.RoundToInt(input.x / gridInterval);
			int gy = Mathf.RoundToInt(input.y / gridInterval);
			Vector2 snapTarget = new Vector2(gx * gridInterval, gy * gridInterval);
			output = snapTarget;
			//距離を計算
			float distance = (snapTarget - input).magnitude;
			if(distance <= snapForce) {
				return true;
			} else {
				return false;
			}
		}

		public override EasyMesh GetEasyMesh(Color color) {

			float halfDrawDistance = drawDistance * 0.5f;
			float halfWidth = drawWidth * 0.5f;
			int halfLineCount = (int)(halfDrawDistance / gridInterval);
			Color subColor = color * 0.5f;


			//下準備
			int totalLineCount = (halfLineCount + 1) * 4;	//めんどくさいのでど真ん中は二重に描画するので+1しとく
			Vector3[] vertices = new Vector3[totalLineCount * 4];
			Color[] colors = new Color[vertices.Length];
			int[] indices = new int[totalLineCount * 6];

			for(int i = 0; i < halfLineCount; ++i) {
				
				float t = i * gridInterval;

				int index = i * 16;
				int indicesIndex = i * 24;

				//頂点
				vertices[index + 0] = new Vector2(-halfDrawDistance, t + halfWidth) + center;
				vertices[index + 1] = new Vector2(-halfDrawDistance, t - halfWidth) + center;
				vertices[index + 2] = new Vector2(halfDrawDistance, t - halfWidth) + center;
				vertices[index + 3] = new Vector2(halfDrawDistance, t + halfWidth) + center;

				vertices[index + 4] = new Vector2(-halfDrawDistance, -t + halfWidth) + center;
				vertices[index + 5] = new Vector2(-halfDrawDistance, -t - halfWidth) + center;
				vertices[index + 6] = new Vector2(halfDrawDistance, -t - halfWidth) + center;
				vertices[index + 7] = new Vector2(halfDrawDistance, -t + halfWidth) + center;

				vertices[index + 8] = new Vector2(t - halfWidth, halfDrawDistance) + center;
				vertices[index + 9] = new Vector2(t + halfWidth, halfDrawDistance) + center;
				vertices[index + 10] = new Vector2(t + halfWidth, -halfDrawDistance) + center;
				vertices[index + 11] = new Vector2(t - halfWidth, -halfDrawDistance) + center;

				vertices[index + 12] = new Vector2(-t - halfWidth, halfDrawDistance) + center;
				vertices[index + 13] = new Vector2(-t + halfWidth, halfDrawDistance) + center;
				vertices[index + 14] = new Vector2(-t + halfWidth, -halfDrawDistance) + center;
				vertices[index + 15] = new Vector2(-t - halfWidth, -halfDrawDistance) + center;

				//色
				Color drawColor = i % majorGridInterval == 0 ? color : subColor;
				for(int j = 0; j < 16; ++j) {
					colors[index + j] = drawColor;
				}

				//インデックス
				indices[indicesIndex + 0] = index + 0;
				indices[indicesIndex + 1] = index + 1;
				indices[indicesIndex + 2] = index + 2;
				indices[indicesIndex + 3] = index + 0;
				indices[indicesIndex + 4] = index + 2;
				indices[indicesIndex + 5] = index + 3;

				indices[indicesIndex + 6] = index + 4;
				indices[indicesIndex + 7] = index + 5;
				indices[indicesIndex + 8] = index + 6;
				indices[indicesIndex + 9] = index + 4;
				indices[indicesIndex + 10] = index + 6;
				indices[indicesIndex + 11] = index + 7;

				indices[indicesIndex + 12] = index + 8;
				indices[indicesIndex + 13] = index + 9;
				indices[indicesIndex + 14] = index + 10;
				indices[indicesIndex + 15] = index + 8;
				indices[indicesIndex + 16] = index + 10;
				indices[indicesIndex + 17] = index + 11;

				indices[indicesIndex + 18] = index + 12;
				indices[indicesIndex + 19] = index + 13;
				indices[indicesIndex + 20] = index + 14;
				indices[indicesIndex + 21] = index + 12;
				indices[indicesIndex + 22] = index + 14;
				indices[indicesIndex + 23] = index + 15;
			}

			return new EasyMesh(vertices, colors, indices);
		}
	}
}