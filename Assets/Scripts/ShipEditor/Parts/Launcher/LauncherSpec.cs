using System;

namespace Scripts.ShipEditor.Parts.Launcher {

	/// <summary>
	/// 砲台性能
	/// </summary>
	public class LauncherSpec {

		public float barrel;		//砲身長
		public float caliber;		//砲口径

		public float shotSpeed;		//射出速度
		public float reloadSpeed;	//装填速度

		public LauncherSpec(float barrel, float caliber) {
			this.barrel = barrel;
			this.caliber = caliber;

			this.shotSpeed = barrel;	//射出速度 = 砲身長
			this.reloadSpeed = caliber;	//装填速度 = 砲口径
		}
	}
}