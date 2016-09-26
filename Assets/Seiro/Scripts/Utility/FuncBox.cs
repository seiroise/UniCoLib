using UnityEngine;
using System;

namespace Seiro.Scripts.Utility {

	/// <summary>
	/// 関数の箱
	/// </summary>
	public class FuncBox {

		/// <summary>
		/// 指定カメラのワールド座標に変換したマウス座標を取得する
		/// </summary>
		public static Vector3 GetMousePosition(Camera cam) {
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = cam.transform.position.z;
			return cam.ScreenToWorldPoint(mousePos);
		}

	}
}