using System;
namespace Seiro.Scripts.EventSystems {
	public interface ICollisionEventHandler {

		/// <summary>
		/// 範囲への侵入
		/// </summary>
		void OnPointerEnter();

		/// <summary>
		/// 範囲からの退出
		/// </summary>
		void OnPointerExit();
	}
}