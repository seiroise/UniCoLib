using UnityEngine;
using System;

namespace Scripts.ShipEditor.Parts {

	/// <summary>
	/// ポリゴン内の特殊点
	/// </summary>
	public abstract class SpecialPoint {

		public Vector2 point;
		public float angle;
		public string tag;
		public bool used;

		public SpecialPoint(Vector2 point, float angle) {
			this.point = point;
			this.angle = angle;
			this.tag = "Untaged";
			this.used = false;
		}
	}
}