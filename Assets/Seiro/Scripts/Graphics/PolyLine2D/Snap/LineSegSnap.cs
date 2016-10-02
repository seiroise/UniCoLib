using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Geometric;
using Seiro.Scripts.Graphics;

namespace Seiro.Scripts.Graphics.PolyLine2D.Snap {
	
	/// <summary>
	/// 線分に対してのスナップ処理
	/// </summary>
	public class LineSegSnap : BaseSnap {

		private Vector2 a;
		private Vector2 b;

		public LineSegSnap(Vector2 a, Vector2 b, float snapForce) : base(snapForce) {
			this.a = a;
			this.b = b;
		}

		public override bool Snap(Vector2 input, out Vector2 output) {

			//スナップ座標を求める
			Vector2 p, q;
			Vector2 snapPoint;

			//a側
			p = b - a;
			q = input - a;
			float aDot = GeomUtil.Dot(p, q);
			if(aDot <= 0) {
				snapPoint = a;
			} else {
				//b側
				p = a - b;
				q = input - b;
				float bDot = GeomUtil.Dot(p, q);
				if(bDot <= 0) {
					snapPoint = b;
				} else {
					//垂線の直交座標
					float projection = bDot / p.magnitude;  //射影距離
					snapPoint = p.normalized * projection + b;
				}
			}

			output = snapPoint;
			//スナップ座標との距離を測る
			if((snapPoint - input).magnitude <= snapForce) {
				return true;
			} else {
				return false;
			}
		}

		public override EasyMesh GetEasyMesh(Color color) {
			List<Vector2> list = new List<Vector2>();
			list.Add(a);
			list.Add(b);
			return EasyMesh.MakePolyLine2D(list, 0.1f, color);
		}
	}
}