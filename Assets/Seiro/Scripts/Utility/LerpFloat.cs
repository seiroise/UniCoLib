using UnityEngine;
using System;

namespace Seiro.Scripts.Utility {

	/// <summary>
	/// floatの補完処理
	/// </summary>
	public class LerpFloat {

		private float value;
		public float Value { get { return value; } }

		private float target;

		private bool processing;
		public bool Processing { get { return processing; } }

		#region Constructor

		public LerpFloat() {
			this.value = 0f;
			this.target = 0f;
			this.processing = false;
		}

		public LerpFloat(float value, float target) {
			SetValues(value, target);
		}

		#endregion

		#region Function

		/// <summary>
		/// 現在地と目標値の設定
		/// </summary>
		public void SetValues(float value, float target) {
			this.value = value;
			this.target = target;
			this.processing = true;
		}

		/// <summary>
		/// 目標値の設定
		/// </summary>
		public void SetTarget(float target) {
			this.target = target;
			this.processing = true;
		}

		/// <summary>
		/// 更新。処理中はtrueを返す
		/// </summary>
		/// <param name="t">T.</param>
		/// <param name="epsilon">Epsilon.</param>
		public bool Update(float t, float epsilon = 0.01f) {
			if(!processing) return false;
			value = Mathf.Lerp(value, target, t);
			if(Mathf.Abs(target - value) < epsilon) {
				value = target;
				processing = false;
			}
			return true;
		}

		#endregion
	}
}