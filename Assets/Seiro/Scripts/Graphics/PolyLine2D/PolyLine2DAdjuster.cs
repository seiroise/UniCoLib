using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.EventSystems.Interface;

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
		public SUICircle markerPrefab;
		public Transform markerParent;
		public Color adjust;
		public Color divide;
		private List<SUICircle> markers;
		private const float EPSILON = 0.01f;

		//ModuleComponent
		private PolyLine2DRenderer renderer;	//描画担当
		private PolyLine2DSupporter supporter;	//補助担当

		//VertexMove
		private List<Vector2> vertices;	//頂点
		private int vertsCount;			//頂点数
		private int selectIndex;		//選択番号
		private bool adjusting = false;	//調整中

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
		/// 調整する頂点群を設定する
		/// </summary>
		public void SetVertices(List<Vector2> vertices) {
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
			for(int i = 0; i < count; ++i) {
				//調整用マーカー
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
			GameObject gObj = (GameObject)Instantiate(markerPrefab.gameObject, point, Quaternion.identity);
			gObj.transform.SetParent(markerParent);
			return gObj.GetComponent<SUICircle>();
		}

		/// <summary>
		/// 頂点調整用マーカーの生成
		/// </summary>
		private SUICircle InstantiateVertexMarker(string markerName, Vector2 point) {
			SUICircle marker = InstantiateMarker(point);
			marker.name = markerName;
			marker.Visible();
			marker.onClick.AddListener(OnVertexMarkerClicked);
			return marker;
		}

		/// <summary>
		/// 入力確認
		/// </summary>
		private void InputCheck() {
			if(adjusting) {
				Vector2 mDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
				if(mDelta.magnitude > EPSILON) {
					SetNoticeLine();
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
			Vector2 p1 = vertices[(selectIndex + vertsCount - 1) % vertsCount];
			Vector2 p2 = vertices[(selectIndex + 1) % vertsCount];

			//補助線の描画
			renderer.SetSubVertices(p1, mPoint, p2);
		}

		#endregion

		#region Callback

		/// <summary>
		/// 頂点マーカーのクリック
		/// </summary>
		private void OnVertexMarkerClicked(GameObject gObj) {
			if(!int.TryParse(gObj.name, out selectIndex)) return;
			vertices = renderer.GetVertices();
			vertsCount = vertices.Count;
			HideMarkers();
			adjusting = true;
		}

		#endregion
	}
}