using UnityEngine;
using UnityEngine.Events;
using System;
using Seiro.Scripts.Utility;
using Seiro.Scripts.Graphics.Circle;
using Seiro.Scripts.Geometric;
using Seiro.Scripts.Graphics;
using Seiro.Scripts.ObjectPool;

namespace Seiro.Scripts.EventSystems.Interface.Circle {

	/// <summary>
	/// 円形のインタフェース
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class SUICircle : MonoBehaviour, ICollisionEventHandler, IMonoPoolItem<SUICircle> {

		[Serializable]
		public class MarkerEvent : UnityEvent<GameObject> { }

		private CircleFragment circle;
		private MeshFilter mf;
		private MeshCollider mc;

		[Header("Geometry")]
		public float startAngle = 0f;
		public float endAngle = 360f;
		public float innerRadius = 1f;
		public float outerRadius = 2f;
		public float density = 1f;

		[Header("Indicate")]
		public CircleFragment.RangeIndicate rangeIndicate = CircleFragment.RangeIndicate.StartToEnd;
		public CircleFragment.RadiusIndicate radiusIndicate = CircleFragment.RadiusIndicate.Fixed;
		public float indicateT = 10f;

		[Header("Interaction")]
		public float interactionT = 10f;

		[Header("Color Interaction")]
		public Color normalColor = Color.white;
		public Color overColor = new Color(0.8f, 0.6f, 0.1f);
		public Color clickColor = new Color(1f, 0.4f, 0f);
		private LerpColor lerpColor;

		[Header("Outer Interaction")]
		public float overOuter = 1.2f;
		public float clickOuter = 1.1f;
		private float normalOuter = 1f;

		[Header("Sprite")]
		public LerpSpriteColor lerpSprite;
		public bool adjustSpriteScale = false;
		public float adjustScale = 1f;

		[Header("Callback")]
		public MarkerEvent onPointerClick;
		public MarkerEvent onPointerDown;
		public MarkerEvent onPointerUp;

		//フラグ
		private bool pointerOver = false;
		private bool pointerDown = false;
		private bool visibled = false;

		//閾値
		private const float SET_MESH_EPSILON = 0.1f;    //小さすぎるメッシュを設定しないため

		#region UnityEvent

		private void Awake() {
			mf = GetComponent<MeshFilter>();
			mc = GetComponent<MeshCollider>();

			circle = new CircleFragment();

			lerpColor = new LerpColor();

			if(lerpSprite) {
				lerpSprite.transform.localScale = Vector3.zero;
			}
		}

		private void Update() {
			if(circle == null) return;
			UpdateFragment();
			UpdateColor();
			UpdateSprite();
		}

		#endregion

		#region Function

		/// <summary>
		/// 更新と描画用メッシュの作成
		/// </summary>
		private void UpdateFragment() {
			if(!circle.Processing) return;
			//更新
			circle.Update();
			//描画キャッシュが更新されていれば更新
			if(circle.CacheUpdated) {
				EasyMesh eMesh = circle.Cache;
				Mesh mesh = null;
				if(eMesh != null) {
					mesh = eMesh.ToMesh();
				}
				mf.mesh = mesh;
				//当たり判定用メッシュの設定
				if(!visibled) {
					if(circle.GetAvgDelta() > SET_MESH_EPSILON) {
						mc.sharedMesh = mesh;
					} else {
						mc.sharedMesh = null;
					}
				} else {
					mc.sharedMesh = mesh;
				}
			}
			//更新の結果処理が終わった場合
			if(!circle.Processing) {
				if(!visibled) gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// 色の更新
		/// </summary>
		private void UpdateColor() {
			if(!lerpColor.Processing) return;
			lerpColor.Update(interactionT * Time.deltaTime);
			circle.DrawColor = lerpColor.Value;
			if(!circle.Processing) circle.UpdateCache();
		}

		/// <summary>
		/// スプライトの更新
		/// </summary>
		private void UpdateSprite() {

			if(!circle.Processing) return;
			if(!lerpSprite) return;

			//座標の調整
			float centerAngle = (circle.NowEnd - circle.NowStart) * 0.5f + circle.NowStart;
			float radiusRange = (circle.NowOuter - circle.NowInner);
			float centerRadius = radiusRange * 0.5f + circle.NowInner;
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
		public void Visible() {

			if(visibled) return;

			//有効化
			if(!gameObject.activeInHierarchy) {
				gameObject.SetActive(true);
			}

			//フラグメントの設定
			circle.SetRange(startAngle, endAngle);
			circle.SetRadius(innerRadius, outerRadius);
			circle.SetOptions(indicateT, density, normalColor);
			circle.SetIndicate(CircleFragment.Indicate.Visible, rangeIndicate, radiusIndicate);

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

			if(!visibled) return;

			//フラグメントの非表示
			circle.ProcessSpeed = indicateT;
			circle.SetIndicate(CircleFragment.Indicate.Hide, rangeIndicate, radiusIndicate);

			//スプライトの非表示
			if(lerpSprite != null) {
				lerpSprite.SetAlphaTarget(0f);
			}

			visibled = false;
		}

		public void SetColor(Color color) {
			normalColor = color;
			overColor = color;
			clickColor = color;

			lerpColor.SetValues(color, color);
		}

		/// <summary>
		/// 倍率を指定して外径を設定
		/// </summary>
		private void SetOuterTarget(float scale) {
			float range = outerRadius - innerRadius;
			circle.ProcessSpeed = interactionT;
			circle.SetOuterTarget(circle.InnerAnchor + range * scale);
		}

		#endregion

		#region ICollisionEventHandler

		public void OnPointerEnter(RaycastHit hit) {
			if(!visibled) return;
			lerpColor.SetTarget(overColor);
			SetOuterTarget(overOuter);
			pointerOver = true;
		}

		public void OnPointerExit(RaycastHit hit) {
			if(!visibled) return;
			lerpColor.SetTarget(normalColor);
			SetOuterTarget(normalOuter);
			pointerOver = false;
		}

		public void OnPointerDown(RaycastHit hit) {
			if(!visibled) return;
			lerpColor.SetTarget(clickColor);
			SetOuterTarget(clickOuter);

			//イベント発火
			onPointerDown.Invoke(gameObject);

			pointerDown = true;
		}

		public void OnPointerUp(RaycastHit hit) {
			if(!visibled) return;
			if(pointerOver) {
				lerpColor.SetTarget(overColor);
				SetOuterTarget(overOuter);
			} else {
				lerpColor.SetTarget(normalColor);
				SetOuterTarget(normalOuter);
			}

			//イベント発火
			onPointerUp.Invoke(gameObject);

			pointerDown = false;
		}

		public void OnPointerClick(RaycastHit hit) {
			if(!visibled) return;
			//イベント発火
			onPointerClick.Invoke(gameObject);
		}

		#endregion

		#region IMonoPoolItem

		public void Activate() {
			Visible();
		}

		public SUICircle GetThis() {
			return this;
		}

		#endregion
	}
}