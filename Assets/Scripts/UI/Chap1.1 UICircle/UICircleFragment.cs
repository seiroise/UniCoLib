using UnityEngine;
using Seiro.Scripts.Graphics.Circle;
using Seiro.Scripts.Graphics;
using Seiro.Scripts.EventSystems;
using Seiro.Scripts.Utility;

/// <summary>
/// 円形UIの断片
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class UICircleFragment : MonoBehaviour, ICollisionEventHandler {

	private UICircle manager;
	private CircleFragment frag;

	private MeshFilter mf;
	//private MeshRenderer mr;
	private MeshCollider mc;

	[Header("Parameter")]
	public float density = 1f;

	[Header("Indicate Option")]
	public CircleFragment.RangeIndicate range = CircleFragment.RangeIndicate.StartToEnd;
	public CircleFragment.RadiusIndicate radius = CircleFragment.RadiusIndicate.Fixed;
	public float indicateT = 10f;

	[Header("Animation")]
	public float animationT = 10f;

	[Header("Color")]
	public Color normalColor = Color.white;
	public Color overColor = new Color(0.8f, 0.6f, 0.1f);
	public Color clickColor = new Color(1f, 0.4f, 0f);
	public Color parentColor = Color.red;
	private LerpColor lerpColor;

	[Header("Radius")]
	public float overOuter = 1.2f;
	public float clickOuter = 1.1f;
	private float normalOuter = 1f;

	//ペアレントモード
	private bool parentMode = false;

	//フラグ
	private bool pointerOver = false;
	private bool pointerDown = false;

	#region UnityEvent

	private void Awake() {
		mf = GetComponent<MeshFilter>();
		mc = GetComponent<MeshCollider>();

		lerpColor = new LerpColor();
		lerpColor.SetValues(normalColor, normalColor);
	}

	private void Update() {
		UpdateFragment();
		UpdateColor();
	}

	#endregion

	#region Function

	/// <summary>
	/// 管理者の設定
	/// </summary>
	public void SetManager(UICircle manager) {
		this.manager = manager;
	}

	/// <summary>
	/// 更新と描画用メッシュの作成
	/// </summary>
	private void UpdateFragment() {
		if(frag == null) return;
		//更新
		frag.Update();
		//描画キャッシュが更新されていれば更新
		if(frag.CacheUpdated) {
			EasyMesh eMesh = frag.Cache;
			Mesh mesh = null;
			if(eMesh != null) {
				mesh = eMesh.ToMesh();
			}
			mf.mesh = mesh;
			mc.sharedMesh = mesh;
		}
	}

	/// <summary>
	/// 色の更新
	/// </summary>
	private void UpdateColor() {
		if(frag == null) return;
		if(!lerpColor.Processing) return;
		lerpColor.Update(animationT * Time.deltaTime);
		frag.DrawColor = lerpColor.Value;
		if(!frag.Processing) frag.UpdateCache();
	}

	/// <summary>
	/// 表示
	/// </summary>
	public void Visible(float start, float end, float inner, float outer) {
		if(frag == null) {
			frag = new CircleFragment();
		}
		frag.SetRange(start, end);
		frag.SetRadius(inner, outer);
		frag.SetOptions(indicateT, density, normalColor);
		frag.SetIndicate(CircleFragment.Indicate.Visible, range, radius);
	}

	/// <summary>
	/// 非表示
	/// </summary>
	public void Hide() {
		if(frag == null) return;
		frag.SetIndicate(CircleFragment.Indicate.Hide, range, radius);
	}

	/// <summary>
	/// 倍率を指定して外径を設定
	/// </summary>
	private void SetOuterTarget(float scale) {
		float range = frag.OuterAnchor - frag.InnerAnchor;
		frag.SetOuterTarget(frag.InnerAnchor + range * scale);
	}

	/// <summary>
	/// ペアレントモードへ
	/// </summary>
	private void SetParentMode() {
		Debug.Log("SetParentMode");
		parentMode = true;
		float max = Mathf.Max(overOuter, clickOuter);
		SetOuterTarget(max);
		lerpColor.SetTarget(parentColor);
	}

	/// <summary>
	/// ペアレントモードの解除
	/// </summary>
	public void ResetParentMode() {
		Debug.Log("ResetParentMode");
		parentMode = false;
		SetOuterTarget(normalOuter);
		lerpColor.SetTarget(normalColor);
	}

	#endregion

	#region ICollisionEventHandler

	public void OnPointerEnter(RaycastHit hit) {
		if(parentMode) return;
		lerpColor.SetTarget(overColor);
		SetOuterTarget(overOuter);
		pointerOver = true;
	}

	public void OnPointerExit(RaycastHit hit) {
		if(parentMode) return;
		lerpColor.SetTarget(normalColor);
		SetOuterTarget(normalOuter);
		pointerOver = false;
	}

	public void OnPointerDown(RaycastHit hit) {
		if(parentMode) return;
		lerpColor.SetTarget(clickColor);
		SetOuterTarget(clickOuter);
		pointerDown = true;
	}

	public void OnPointerUp(RaycastHit hit) {
		if(parentMode) return;
		if(pointerOver) {
			lerpColor.SetTarget(overColor);
			SetOuterTarget(overOuter);
		} else {
			lerpColor.SetTarget(normalColor);
			SetOuterTarget(normalOuter);
		}
		pointerDown = false;
	}

	public void OnPointerClick(RaycastHit hit) {
		if(parentMode) return;
		if(manager) {
			if(manager.Visible(transform)) {
				//子を表示したのでペアレントモードへ
				SetParentMode();
			}
		}
	}

	#endregion
}