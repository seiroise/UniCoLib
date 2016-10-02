using UnityEngine;

namespace Seiro.Scripts.Graphics.PolyLine2D.Snap {

	/// <summary>
	/// ある座標に対してスナップ処理を行う基底クラス
	/// </summary>
	public abstract class BaseSnap {

		protected float snapForce;

		public BaseSnap(float snapForce) {
			this.snapForce = snapForce;
		}

		public abstract bool Snap(Vector2 input, out Vector2 output);

		public abstract EasyMesh GetEasyMesh(Color color);
	}
}