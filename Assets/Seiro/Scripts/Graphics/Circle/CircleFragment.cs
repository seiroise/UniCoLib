using UnityEngine;
using System.Collections;
using Seiro.Scripts.Geometric;

namespace Seiro.Scripts.Graphics.Circle {

	/// <summary>
	/// 円のかけら
	/// </summary>
	public class CircleFragment {

		/// <summary>
		/// 線形補完用パラメータ
		/// </summary>
		public class LerpParm {

			public float value;         //現在値
			private float target;       //目標値
			public bool processing;     //処理中

			#region Constructor

			public LerpParm(float value, float target) {
				this.value = value;
				this.target = target;
				this.processing = true;
			}

			#endregion

			#region Function

			/// <summary>
			/// 更新
			/// </summary>
			public float Update(float t, float epsilon = 0.001f) {
				if(!processing) return value;
				value = Mathf.Lerp(value, target, t);
				if(Mathf.Abs(target - value) < epsilon) {
					value = target;
					processing = false;
				}
				return value;
			}

			#endregion

		}

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
		}

		/// <summary>
		/// 半径の表示オプション
		/// </summary>
		public enum RadiusIndicate {
			InnerToOuter,
			OuterToInner,
			Fixed
		}

		//角度範囲
		private float startAnchor;        //始点
		private LerpParm start;
		private float endAnchor;          //終点
		private LerpParm end;

		//半径
		private float innerAnchor;        //内径
		private LerpParm inner;
		private float outerAnchor;        //外径
		private LerpParm outer;

		//処理速度
		private float processSpeed = 20f;

		//密度(細かさ)
		private float density;

		//色
		private Color color;

		//キャッシュ
		private EasyMesh cache;
		public EasyMesh Cache { get { return cache; } }

		//フラグ
		private bool processing = false;    //処理中
		public bool Processing { get { return processing; } }

		//その他
		private const float EPSILON = 0.001f;

		#region Function

		/// <summary>
		/// 更新
		/// </summary>
		public void Update() {
			if(!processing) return;
			if(start == null || end == null || inner == null || outer == null) return;
			//更新
			float t = processSpeed * Time.deltaTime;
			start.Update(t, EPSILON);
			end.Update(t, EPSILON);
			inner.Update(t, EPSILON);
			outer.Update(t, EPSILON);

			//処理中確認
			processing = start.processing || end.processing || inner.processing || outer.processing;

			//簡易メッシュの作成
			cache = MakeEasyMesh();
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
				if(range == RangeIndicate.StartToEnd) {
					//startからend
					start = new LerpParm(startAnchor, startAnchor);
					end = new LerpParm(startAnchor, endAnchor);

					if(radius == RadiusIndicate.InnerToOuter) {
						inner = new LerpParm(innerAnchor, innerAnchor);
						outer = new LerpParm(innerAnchor, outerAnchor);
					} else if(radius == RadiusIndicate.OuterToInner) {
						inner = new LerpParm(outerAnchor, innerAnchor);
						outer = new LerpParm(outerAnchor, outerAnchor);
					}
				} else {
					//endからstart
					start = new LerpParm(endAnchor, startAnchor);
					end = new LerpParm(endAnchor, endAnchor);

					if(radius == RadiusIndicate.InnerToOuter) {
						inner = new LerpParm(innerAnchor, innerAnchor);
						outer = new LerpParm(innerAnchor, outerAnchor);
					} else if(radius == RadiusIndicate.OuterToInner) {
						inner = new LerpParm(outerAnchor, innerAnchor);
						outer = new LerpParm(outerAnchor, outerAnchor);
					}
				}
			} else {
				//非表示
				if(range == RangeIndicate.StartToEnd) {
					//startからend
					start = new LerpParm(startAnchor, endAnchor);
					end = new LerpParm(endAnchor, endAnchor);

					if(radius == RadiusIndicate.InnerToOuter) {
						inner = new LerpParm(innerAnchor, outerAnchor);
						outer = new LerpParm(outerAnchor, outerAnchor);
					} else if(radius == RadiusIndicate.OuterToInner) {
						inner = new LerpParm(innerAnchor, innerAnchor);
						outer = new LerpParm(outerAnchor, innerAnchor);
					}
				} else {
					//endからstart
					start = new LerpParm(startAnchor, startAnchor);
					end = new LerpParm(endAnchor, startAnchor);

					if(radius == RadiusIndicate.InnerToOuter) {
						inner = new LerpParm(innerAnchor, outerAnchor);
						outer = new LerpParm(outerAnchor, outerAnchor);
					} else if(radius == RadiusIndicate.OuterToInner) {
						inner = new LerpParm(innerAnchor, innerAnchor);
						outer = new LerpParm(outerAnchor, innerAnchor);
					}
				}
			}

			//半径の固定設定
			if(radius == RadiusIndicate.Fixed) {
				inner = new LerpParm(innerAnchor, innerAnchor);
				outer = new LerpParm(outerAnchor, outerAnchor);
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
			this.color = color;
		}

		/// <summary>
		/// 簡易メッシュの作成
		/// </summary>
		private EasyMesh MakeEasyMesh() {
			EasyMesh eMesh = new EasyMesh();
			//角度
			float diffAngle = end.value - start.value;
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
				verts[i * 2 + 0] = GeomUtil.DegToVector2(angle + start.value) * inner.value;
				verts[i * 2 + 1] = GeomUtil.DegToVector2(angle + start.value) * outer.value;

				//colors
				colors[i * 2 + 0] = color;
				colors[i * 2 + 1] = color;

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
			verts[i * 2 + 0] = GeomUtil.DegToVector2(end.value) * inner.value;
			verts[i * 2 + 1] = GeomUtil.DegToVector2(end.value) * outer.value;

			//colors
			colors[i * 2 + 0] = color;
			colors[i * 2 + 1] = color;

			eMesh.verts = verts;
			eMesh.colors = colors;
			eMesh.indices = indices;

			return eMesh;
		}

		#endregion
	}
}