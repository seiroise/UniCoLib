using UnityEngine;
using System.Collections;
using Seiro.Scripts.Geometric;
using Seiro.Scripts.Utility;

namespace Seiro.Scripts.Graphics.Circle {

	/// <summary>
	/// 円のかけら
	/// </summary>
	public class CircleFragment {

		/// <summary>
		/// 表示方法のオプション
		/// </summary>
		public enum Indicate {
			Visible,
			Hide
		}

		/// <summary>
		/// 範囲の表示オプション
		/// </summary>
		public enum RangeIndicate {
			StartToEnd,
			EndToStart,
			CenterToOutside,
			Fixed
		}

		/// <summary>
		/// 半径の表示オプション
		/// </summary>
		public enum RadiusIndicate {
			InnerToOuter,
			OuterToInner,
			CenterToOutside,
			Fixed
		}

		//角度範囲
		private float startAnchor;          //始点
		public float StartAnchor { get { return startAnchor; } }
		private LerpFloat lerpStart;
		public float NowStart { get { return lerpStart.Value; } }

		private float endAnchor;            //終点
		public float EndAnchor { get { return endAnchor; } }
		private LerpFloat lerpEnd;
		public float NowEnd { get { return lerpEnd.Value; } }

		//半径
		private float innerAnchor;          //内径
		public float InnerAnchor { get { return innerAnchor; } }
		private LerpFloat lerpInner;
		public float NowInner { get { return lerpInner.Value; } }

		private float outerAnchor;          //外径
		public float OuterAnchor { get { return outerAnchor; } }
		private LerpFloat lerpOuter;
		public float NowOuter { get { return lerpOuter.Value; } }

		//オプション
		private float processSpeed = 20f;   //処理速度
		public float ProcessSpeed { get { return processSpeed; } set { processSpeed = value; } }
		private float density = 1f;         //密度(細かさ)
		public float Density { get { return density; } set { density = value; } }
		private Color drawColor = Color.white;  //色
		public Color DrawColor { get { return drawColor; } set { drawColor = value; } }

		//キャッシュ
		private bool cacheUpdated;
		public bool CacheUpdated { get { return cacheUpdated; } }
		private EasyMesh cache;
		public EasyMesh Cache {
			get {
				cacheUpdated = false;
				return cache;
			}
		}

		//フラグ
		private bool processing = false;    //処理中
		public bool Processing { get { return processing; } }
		private bool converge = false;      //収束フラグ

		//その他
		private const float EPSILON = 0.001f;

		#region Constructor

		public CircleFragment() {
			this.lerpStart = new LerpFloat();
			this.lerpEnd = new LerpFloat();
			this.lerpInner = new LerpFloat();
			this.lerpOuter = new LerpFloat();
		}

		#endregion

		#region Function

		/// <summary>
		/// 更新
		/// </summary>
		public void Update() {
			if(!processing) return;
			//更新
			float t = processSpeed * Time.deltaTime;
			lerpStart.Update(t, EPSILON);
			lerpEnd.Update(t, EPSILON);
			lerpInner.Update(t, EPSILON);
			lerpOuter.Update(t, EPSILON);

			//処理中確認
			processing = lerpStart.Update(t, EPSILON) || lerpEnd.Update(t, EPSILON) || lerpInner.Update(t, EPSILON) || lerpOuter.Update(t, EPSILON);
			//processing = start.processing || end.processing || inner.processing || outer.processing;
			//簡易メッシュの作成
			if(!processing && converge) {
				cache = null;
			} else {
				cache = MakeEasyMesh();
			}
			cacheUpdated = true;
		}

		/// <summary>
		/// 角度範囲の設定
		/// </summary>
		public void SetRange(float start, float end) {
			this.startAnchor = start;
			this.endAnchor = end;
		}

		/// <summary>
		/// 半径範囲の設定
		/// </summary>
		public void SetRadius(float inner, float outer) {
			this.innerAnchor = inner;
			this.outerAnchor = outer;
		}

