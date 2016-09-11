using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// ドラッグ可能なゲームオブジェクト
/// </summary>
public class DraggableObject : MonoBehaviour {

	private Action<Transform> moveCallback;    //移動時に呼ぶコールバック
	public Action<Transform> MoveCallback {
		set { moveCallback = value; }
		get { return moveCallback; }
	}

	private void OnMouseDrag() {
		Vector3 pos = Input.mousePosition;
		pos.z = -Camera.main.transform.position.z;
		transform.position = Camera.main.ScreenToWorldPoint(pos);

		if(moveCallback != null) moveCallback(transform);
	}
}