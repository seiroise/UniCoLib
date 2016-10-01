using UnityEngine;
using System;
using Seiro.Scripts.Graphics;

namespace Seiro.Scripts.Geometric.Polygon {

	/// <summary>
	/// 凹多角形オブジェクト
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class ConcavePolygonObject : MonoBehaviour {

		private MeshFilter mf;
		private MeshCollider mc;

		//元ポリゴンデータ
		private ConcavePolygon origin;
		public ConcavePolygon Origin { get { return origin; } }

		private EasyMesh eMesh;
		public EasyMesh EMesh { get { return eMesh; } }

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
			this.origin = polygon;
			StartCoroutine(polygon.CoToEasyMesh(Color.white,OnUpdatePolygon, OnEndPolygon));
		}

		#endregion

		#region Callback

		/// <summary>
		/// ポリゴン生成中の更新コールバック
		/// </summary>
		private void OnUpdatePolygon(EasyMesh eMesh) {
			Mesh mesh = eMesh.ToMesh();
			mf.mesh = mesh;
		}

		/// <summary>
		/// ポリゴン生成時の終了コールバック
		/// </summary>
		private void OnEndPolygon(EasyMesh eMesh) {
			//mc.sharedMesh = mesh;
		}

		#endregion
	}
}