		/// <summary>
		/// 表示方法の設定
		/// </summary>
		public void SetIndicate(Indicate indicate, RangeIndicate range, RadiusIndicate radius) {
			//3種類2つの項目を総当たり的に表現
			if(indicate == Indicate.Visible) {
				//表示
				converge = false;
				if(range == RangeIndicate.StartToEnd) {
					//startからend
					lerpStart.SetValues(startAnchor, startAnchor);
					lerpEnd.SetValues(startAnchor, endAnchor);
					//start.SetTarget(startAnchor);
					//end.SetTarget(endAnchor);
				} else if(range == RangeIndicate.EndToStart) {
					//endからstart
					lerpStart.SetValues(endAnchor, startAnchor);
					lerpEnd.SetValues(endAnchor, endAnchor);
					//start.SetTarget(startAnchor);
					//end.SetTarget(endAnchor);
				} else if(range == RangeIndicate.CenterToOutside) {
					//centerからoutside
					float center = (endAnchor - startAnchor) * 0.5f + startAnchor;
					lerpStart.SetValues(center, startAnchor);
					lerpEnd.SetValues(center, endAnchor);
					//start.SetTarget(startAnchor);
					//end.SetTarget(endAnchor);
				}

				if(radius == RadiusIndicate.InnerToOuter) {
					//innerからouter
					lerpInner.SetValues(innerAnchor, innerAnchor);
					lerpOuter.SetValues(innerAnchor, outerAnchor);
					//inner.SetTarget(innerAnchor);
					//outer.SetTarget(outerAnchor);
				} else if(radius == RadiusIndicate.OuterToInner) {
					//outerからinner
					lerpInner.SetValues(outerAnchor, innerAnchor);
					lerpOuter.SetValues(outerAnchor, outerAnchor);
					//inner.SetTarget(innerAnchor);
					//outer.SetTarget(outerAnchor);
				} else if(radius == RadiusIndicate.CenterToOutside) {
					//centerからoutside
					float center = (outerAnchor - innerAnchor) * 0.5f + innerAnchor;
					lerpInner.SetValues(center, innerAnchor);
					lerpOuter.SetValues(center, outerAnchor);
					//inner.SetTarget(innerAnchor);
					//outer.SetTarget(outerAnchor);
				}

			} else {
				//非表示
				converge = true;
				if(range == RangeIndicate.StartToEnd) {
					//startからend
					lerpStart.SetValues(startAnchor, endAnchor);
					lerpEnd.SetValues(endAnchor, endAnchor);
					//start.SetTarget(endAnchor);
					//end.SetTarget(endAnchor);
				} else if(range == RangeIndicate.EndToStart) {
					//endからstart
					lerpStart.SetValues(startAnchor, startAnchor);
					lerpEnd.SetValues(endAnchor, startAnchor);
					//start.SetTarget(startAnchor);
					//end.SetTarget(startAnchor);
				} else if(range == RangeIndicate.CenterToOutside) {
					//centerからoutside
					float center = (endAnchor - startAnchor) * 0.5f + startAnchor;
					lerpStart.SetValues(startAnchor, center);
					lerpEnd.SetValues(endAnchor, center);
					//start.SetTarget(center);
					//end.SetTarget(center);
				}

				if(radius == RadiusIndicate.InnerToOuter) {
					//innerからouter
					lerpInner.SetValues(innerAnchor, outerAnchor);
					lerpOuter.SetValues(outerAnchor, outerAnchor);
					//start.SetTarget(outerAnchor);
					//end.SetTarget(outerAnchor);
				} else if(radius == RadiusIndicate.OuterToInner) {
					//outerからinner
					lerpInner.SetValues(innerAnchor, innerAnchor);
					lerpOuter.SetValues(outerAnchor, innerAnchor);
					//start.SetTarget(innerAnchor);
					//end.SetTarget(innerAnchor);
				} else if(radius == RadiusIndicate.CenterToOutside) {
					//centerからoutside
					float center = (outerAnchor - innerAnchor) * 0.5f + innerAnchor;
					lerpInner.SetValues(innerAnchor, center);
					lerpOuter.SetValues(outerAnchor, center);
					//start.SetTarget(center);
					//end.SetTarget(center);
				}
			}

			//範囲の固定設定
			if(range == RangeIndicate.Fixed) {
				lerpStart.SetValues(startAnchor, startAnchor);
				lerpEnd.SetValues(endAnchor, endAnchor);
			}
			//半径の固定設定
			if(radius == RadiusIndicate.Fixed) {
				lerpInner.SetValues(innerAnchor, innerAnchor);
				lerpOuter.SetValues(outerAnchor, outerAnchor);
			}

			//処理中
			processing = true;
		}

