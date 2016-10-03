using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Geometric;
using Seiro.Scripts.Graphics;

namespace Seiro.Scripts.Graphics.PolyLine2D.Snap {
	
	/// <summary>
	/// 放射状に対してのスナップ処理
	/// </summary>
	public class RadialSnap : BaseSnap {

		private Vector2 point;
		private int count;
		private float deltaAngle;
		private float[] angles;
		private float width = 0.1f;
		private float drawDistance = 100f;

		public RadialSnap(Vector2 point, int count, float snapForce) : base(snapForce) {
			this.point = point;
			this.count = count;

			this.angles = new float[count];
			this.deltaAngle = 360f / count;
			float deltaSum = 0f;
			for(int i = 0; i < count; ++i) {
				angles[i] = deltaSum;
				deltaSum += deltaAngle;
			}
		}

		public override bool Snap(Vector2 input, out Vector2 output) {
			float angle = GeomUtil.TwoPointAngle(point, input);
			//最も近い放射角度を求める
			float nearAngle = 0f;
			float halfDelta = deltaAngle * 0.5f;
			for(int i = 0; i < angles.Length; ++i) {
				if((angles[i] - halfDelta) < angle && angle < (angles[i] + halfDelta)) {
					nearAngle = angles[i];
					break;
				}
			}
			//線上の射影からスナップ座標を求める
			Vector2 p, q;
			p = GeomUtil.DegToVector2(nearAngle);
			q = input - point;
			float dot = GeomUtil.Dot(p, q);
			float projection = dot / p.magnitude;   //射影距離
			output = p.normalized * projection + point;

			//スナップ座標との距離を測る
			if((output - input).magnitude <= snapForce) {
				return true;
			} else {
				return false;
			}
		}

		public override EasyMesh GetEasyMesh(Color color) {
			Vector3[] vertices = new Vector3[count * 4];
			Color[] colors = new Color[vertices.Length];
			int[] indices = new int[count * 6];

			for(int i = 0; i < count; ++i) {
				Vector2 dir = GeomUtil.DegToVector2(angles[i]);
				Vector2 verDir = GeomUtil.RotateVector2(dir, 90f);
				dir *= drawDistance;
				verDir *= width * 0.5f;

				int index = i * 4;
				int indicesIndex = i * 6;

				//頂点
				vertices[index + 0] = dir + verDir + point;
				vertices[index + 1] = dir - verDir + point;
				vertices[index + 2] = -dir - verDir + point;
				vertices[index + 3] = -dir + verDir + point;

				//色
				colors[index + 0] = color;
				colors[index + 1] = color;
				colors[index + 2] = color;
				colors[index + 3] = color;

				//インデックス
				indices[indicesIndex + 0] = index + 0;
				indices[indicesIndex + 1] = index + 1;
				indices[indicesIndex + 2] = index + 2;
				indices[indicesIndex + 3] = index + 0;
				indices[indicesIndex + 4] = index + 2;
				indices[indicesIndex + 5] = index + 3;
			}

			return new EasyMesh(vertices, colors, indices);
		}
	}
}