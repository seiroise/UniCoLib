using UnityEngine;
using System;

namespace Seiro.Scripts.ObjectPool {

	/// <summary>
	/// MonoBehaviourのプール用アイテム
	/// </summary>
	public interface IMonoPoolItem<T> where T :  MonoBehaviour {

		/// <summary>
		/// 有効化
		/// </summary>
		void Activate();

		/// <summary>
		/// 自身を返す
		/// </summary>
		T GetThis();
	}
}