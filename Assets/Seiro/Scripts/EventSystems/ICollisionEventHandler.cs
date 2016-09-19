using UnityEngine;
using System;

namespace Seiro.Scripts.EventSystems {

	/// <summary>
	/// 当たり判定イベント受け取りインタフェース
	/// </summary>
	public interface ICollisionEventHandler {

		/// <summary>
		/// 範囲への侵入
		/// </summary>
		void OnPointerEnter(RaycastHit hit);

		/// <summary>
		/// 範囲からの退出
		/// </summary>
		void OnPointerExit(RaycastHit hit);

		/// <summary>
		/// 範囲内でのボタンの押下
		/// </summary>
		void OnPointerDown(RaycastHit hit);

		/// <summary>
		/// 範囲内でのボタンの押上
		/// </summary>
		void OnPointerUp(RaycastHit hit);

		/// <summary>
		/// 範囲内でのクリック
		/// </summary>
		void OnPointerClick(RaycastHit hit);
	}
}