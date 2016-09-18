using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.Circle;
using Seiro.Scripts.Graphics;

public class Chap1_CircleUI_Demo : MonoBehaviour {

	private List<CircleFragment> fragments;

	public MeshFilter mf;
	public int num;
	public Gradient colors;

	#region UnityEvent

	private void Start() {
		fragments = new List<CircleFragment>();
		float deltaAngle = 360f / num;
		for(int i = 0; i < num; ++i) {
			CircleFragment fragment = new CircleFragment();
			float start = deltaAngle * i;
			fragment.SetRange(start, start + deltaAngle);
			fragment.SetRadius(4f, 4.1f);
			fragment.SetOptions(10f, 1f, colors.Evaluate((float)i / num));
			fragments.Add(fragment);
		}

		for(int i = 0; i < fragments.Count; ++i) {
			fragments[i].SetIndicate(CircleFragment.Indicate.Visible, CircleFragment.RangeIndicate.StartToEnd, CircleFragment.RadiusIndicate.Fixed);
		}
	}

	private void Update() {
		//入力
		if(Input.GetMouseButtonUp(0)) {
			for(int i = 0; i < fragments.Count; ++i) {
				fragments[i].SetIndicate(CircleFragment.Indicate.Hide, CircleFragment.RangeIndicate.StartToEnd, CircleFragment.RadiusIndicate.Fixed);
			}
		}
		if(Input.GetMouseButtonDown(0)) {
			for(int i = 0; i < fragments.Count; ++i) {
				fragments[i].SetIndicate(CircleFragment.Indicate.Visible, CircleFragment.RangeIndicate.StartToEnd, CircleFragment.RadiusIndicate.Fixed);
			}
		}
		/*
		if(fragments != null) {
			EasyMesh[] eMeshs = new EasyMesh[fragments.Count];
			for(int i = 0; i < fragments.Count; ++i) {
				eMeshs[i] = fragments[i].Update();
			}
			mf.mesh = EasyMesh.ToMesh(eMeshs);
		}
		*/
	}

	#endregion
}