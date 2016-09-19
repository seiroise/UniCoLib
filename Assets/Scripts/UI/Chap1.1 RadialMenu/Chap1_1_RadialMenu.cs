using UnityEngine;
using System;

public class Chap1_1_RadialMenu : MonoBehaviour {

	public RadialMenu radialMenu;

	#region UnityEvent

	private void Start() {
		radialMenu.AddClickCallback("Exit.OK", OnCLickedExitOK);
	}

	private void Update() {
		if(Input.GetMouseButtonDown(1)) {
			if(radialMenu.Visibled) {
				radialMenu.Hide();
			} else {
				radialMenu.Visible();
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