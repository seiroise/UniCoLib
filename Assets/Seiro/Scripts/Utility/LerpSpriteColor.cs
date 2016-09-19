using UnityEngine;
using System;

namespace Seiro.Scripts.Utility {

	/// <summary>
	/// スプライトのColorの線形補間
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	public class LerpSpriteColor : MonoBehaviour {

		private SpriteRenderer target;
		public SpriteRenderer Target { get { return target; } }

		private LerpColor lerpColor;

		[SerializeField]
		private float t = 10f;

		#region UnityEvent

		private void Awake() {
			target = GetComponent<SpriteRenderer>();
			lerpColor = new LerpColor(target.color, target.color);
		}

		private void Update() {
			if(!lerpColor.Processing) return;
			lerpColor.Update(t * Time.deltaTime);
			target.color = lerpColor.Value;
		}

		#endregion

		#region Function

		/// <summary>
		/// 透過度の現在地と目標値を設定
		/// </summary>
		public void SetAlphas(float value, float target) {
			Color vc = lerpColor.Value;
			vc.a = value;
			Color tc = new Color(vc.r, vc.g, vc.b, target);
			lerpColor.SetValues(vc, tc);
		}

		/// <summary>
		/// 透過度の目標値を設定
		/// </summary>
		public void SetAlphaTarget(float to) {
			Color c = lerpColor.Value;
			c.a = to;
			lerpColor.SetTarget(c);
		}

		#endregion
	}
}