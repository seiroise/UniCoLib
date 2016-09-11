using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// トランスフォームの変更検知
/// </summary>
[RequireComponent(typeof(Transform))]
public class TransformDetector : MonoBehaviour {

	private Transform trans;

	//コールバック
	public Action<Transform> positionChangeCallback { get; set; }
	public Action<Transform> rotationChangeCallback { get; set; }
	public Action<Transform> scaleChangeCallback { get; set; }

	//前回計測値
	private Vector3 prevPosition;
	private Quaternion prevRotation;
	private Vector3 prevScale;

	#region UnityEvent

	private void Awake() {
		trans = GetComponent<Transform>();
	}

	private void Update() {
		CheckPosition();
		CheckRotation();
		CheckScale();
	}

	#endregion

	#region Function

	/// <summary>
	/// 座標の変更確認
	/// </summary>
	private void CheckPosition() {
		if(positionChangeCallback == null) return;
		if(prevPosition != trans.position) {
			positionChangeCallback(trans);
		}
		prevPosition = trans.position;
	}

	/// <summary>
	/// 回転の変更確認
	/// </summary>
	private void CheckRotation() {
		if(rotationChangeCallback == null) return;
		if(prevRotation != trans.rotation) {
			rotationChangeCallback(trans);
		}
		prevRotation = trans.rotation;
	}

	/// <summary>
	/// 大きさの変更確認
	/// </summary>
	private void CheckScale() {
		if(scaleChangeCallback == null) return;
		if(prevScale != trans.localScale) {
			scaleChangeCallback(trans);
		}
		prevScale = trans.localScale;
	}

	#endregion
}