using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// RadialMenu
/// </summary>
public class RadialMenu : MonoBehaviour {

	public float innerRadius = 1f;
	public float radiusRange = 0.5f;
	public float sectorInterval = 0.1f;
	public float trackInterval = 1f;

	[Header("Range")]
	public float startAngle = 0f;
	public float endAngle = 360f;

	//スタック関連
	private Stack<Transform> stack;
	public bool Visibled { get { return stack.Count > 0; } }

	//コールバック
	private Dictionary<string, Action<GameObject>> clickCallbackDic;

	#region UnityEvent

	private void Awake() {
		stack = new Stack<Transform>();
		clickCallbackDic = new Dictionary<string, Action<GameObject>>();
	}

	#endregion

	#region Function

	/// <summary>
	/// 指定したtransform配下(1階層分)のTを取得する
	/// </summary>
	private List<T> GetComponentsInChildren<T>(Transform trans) where T : Component {
		//例外処理
		if(trans == null) return null;
		if(trans.childCount <= 0) return null;

		List<T> list = new List<T>();
		foreach(Transform e in trans) {
			T t = e.GetComponent<T>();
			if(t != null) list.Add(t);
		}
		if(list.Count <= 0) {
			return null;
		} else {
			return list;
		}
	}

	/// <summary>
	/// 自身からの子の距離を取得する
	/// </summary>
	private int GetChildDistance(Transform child) {
		int distance = 0;
		while(child != transform) {
			child = child.parent;
			if(child == null) return -1;    //ルートに到達
			distance++;
		}
		return distance;
	}

	/// <summary>
	/// 断片群を表示する
	/// </summary>
	private void VisibleFragment(List<UICircleFragment> frags, int depth) {

		//表示用パラメータを求める
		float deltaAngle = (endAngle - startAngle) / frags.Count;
		float halfInterval = trackInterval * 0.5f;
		float startOffset = deltaAngle > 0f ? halfInterval : -halfInterval;
		float endOffset = -startOffset;
		float overOuter = sectorInterval / radiusRange;
		float clickOuter = overOuter * 0.5f;

		//表示
		for(int i = 0; i < frags.Count; ++i) {
			frags[i].SetManager(this);
			float start = deltaAngle * i + startAngle + startOffset;
			float end = start + deltaAngle + endOffset;
			float inner = innerRadius + (radiusRange + sectorInterval) * depth;
			float outer = inner + radiusRange;
			frags[i].gameObject.SetActive(true);
			frags[i].Visible(start, end, inner, outer);
			frags[i].SetOuterScale(overOuter + 1f, clickOuter + 1f);
		}
	}

	/// <summary>
	/// 表示。子を表示した場合はtrueを返す
	/// </summary>
	public bool Visible(Transform trans) {
		if(stack.Count == 0) {
			//スタックが空の時は自身以外弾く
			if(trans == transform) {
				//子を取得する
				List<UICircleFragment> list = GetComponentsInChildren<UICircleFragment>(trans);
				if(list == null) return false;
				//表示
				VisibleFragment(list, stack.Count);
				//スタックに追加
				stack.Push(trans);
				return true;
			}
		} else {
			int distance = GetChildDistance(trans);
			int adjust = distance - stack.Count;
			if(adjust >= 0) {
				//子を取得する
				List<UICircleFragment> list = GetComponentsInChildren<UICircleFragment>(trans);
				if(list == null) return false;
				//表示
				VisibleFragment(list, stack.Count);
				//スタックに追加
				stack.Push(trans);
				return true;
			} else if(adjust < 0) {
				for(int i = adjust; i < 0; ++i) {
					Hide(stack.Pop());
				}
				return Visible(trans);
			}
		}
		return false;
	}

	/// <summary>
	/// 表示
	/// </summary>
	public void Visible() {
		Visible(transform);
	}

	/// <summary>
	/// 座標を指定して表示
	/// </summary>
	public void Visible(Vector2 point) {
		transform.localPosition = point;
		Visible(transform);
	}

	/// <summary>
	/// 断片群を非表示にする
	/// </summary>
	private void HideFragment(List<UICircleFragment> frags) {
		for(int i = 0; i < frags.Count; ++i) {
			frags[i].Hide();
		}
	}

	/// <summary>
	/// 非表示
	/// </summary>
	public void Hide(Transform trans) {
		//子を取得する
		List<UICircleFragment> list = GetComponentsInChildren<UICircleFragment>(trans);
		if(list == null) return;
		//非表示
		HideFragment(list);
		//UICircleFragmentを取得
		UICircleFragment frag = trans.GetComponent<UICircleFragment>();
		if(frag != null) {
			frag.ResetParentMode();
		}
	}

	/// <summary>
	/// 非表示
	/// </summary>
	public void Hide() {
		while(stack.Count > 0) {
			Hide(stack.Pop());
		}
	}

	/// <summary>
	/// コールバックの追加
	/// </summary>
	public void AddClickCallback(string path, Action<GameObject> callback) {
		if(clickCallbackDic.ContainsKey(path)) {
			clickCallbackDic[path] += callback;
		} else {
			clickCallbackDic.Add(path, callback);
		}
	}

	/// <summary>
	/// 断片をクリック
	/// </summary>
	public void FragmentClicked(GameObject gObj) {
		
		//パスを求める
		StringBuilder sb = new StringBuilder();
		int i = 0;
		foreach(var e in stack.Reverse()) {
			if(i == 0) {
				++i;
				continue;
			}
			sb.Append(e.name);
			sb.Append(".");
		}
		sb.Append(gObj.name);
		string path = sb.ToString();

		//辞書を確認
		if(clickCallbackDic.ContainsKey(path)) {
			//コールバックを走らせる
			clickCallbackDic[path](gObj);
		} 
	}

	#endregion
}