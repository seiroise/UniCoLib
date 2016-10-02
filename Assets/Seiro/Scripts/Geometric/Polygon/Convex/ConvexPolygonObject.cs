using UnityEngine;
using System;

namespace Seiro.Scripts.Geometric.Polygon.Convex {

	/// <summary>
	/// 凸多角形オブジェクト
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class ConvexPolygonObject : MonoBehaviour {

		private MeshFilter meshFilter;
		private MeshCollider meshCollider;
		private Mesh mesh;

		//元ポリゴンデータ
		private ConvexPolygon origin;
		public ConvexPolygon Origin {
			get { return origin; }
			set {
				origin = value;
				if(setOriginCallback != null) setOriginCallback(value);
			}
		}
		private Action<ConvexPolygon> setOriginCallback;
		public Action<ConvexPolygon> SetOriginCallback {
			get { return setOriginCallback; }
			set { setOriginCallback = value; }
		}

		#region UnityEvent

		private void Awake() {
			meshFilter = GetComponent<MeshFilter>();
			meshCollider = GetComponent<MeshCollider>();
		}

		#endregion

		#region 

		/// <summary>
		/// 凸多角形の更新
		/// </summary>
		public void UpdatePolygon(ConvexPolygon polygon) {
			mesh = polygon.ToAltMesh();
			meshFilter.mesh = mesh;
			meshCollider.sharedMesh = mesh;
		}

		#endregion
	}
}