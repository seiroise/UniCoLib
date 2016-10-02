using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.EventSystems.Interface.Circle;

namespace Seiro.Scripts.Graphics.PolyLine2D {

	/// <summary>
	/// 二次元ポリラインの調整
	/// </summary>
	public class PolyLine2DAdjuster : PolyLine2DEditorComponent {

		public enum MarkerType {
			Vertex,
			Midpoint
		}

		[Header("LineParameter")]
		public float width = 0.1f;
		public Color color = Color.white;

		[Header("Marker")]
		public SUICirclePool markerPool;
		private List<SUICircle> markers;
		public Transform markerParent;
		public Color vertex;
		public Color midpoint;
		private const float EPSILON = 0.01f;

		[Header("ExitConditions")]
		public KeyCode exitKey = KeyCode.Return;    //終了キー

		[Header("Callback")]
		public ExitEvent onExit;                //終了コールバック

		//Module Component
		private PolyLine2DRenderer renderer;    //描画担当
		private PolyLine2DSupporter supporter;  //補助担当

		//Adjust
		private List<Vector2> adjustVertices;   //調整頂点群
		private int adjustVertsCount;           //調整頂点群の数
		private int adjustIndex;                //調整頂点番号
		private bool adjusting = false;         //調整中

		//Connected
		private bool connected;                 //先頭と末尾の接続
		private bool adjustConnected;           //接続点の調整

		#region UnityEvent

		private void Update() {
			InputCheck();
		}

		#endregion

		#region VirtualFunction

		public override void Initialize(PolyLine2DEditor editor) {
			base.Initialize(editor);
			markers = new List<SUICircle>();
			renderer = editor.renderer;
			supporter = editor.supporter;
		}

		#endregion

		#region Function

		/// <summary>
		/// 調整する頂点群を設定する。connectedは先頭と末尾の接続しているか
		/// </summary>
		public void SetVertices(List<Vector2> vertices, bool connected = false) {
			this.connected = connected;
			//マーカーの表示
			IndicateMarkers(vertices);
			//線の表示
			renderer.SetVertices(vertices);
		}

		/// <summary>
		/// 頂点リストから調整用マーカーの表示
		/// </summary>
		private void IndicateMarkers(List<Vector2> vertices) {

			int count = vertices.Count;

			//中点マーカー
			for(int i = 0; i < count - 1; ++i) {
				markers.Add(InstantiateMidpointMarker((i + 1).ToString(), vertices[i], vertices[i + 1]));
			}

			//頂点マーカー
			if(connected) --count;
			for(int i = 0; i < count; ++i) {
				markers.Add(InstantiateVertexMarker(i.ToString(), vertices[i]));
			}
		}

		/// <summary>
		/// 調整用マーカーの非表示
		/// </summary>
		private void HideMarkers() {
			for(int i = 0; i < markers.Count; ++i) {
				markers[i].Hide();
			}
		}

		/// <summary>
		/// 調整用マーカーの生成
		/// </summary>
		private SUICircle InstantiateMarker(Vector2 point) {
			return markerPool.PopItem(point);
		}

		/// <summary>
		/// 頂点調整用マーカーの生成
		/// </summary>
		private SUICircle InstantiateVertexMarker(string markerName, Vector2 point) {
			SUICircle marker = InstantiateMarker(point);
			marker.name = markerName;
			//marker.normalColor = vertex;
			marker.SetColor(vertex);
			marker.Visible();
			marker.onPointerDown.RemoveAllListeners();
			marker.onPointerDown.AddListener(OnVertexMarkerDown);
			return marker;
		}

		/// <summary>
		/// 中点調整用マーカーの生成
		/// </summary>
		private SUICircle InstantiateMidpointMarker(string markerName, Vector2 p1, Vector2 p2) {
			Vector2 point = (p2 - p1) * 0.5f + p1;
			SUICircle marker = InstantiateMarker(point);
			marker.name = markerName;
			//marker.normalColor = midpoint;
			marker.SetColor(midpoint);
			marker.Visible();
			marker.onPointerDown.RemoveAllListeners();
			marker.onPointerDown.AddListener(OnMidpointMarkerDown);
			return marker;
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		private void InputCheck() {

			if(Input.GetKeyDown(exitKey)) {
				Exit();
			}

			if(adjusting) {
				Vector2 mDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
				if(mDelta.magnitude > EPSILON) {
					SetNoticeLine();
				}
				if(Input.GetMouseButtonUp(0)) {
					EndVertexMove(editor.GetMousePoint());
				}
			}
		}

		/// <summary>
		/// 予告線の設定
		/// </summary>
		private void SetNoticeLine() {

			//マウス座標
			Vector2 mPoint = editor.GetMousePoint();
			//スナップ
			Vector2 snapPoint;
			if(editor.supporter.Snap(mPoint, out snapPoint)) {
				mPoint = snapPoint;
			}

			//補助線の描画
			if(adjustConnected) {
				//接続点の変更
				Vector2 p1 = adjustVertices[1];
				Vector2 p2 = adjustVertices[adjustVertsCount - 2];
				renderer.SetSubVertices(p1, mPoint, p2);
			} else {
				Vector2 p1 = adjustVertices[(adjustIndex + adjustVertsCount - 1) % adjustVertsCount];
				Vector2 p2 = adjustVertices[(adjustIndex + 1) % adjustVertsCount];
				renderer.SetSubVertices(p1, mPoint, p2);
			}
		}

		/// <summary>
		/// 頂点移動開始
		/// </summary>
		private void StartVertexMove(List<Vector2> vertices, int index) {
			adjustVertices = vertices;
			adjustVertsCount = adjustVertices.Count;
			adjustIndex = index;

			//接続している場合
			if(connected) {
				adjustConnected = (index == 0 || index == adjustVertsCount - 1);
			}
			HideMarkers();
			SetNoticeLine();
			adjusting = true;
		}

		/// <summary>
		/// 頂点移動終了
		/// </summary>
		private void EndVertexMove(Vector2 movedPoint) {
			//頂点座標の変更
			if(adjustConnected) {
				int prevLast = adjustVertsCount - 1;
				renderer.Change(0, movedPoint);
				renderer.Change(prevLast, movedPoint);
				adjustVertices[0] = movedPoint;
				adjustVertices[prevLast] = movedPoint;
			} else {
				renderer.Change(adjustIndex, movedPoint);
				adjustVertices[adjustIndex] = movedPoint;
			}

			renderer.ClearSubLine();
			IndicateMarkers(adjustVertices);
			adjusting = false;
		}

		/// <summary>
		/// 頂点の挿入
		/// </summary>
		private void InsertVertex(int index, Vector2 point) {
			renderer.Insert(index, point);
			StartVertexMove(renderer.GetVertices(), index);
		}

		/// <summary>
		/// 終了
		/// </summary>
		private void Exit() {
			if(adjusting) {
				adjusting = false;
			}
			HideMarkers();
			//コールバック
			onExit.Invoke(renderer.GetVertices());
		}

		#endregion

		#region Callback

		/// <summary>
		/// 頂点マーカーの押下
		/// </summary>
		private void OnVertexMarkerDown(GameObject gObj) {
			int index;
			if(!int.TryParse(gObj.name, out index)) return;
			//調整開始
			StartVertexMove(renderer.GetVertices(), index);
		}

		/// <summary>
		/// 中点マーカーの押下
		/// </summary>
		private void OnMidpointMarkerDown(GameObject gObj) {
			int index;
			if(!int.TryParse(gObj.name, out index)) return;
			//頂点の挿入
			InsertVertex(index, gObj.transform.localPosition);
		}

		#endregion
	}
}