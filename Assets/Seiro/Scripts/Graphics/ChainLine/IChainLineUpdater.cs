using UnityEngine;
using System.Collections.Generic;

namespace Seiro.Scripts.Graphics.ChainLine {

	/// <summary>
	/// ChainLineの更新用インタフェース
	/// </summary>
	public interface IChainLineUpdater {

		/// <summary>
		/// 初期化時に一度だけ呼ばれる
		/// </summary>
		void Init(ChainLine owner);

		/// <summary>
		/// 更新時にChainLineから呼ばれる
		/// </summary>
		void Update(List<Vertex> vertices);
	}
}