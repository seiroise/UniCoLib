using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Geometric;
using Seiro.Scripts.Graphics.PolyLine2D.Snap;

namespace Seiro.Scripts.Graphics.PolyLine2D {

	/// <summary>
	/// 二次元ポリラインの作成
	/// </summary>
	public class PolyLine2DMaker : PolyLine2DEditorComponent {

		public enum ExitType {
			Key,
			StartAndEnd
		}

		[Header("InputParameter")]
		public int addButton = 0;                   //追加ボタン
		public int removeButton = 1;                //削除ボタン
		public KeyCode clearKey = KeyCode.Escape;   //全削除/戻るボタン
		private const float EPSILON = 0.01f;        //マウス座標の移動差分の検出閾値

		[Header("ExitConditions")]
		public ExitType exitType = ExitType.Key;    //終了パターン
		public KeyCode exitKey = KeyCode.Return;    //終了キー
		private bool exit = false;                  //終了

		[Header("Callback")]
		public ExitEvent onExit;                    //終了コールバック

		//ModuleComponent
		private PolyLine2DRenderer renderer;        //描画担当
		private PolyLine2DSupporter supporter;      //補助担当

		#region UnityEvent

		private void Update() {
			InputCheck();
		}

		#endregion

		#region VirtualFunction

		public override void Initialize(PolyLine2DEditor editor) {
			base.Initialize(editor);
			renderer = editor.renderer;
			supporter = editor.supporter;
		}

		public override void Enable() {
			base.Enable();

			//スナップの設定
			SetSnap();
			exit = false;
		}

		public override void Disable() {
			//線の削除
			renderer.Clear();
			//スナップの削除
			editor.supporter.Clear();

			base.Disable();
		}

		#endregion

		#region Function

		/// <summary>
		/// 入力確認
		/// </summary>
		private void InputCheck() {

			//ボタン入力
			if(Input.GetMouseButtonDown(addButton)) {
				//追加
				Add();
			} else if(Input.GetMouseButtonDown(removeButton)) {
				//削除
				Remove();
			} else if(Input.GetKeyDown(clearKey)) {
				//全削除/戻る
				Escape();
			} else {
				//移動量の確認
				CheckDelta();
			}
		}

		/// <summary>
		/// 追加
		/// </summary>
		private void Add() {
			Vector2 mPoint;
			if(!editor.GetMousePoint(out mPoint)) return;

			//例外検出
			if(!editor.checker.AddCheck(renderer.GetVertices(), mPoint)) return;

			//スナップ
			Vector2 snapPoint;
			if(editor.supporter.Snap(mPoint, out snapPoint)) {
				mPoint = snapPoint;
				//終了スナップ確認
				editor.supporter.SnapCallback();
				if(exit) {
					//追加
					renderer.Add(mPoint);
					Exit();
					return;
				}
			}

			//追加
			renderer.Add(mPoint);

			//スナップの設定
			SetSnap();
		}

		/// <summary>
		/// 削除
		/// </summary>
		private void Remove() {
			//削除
			renderer.RemoveLast();
			//スナップの設定
			SetSnap();
			SetNoticeLine();
		}

		/// <summary>
		/// 戻る
		/// </summary>
		private void Escape() {

			//頂点数が0確認
			if(renderer.GetVertexCount() == 0) {
				//終了
				Exit();
			} else {
				//全削除
				renderer.Clear();
				//スナップだけ残す
				SetSnap();
			}
		}

		/// <summary>
		/// 移動量の確認
		/// </summary>
		private void CheckDelta() {
			//差分確認
			Vector2 mDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			if(mDelta.magnitude > EPSILON) {
				SetNoticeLine();
			}
		}

		/// <summary>
		/// 予告線の設定
		/// </summary>
		private void SetNoticeLine() {
			int count = renderer.GetVertexCount();
			if(count == 0) {
				renderer.SetSubVertices(null);
				return;
			}
			//予告線の更新
			Vector2 mPoint;
			editor.GetMousePoint(out mPoint);
			//スナップ
			Vector2 snapPoint;
			if(editor.supporter.Snap(mPoint, out snapPoint)) {
				mPoint = snapPoint;
			}
			//補助線の描画
			renderer.SetSubVertices(renderer.GetVertex(count - 1), mPoint);
		}

		/// <summary>
		/// スナップの設定
		/// </summary>
		private void SetSnap() {
			List<Vector2> vertices = renderer.GetVertices();
			int count = vertices.Count;
			float snapForce = 0.5f;

			//既存のスナップの消去
			supporter.Clear();

			//終了スナップ
			if(exitType == ExitType.StartAndEnd) {
				if(count >= 2) {
					Vector2 start = vertices[0];
					supporter.AddSnap(10, new PointSnap(start, snapForce), OnSnapEndPoint);
				}
			}

			//デフォルトスナップの追加
			supporter.AddDefaultSnap();

			//描画
			supporter.Draw();
		}

		/// <summary>
		/// 終了
		/// </summary>
		private void Exit() {
			List<Vector2> vertices = renderer.GetVertices();
			//コールバック
			onExit.Invoke(vertices.Count == 0 ? null : vertices);
		}

		#endregion

		#region Callback

		/// <summary>
		/// スナップの終了コールバック
		/// </summary>
		private void OnSnapEndPoint() {
			exit = true;
		}

		#endregion
	}
}