using UnityEngine;
using System;

namespace Seiro.Scripts.Graphics.PolyLine2D {

	/// <summary>
	/// 二次元ポリラインの編集コンポーネント
	/// </summary>
	public abstract class PolyLine2DEditorComponent : MonoBehaviour {

		protected PolyLine2DEditor editor;

		#region VirtualFunction

		/// <summary>
		/// 初期化
		/// </summary>
		public virtual void Initialize(PolyLine2DEditor editor) {
			this.editor = editor;
		}

		/// <summary>
		/// 有効化
		/// </summary>
		public virtual void Enable() {
			enabled = true;
		}

		/// <summary>
		/// 無効化
		/// </summary>
		public virtual void Disable() {
			enabled = false;
		}

		#endregion
	}
}