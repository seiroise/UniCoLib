using System;

namespace Seiro.Scripts.FSM {

	/// <summary>
	/// 有限ステート
	/// </summary>
	public interface IFiniteState<T> {

		/// <summary>
		/// 初期化
		/// </summary>
		void Initialize(IFiniteStateMachine<T> stateMachine);

		/// <summary>
		/// 開始
		/// </summary>
		void Enter();

		/// <summary>
		/// 終了
		/// </summary>
		void Exit();
	}
}