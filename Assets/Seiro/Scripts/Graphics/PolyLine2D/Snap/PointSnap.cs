using UnityEngine;
using System.Collections;
using System;
using Seiro.Scripts.Graphics;

namespace Seiro.Scripts.Graphics.PolyLine2D.Snap {

	/// <summary>
	/// 点に対してのスナップ
	/// </summary>
	public class PointSnap : BaseSnap {

		private Vector2 point;

		public PointSnap(Vector2 point, float forceSnap) : base(forceSnap) {
			this.point = point;
		}

		public override bool Snap(Vector2 input, out Vector2 output) {
			output = point;
			float distance = (input - point).magnitude;
			if(distance <= snapForce) {
				return true;
			} else {
				return false;
			}
		}

		public override EasyMesh GetEasyMesh(Color color) {
			return null;
		}
	}
}