		/// <summary>
		/// オプションの設定
		/// </summary>
		public void SetOptions(float progressSpeed, float density, Color color) {
			this.processSpeed = progressSpeed;
			this.density = density;
			this.drawColor = color;
		}

		/// <summary>
		/// 外径の目標値を設定する
		/// </summary>
		public void SetOuterTarget(float target) {
			lerpOuter.SetTarget(target);
			processing = true;
		}

		/// <summary>
		/// 現在のパラメータでキャッシュを更新
		/// </summary>
		public void UpdateCache() {
			cache = MakeEasyMesh();
			cacheUpdated = true;
		}

		/// <summary>
		/// 簡易メッシュの作成
		/// </summary>
		private EasyMesh MakeEasyMesh() {
			EasyMesh eMesh = new EasyMesh();
			//角度
			float diffAngle = lerpEnd.Value - lerpStart.Value;
			float addAngle = (diffAngle > 0 ? 1f : -1f) * density;
			float angle = 0f;
			int addNum = (int)(diffAngle / addAngle);
			int i = 0;
			//下準備
			Vector3[] verts = new Vector3[(addNum + 2) * 2];
			Color[] colors = new Color[verts.Length];
			int[] indices = new int[(addNum + 1) * 6];

			do {
				//verts
				verts[i * 2 + 0] = GeomUtil.DegToVector2(angle + lerpStart.Value) * lerpInner.Value;
				verts[i * 2 + 1] = GeomUtil.DegToVector2(angle + lerpStart.Value) * lerpOuter.Value;

				//colors
				colors[i * 2 + 0] = drawColor;
				colors[i * 2 + 1] = drawColor;

				//indices
				if(diffAngle > 0) {
					indices[i * 6 + 0] = (i + 0) * 2 + 0;
					indices[i * 6 + 1] = (i + 1) * 2 + 0;
					indices[i * 6 + 2] = (i + 0) * 2 + 1;
					indices[i * 6 + 3] = (i + 1) * 2 + 0;
					indices[i * 6 + 4] = (i + 1) * 2 + 1;
					indices[i * 6 + 5] = (i + 0) * 2 + 1;
				} else {
					indices[i * 6 + 0] = (i + 0) * 2 + 0;
					indices[i * 6 + 1] = (i + 0) * 2 + 1;
					indices[i * 6 + 2] = (i + 1) * 2 + 0;
					indices[i * 6 + 3] = (i + 1) * 2 + 0;
					indices[i * 6 + 4] = (i + 0) * 2 + 1;
					indices[i * 6 + 5] = (i + 1) * 2 + 1;
				}
				angle += addAngle;
				++i;
			} while(addNum >= i);
			angle = diffAngle;
			//verts
			verts[i * 2 + 0] = GeomUtil.DegToVector2(lerpEnd.Value) * lerpInner.Value;
			verts[i * 2 + 1] = GeomUtil.DegToVector2(lerpEnd.Value) * lerpOuter.Value;

			//colors
			colors[i * 2 + 0] = drawColor;
			colors[i * 2 + 1] = drawColor;

			eMesh.verts = verts;
			eMesh.colors = colors;
			eMesh.indices = indices;

			return eMesh;
		}

		/// <summary>
		/// start,end, inner, outerの平均変化量を取得する
		/// </summary>
		public float GetAvgDelta() {
			return (lerpStart.Delta + lerpEnd.Delta + lerpInner.Delta + lerpOuter.Delta) / 4f;
		}

		#endregion
	}
}