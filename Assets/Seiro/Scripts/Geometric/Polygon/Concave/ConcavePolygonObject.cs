using UnityEngine;
using UnityEngine.Events;
using System;
using Seiro.Scripts.Graphics;
using Seiro.Scripts.EventSystems;

namespace Seiro.Scripts.Geometric.Polygon.Concave {

	/// <summary>
	/// 凹多角形オブジェクト
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class ConcavePolygonObject : MonoBehaviour, ICollisionEventHandler {

		[Serializable]
		public class ClickEvent : UnityEvent<GameObject> {};

		private MeshFilter mf;
		private MeshCollider mc;

		//元ポリゴンデータ
		private ConcavePolygon origin;
		public ConcavePolygon Origin { get { return origin; } }

		private EasyMesh eMesh;
		public EasyMesh EMesh { get { return eMesh; } }

		[Header("Callback")]
		public ClickEvent onClick;

		#region UnityEvent

		private void Awake() {
			mf = GetComponent<MeshFilter>();
			mc = GetComponent<MeshCollider>();
		}

		#endregion

		#region Function

		/// <summary>
		/// 多角形の設定
		/// </summary>
		public void SetPolygon(ConcavePolygon polygon) {
			origin = polygon;
			eMesh = polygon.ToEasyMesh(Color.white);
			Mesh mesh = eMesh.ToMesh();
			mf.mesh = mesh;
			if(mc) {
				mc.sharedMesh = mesh;
			}
		}

		#endregion

		#region Interface

		public void OnPointerEnter(RaycastHit hit) {}

		public void OnPointerExit(RaycastHit hit) {}

		public void OnPointerDown(RaycastHit hit) {}

		public void OnPointerUp(RaycastHit hit) {}

		public void OnPointerClick(RaycastHit hit) {
			onClick.Invoke(gameObject);
		}

		#endregion
	}
}