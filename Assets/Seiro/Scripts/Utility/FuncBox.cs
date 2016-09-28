using UnityEngine;

namespace Seiro.Scripts.Utility {

	/// <summary>
	/// 関数の箱
	/// </summary>
	public class FuncBox {

		#region Camera

		/// <summary>
		/// 指定カメラのワールド座標に変換したマウス座標を取得する
		/// </summary>
		public static Vector3 GetMousePosition(Camera cam) {
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = cam.transform.position.z;
			return cam.ScreenToWorldPoint(mousePos);
		}

		#endregion

		#region Random

		/// <summary>
		/// ランダムな二次元ベクトルを取得する
		/// </summary>
		public static Vector2 RandomVector2(float min = -1f, float max = 1f) {
			return new Vector2(Random.Range(min, max), Random.Range(min, max));
		}

		#endregion

	}
}