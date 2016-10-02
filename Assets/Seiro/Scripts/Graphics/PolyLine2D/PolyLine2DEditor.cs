using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using Seiro.Scripts.Utility;

namespace Seiro.Scripts.Graphics.PolyLine2D {

	/// <summary>
	/// 二次元ポリラインの編集
	/// </summary>
	public class PolyLine2DEditor : MonoBehaviour{

		[Header("StateComponent")]
		public PolyLine2DMaker maker;
		public PolyLine2DAdjuster adjuster;

		[Header("ModuleComponent")]
		public PolyLine2DRenderer renderer;
		public PolyLine2DChecker checker;
		public PolyLine2DSupporter supporter;

		private PolyLine2DEditorComponent nowState;
		private List<PolyLine2DEditorComponent> coms;

		[Header("Camera")]
		public Camera cam;

		[Header("Callback")]
		public ExitEvent onExit;		//MakerとAdjusterの終了イベント

		private PolyLine2D polyLine;	//みんなでこれを編集する

		#region UnityEvent

		private void Awake() {
			coms = new List<PolyLine2DEditorComponent>();
			coms.Add(maker);
			coms.Add(adjuster);
			coms.Add(renderer);
			coms.Add(checker);
			coms.Add(supporter);

			foreach(var e in coms) {
				e.Initialize(this);
			}

			//StateComponentの無効化
			maker.Disable();
			adjuster.Disable();

			nowState = null;
		}

		private void Start() {
			EnableMaker();
		}

		#endregion

		#region Function

		/// <summary>
		/// Makerの有効化
		/// </summary>
		public void EnableMaker() {
			//現在の状態を無効化
			if(nowState == maker) {
				return;
			} else if(nowState != null) {
				nowState.Disable();
			}

			nowState = maker;

			//Makerを有効化
			maker.Enable();
			//終了コールバックの設定
			maker.onExit.RemoveAllListeners();
			maker.onExit.AddListener(OnExit);
		}

		/// <summary>
		/// Adjusterの有効化
		/// </summary>
		public void EnableAdjuster(List<Vector2> vertices, bool connected) {
			//現在の状態を無効化
			if(nowState == adjuster) {
				return;
			} else if(nowState != null) {
				nowState.Disable();
			}
			nowState = adjuster;

			//Adjusterを有効化
			adjuster.Enable();
			//頂点の設定
			adjuster.SetVertices(vertices, connected);
			//終了コールバックの設定
			adjuster.onExit.RemoveAllListeners();
			adjuster.onExit.AddListener(OnExit);
		}

		/// <summary>
		/// マウス座標の取得
		/// </summary>
		public Vector2 GetMousePoint() {
			return FuncBox.GetMousePosition(cam);
		}

		#endregion

		#region Callback

		/// <summary>
		/// Makerの終了コールバック
		/// </summary>
		private void OnExit(List<Vector2> vertices) {
			//イベント発火
			onExit.Invoke(vertices);
			EnableAdjuster(vertices, true);
		}

		#endregion
	}
}