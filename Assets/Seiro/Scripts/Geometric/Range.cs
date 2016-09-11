using System;

namespace Seiro.Scripts.Geometric {

	/// <summary>
	/// 範囲
	/// </summary>
	public class Range {

		public float min { get; set; }
		public float max { get; set; }

		#region Constructors

		public Range (float min, float max) {
			this.min = min;
			this.max = max;
		}

		#endregion
	}
}