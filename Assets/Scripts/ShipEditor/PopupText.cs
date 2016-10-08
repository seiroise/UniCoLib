using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 何かしらを強調表示してくれるテキスト
/// </summary>
public class PopupText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public RectTransform targetTrans;

	public Vector2 start;
	public Vector2 target;

	public float lerpT = 10f;

	private Vector3 nowTarget;
	private bool lerped = false;

	#region UnityEvent

	private void Start() {
		SetTarget(start);
	}

	private void Update() {
		UpdateSize();
	}

	#endregion

	#region Function

	/// <summary>
	/// 大きさの更新
	/// </summary>
	private void UpdateSize() {
		if(!lerped) return;
		Vector2 now = targetTrans.sizeDelta;
		Vector2 lerpSize = Vector2.Lerp(now, nowTarget, lerpT * Time.deltaTime);
		float delta = (lerpSize - now).magnitude;
		if(delta < 0.01f) {
			lerpSize = nowTarget;
			lerped = false;
		}
		targetTrans.sizeDelta = lerpSize;
	}

	private void SetTarget(Vector2 targetSize) {
		nowTarget = targetSize;
		lerped = true;
	}

	#endregion

	#region IPointerEventHandler

	public void OnPointerEnter(PointerEventData eventData) {
		SetTarget(target);
	}

	public void OnPointerExit(PointerEventData eventData) {
		SetTarget(start);
	}

	#endregion
}