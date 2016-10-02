using UnityEngine;
using System;
using System.Collections.Generic;
using Seiro.Scripts.Graphics;
using Seiro.Scripts.Graphics.PolyLine2D.Snap;

/// <summary>
/// 補助線の提示、推測
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AdditionalLineProponent : MonoBehaviour {

	/// <summary>
	/// 優先度付きスナップ
	/// </summary>
	private class PrecedenceSnap {
		public int precedence;
		public BaseSnap snap;

		public PrecedenceSnap(int precedence, BaseSnap snap) {
			this.precedence = precedence;
			this.snap = snap;
		}
	}

	public Color color = Color.white;

	private MeshFilter mf;
	private List<PrecedenceSnap> snaps;
	private Dictionary<PrecedenceSnap, Action> callbackDic;	//スナップ時コールバック
	private PrecedenceSnap prevSnap;

	#region UnityEvent

	private void Awake() {
		mf = GetComponent<MeshFilter>();
		snaps = new List<PrecedenceSnap>();
		callbackDic = new Dictionary<PrecedenceSnap, Action>();
	}

	#endregion

	#region Function

	/// <summary>
	/// スナップの追加
	/// </summary>
	public void AddSnap(int precedence, BaseSnap snap, Action callback = null) {
		PrecedenceSnap pre = new PrecedenceSnap(precedence, snap);
		snaps.Add(pre);
		if(callback != null) callbackDic.Add(pre, callback);
	}

	/// <summary>
	/// 補助線の描画
	/// </summary>
	public void Draw() {
		//描画
		EasyMesh[] eMeshes = new EasyMesh[snaps.Count];
		for(int i = 0; i < snaps.Count; ++i) {
			eMeshes[i] = snaps[i].snap.GetEasyMesh(color);
		}
		mf.mesh = EasyMesh.ToMesh(eMeshes);
	}

	/// <summary>
	/// 補助線の消去
	/// </summary>
	public void Clear() {
		snaps.Clear();
		mf.mesh = null;
	}

	/// <summary>
	/// スナップ
	/// </summary>
	public bool Snap(Vector2 point, out Vector2 result) {
		result = Vector2.zero;

		if(snaps.Count == 0) return false;
		float minDistance = float.MaxValue;
		Vector2 sample;
		PrecedenceSnap snap = null;
		bool snaped = false;

		for(int i = 0; i < snaps.Count; ++i) {
			PrecedenceSnap e = snaps[i];
			if(e.snap.Snap(point, out sample)) {
				if(snap == null) {
					result = sample;
					snap = e;
					minDistance = (sample - point).magnitude;
					snaped = true;
				} else if(e.precedence > snap.precedence) {
					result = sample;
					snap = e;
					minDistance = (sample - point).magnitude;
					snaped = true;
				} else if(snap.precedence == e.precedence) {
					//距離を測る
					float distance = (sample - point).magnitude;
					if(minDistance > distance) {
						result = sample;
						snap = e;
						minDistance = distance;
						snaped = true;
					}
				}
			}
		}
		prevSnap = snap;
		return snaped;
	}

	/// <summary>
	/// コールバック呼び出し
	/// </summary>
	public void SnapCallback() {
		if(prevSnap!= null) {
			if(callbackDic.ContainsKey(prevSnap)) {
				callbackDic[prevSnap]();
			}
		}
	}

	#endregion
}