using UnityEngine;
using Seiro.Scripts.Geometric.Polygon.Convex;

namespace Seiro.Scripts.Geometric.Diagram.Voronoi {

	/// <summary>
	/// ボロノイサイト(母点用オブジェクト)
	/// </summary>
	[RequireComponent(typeof(ConvexPolygonObject), typeof(ConvexPolygonButton), typeof(TransformDetector))]
	public class VoronoiSite : MonoBehaviour {

		private ConvexPolygonObject polygonObject;
		public ConvexPolygonObject PolygonObject { get { return polygonObject; } }
		private ConvexPolygonButton polygonButton;
		public ConvexPolygonButton PolygonButton { get { return polygonButton; } }
		private TransformDetector transformDetector;
		public TransformDetector TransformDetector { get { return transformDetector; } }

		#region UnityEvent

		private void Awake() {
			polygonObject = GetComponent<ConvexPolygonObject>();
			polygonButton = GetComponent<ConvexPolygonButton>();
			transformDetector = GetComponent<TransformDetector>();
		}

		#endregion
	}
}