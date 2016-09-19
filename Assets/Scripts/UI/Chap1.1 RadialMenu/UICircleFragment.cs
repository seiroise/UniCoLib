using UnityEngine;
using Seiro.Scripts.Graphics.Circle;
using Seiro.Scripts.Graphics;
using Seiro.Scripts.EventSystems;
using Seiro.Scripts.Utility;
using Seiro.Scripts.Geometric;

/// <summary>
/// UI用の円の断片
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class UICircleFragment : MonoBehaviour, ICollisionEventHandler {

	private RadialMenu manager;
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

	//[Header("Radius")]
	private float overOuter = 1.2f;
	private float clickOuter = 1.1f;
	private float normalOuter = 1f;

	[Header("Sprite")]
	public LerpSpriteColor lerpSprite;
	public bool adjustSpriteScale = false;
	public float adjustScale = 1f;

	//ペアレントモード
	private bool parentMode = false;

	//フラグ
	private bool pointerOver = false;
	private bool pointerDown = false;
	private bool visibled = false;

	//閾値
	private const float SET_MESH_EPSILON = 0.1f;	//小さすぎるメッシュを設定しないため

	#region UnityEvent

	private void Awake() {
		mf = GetComponent<MeshFilter>();
		mc = GetComponent<MeshCollider>();

		lerpColor = new LerpColor();
		lerpColor.SetValues(normalColor, normalColor);

		if(lerpSprite) {
			lerpSprite.transform.localScale = Vector3.zero;
		}
	}

	private void Start() {
		gameObject.SetActive(false);
	}

	private void Update() {
		if(frag == null) return;
		UpdateFragment();
		UpdateColor();
		UpdateSprite();
	}

	#endregion

	#region Function

	/// <summary>
	/// 管理者の設定
	/// </summary>
	public void SetManager(RadialMenu manager) {
		this.manager = manager;
	}

	/// <summary>
	/// 更新と描画用メッシュの作成
	/// </summary>
	private void UpdateFragment() {
		if(!frag.Processing) return;
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
			//当たり判定用メッシュの設定
			if(!visibled){
				if(frag.GetAvgDelta() > SET_MESH_EPSILON) {
					mc.sharedMesh = mesh;
				} else {
					mc.sharedMesh = null;
				}
			} else {
				mc.sharedMesh = mesh;
			}
		}
		//更新の結果処理が終わった場合
		if(!frag.Processing) {
			if(!visibled) gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// 色の更新
	/// </summary>
	private void UpdateColor() {
		if(!lerpColor.Processing) return;
		lerpColor.Update(animationT * Time.deltaTime);
		frag.DrawColor = lerpColor.Value;
		if(!frag.Processing) frag.UpdateCache();
	}

	/// <summary>
	/// スプライトの更新
	/// </summary>
	private void UpdateSprite() {

		if(!frag.Processing) return;
		if(!lerpSprite) return;

		//座標の調整
		float centerAngle = (frag.NowEnd - frag.NowStart) * 0.5f + frag.NowStart;
		float radiusRange = (frag.NowOuter - frag.NowInner);
		float centerRadius = radiusRange * 0.5f + frag.NowInner;
		Vector3 pos = GeomUtil.DegToVector2(centerAngle) * centerRadius;
		pos.z = transform.localPosition.z - 1f;

		//lerpSpriteは自身のローカルにいること?
		lerpSprite.transform.localPosition = pos;

		//大きさの調整
		if(adjustSpriteScale) {
			Vector2 size = lerpSprite.Target.sprite.bounds.size;
			float scale = radiusRange / size.magnitude;
			lerpSprite.transform.localScale = Vector2.one * (scale * adjustScale);
		}
	}

	/// <summary>
	/// 表示
	/// </summary>
	public void Visible(float start, float end, float inner, float outer) {

		//フラグメントの表示
		if(frag == null) {
			frag = new CircleFragment();
		}
		frag.SetRange(start, end);
		frag.SetRadius(inner, outer);
		frag.SetOptions(indicateT, density, normalColor);
		frag.SetIndicate(CircleFragment.Indicate.Visible, range, radius);

		//スプライトの表示
		if(lerpSprite != null) {
			lerpSprite.SetAlphas(0f, 1f);
		}

		visibled = true;
	}

	/// <summary>
	/// 非表示
	/// </summary>
	public void Hide() {

		//フラグメントの非表示
		if(frag == null) return;
		frag.ProcessSpeed = indicateT;
		frag.SetIndicate(CircleFragment.Indicate.Hide, range, radius);

		//スプライトの非表示
		if(lerpSprite != null) {
			lerpSprite.SetAlphaTarget(0f);
		}

		visibled = false;
	}

	/// <summary>
	/// 倍率を指定して外径を設定
	/// </summary>
	private void SetOuterTarget(float scale) {
		float range = frag.OuterAnchor - frag.InnerAnchor;
		frag.ProcessSpeed = animationT;
		frag.SetOuterTarget(frag.InnerAnchor + range * scale);
	}

	/// <summary>
	/// オーバー、クリック時の外径の倍率
	/// </summary>
	public void SetOuterScale(float over, float click) {
		overOuter = over;
		clickOuter = click;
	}

	/// <summary>
	/// ペアレントモードへ
	/// </summary>
	private void SetParentMode() {
		parentMode = true;
		float max = Mathf.Max(overOuter, clickOuter);
		SetOuterTarget(max);
		lerpColor.SetTarget(parentColor);
	}

	/// <summary>
	/// ペアレントモードの解除
	/// </summary>
	public void ResetParentMode() {
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
			manager.FragmentClicked(gameObject);
			if(manager.Visible(transform)) {
				//子を表示したのでペアレントモードへ
				SetParentMode();
			} else {
				//子を表示してない場合はUIを非表示に
				manager.Hide();
			}
		}
	}

	#endregion
}