using UnityEngine;
using System;

namespace Scripts.ShipEditor.Parts.Launcher {

	/// <summary>
	/// 砲台
	/// </summary>
	public class Launcher : SpecialPoint {

		public LauncherSpec spec;	//性能

		public string shotObjectID;	//射出体ID

		public Launcher(Vector2 point, float angle, float barrel, float caliber) : base(point, angle) {
			this.spec = new LauncherSpec(angle, barrel);
		}
	}
}