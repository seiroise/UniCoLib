using System;

namespace Seiro.Scripts.FSM {
	public interface IFiniteStateMachine<T> {

		/// <summary>
		/// 初期化
		/// </summary>
		void Initialize();

		/// <summary>
		/// 状態の有効化
		/// </summary>
		void Activate(IFiniteState<T> state);

		/// <summary>
		/// 状態の無効化
		/// </summary>
		void Disactivate(IFiniteState<T> state);
	}
}