using UnityEngine;
using System;

namespace Seiro.Scripts.Utility {

	/// <summary>
	/// Colorの補完処理
	/// </summary>
	public class LerpColor {

		private Color value;
		public Color Value { get { return value; } }

		private Color target;

		private bool processing;
		public bool Processing { get { return processing; } }

		#region Constructor

		public LerpColor() {
			this.value = Color.white;
			this.target = Color.white;
			this.processing = false;
		}

		public LerpColor(Color value, Color target) {
			SetValues(value, target);
		}

		#endregion

		#region Function

		/// <summary>
		/// 現在地と目標値の設定
		/// </summary>
		public void SetValues(Color value, Color target) {
			this.value = value;
			this.target = target;
			this.processing = true;
		}

		/// <summary>
		/// 目標値の設定
		/// </summary>
		public void SetTarget(Color target) {
			this.target = target;
			this.processing = true;
		}

		/// <summary>
		/// 更新。処理中はtrueを返す
		/// </summary>
		public bool Update(float t, float epsilon = 0.01f) {
			if(!processing) return false;
			value = Color.Lerp(value, target, t);
			//差を求める
			Color delta = target - value;
			float sum = 0f;
			for(int i = 0; i < 4; ++i) {
				sum += Mathf.Abs(delta[i]);
			}
			if(sum < epsilon) {
				value = target;
				processing = false;
				return true;
			}
			return true;
		}

		#endregion
	}
}