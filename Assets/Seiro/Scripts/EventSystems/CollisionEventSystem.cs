using UnityEngine;
using System.Collections.Generic;

namespace Seiro.Scripts.EventSystems {

	/// <summary>
	/// 画面上のあたり判定への各種入力イベントの通知
	/// </summary>
	public class CollisionEventSystem : MonoBehaviour {

		[SerializeField]
		private Camera camera;  //レイキャスト用カメラ

		private Collider prevCollider;

		//private Dictionary<Collider, > cash;

		#region UnityEvent

		private void Update() {
			Vector2 screenPos = Input.mousePosition;
			Highlight(screenPos);
		}

		#endregion

		#region Function

		/// <summary>
		/// 重なり判定
		/// </summary>
		private void Highlight(Vector2 screenPos) {
			Ray ray = camera.ScreenPointToRay(screenPos);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 100f)) {
				Collider hitCollider = hit.collider;
				//ヒットした場合
				if(prevCollider != hitCollider) {
					if(prevCollider == null) {
						//Enter
						EnterCollider(hitCollider);
					} else {
						//Exit & Enter
						ExitCollider(prevCollider);
						EnterCollider(hitCollider);
					}
				}
			} else {
				//ヒットしなかった場合
				if(prevCollider != null) {
					//Exit
					ExitCollider(prevCollider);
				}
			}
		}

		/// <summary>
		/// コライダー範囲に侵入
		/// </summary>
		private void EnterCollider(Collider col) {
			Debug.Log("Enter");
			//コンポーネントの取得
			ICollisionEventHandler handler = col.GetComponent<ICollisionEventHandler>();
			if(handler != null) {
				handler.OnPointerEnter();
			}
			prevCollider = col;
		}

		/// <summary>
		/// コライダー範囲から退出
		/// </summary>
		private void ExitCollider(Collider col) {
			Debug.Log("Exit");
			//コンポーネントの取得
			ICollisionEventHandler handler = col.GetComponent<ICollisionEventHandler>();
			if(handler != null) {
				handler.OnPointerExit();
			}
			prevCollider = null;
		}

		#endregion
	}
}