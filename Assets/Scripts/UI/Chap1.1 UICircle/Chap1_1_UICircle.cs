using UnityEngine;
using System;

public class Chap1_1_UICircle : MonoBehaviour {

	public UICircle circle;

	#region UnityEvent

	private void Start() {
		circle.AddClickCallback("Exit.OK", OnCLickedExitOK);
	}

	private void Update() {
		if(Input.GetMouseButtonDown(1)) {
			if(circle.Visibled) {
				circle.Hide();
			} else {
				circle.Visible();
			}
		}
	}

	#endregion

	#region Callback

	private void OnCLickedExitOK(GameObject gObj) {
		Debug.Log("Exit OK!");
	}

	#endregion
}