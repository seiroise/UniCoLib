using UnityEngine;
using Seiro.Scripts.EventSystems;
using Seiro.Scripts.Geometric.Polygon.Convex;
using Seiro.Scripts.Geometric.Polygon.Operation;

/// <summary>
/// 凸多角形をボタンにしてみる
/// </summary>
[RequireComponent(typeof(ConvexPolygonObject))]
public class ConvexPolygonButton : MonoBehaviour, ICollisionEventHandler {

	private ConvexPolygonObject polygonObject;
	private bool hover = false;

	[Header("Animation")]
	public float duration = 0.5f;
	[Header("Scale")]
	public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	[Header("Subdivision")]
	public AnimationCurve divisionCurve = AnimationCurve.Linear(0f, 5f, 1f, 0f);
	public AnimationCurve tCurve = AnimationCurve.EaseInOut(0f, 0.7f, 1f, 0.3f);

	private float timer = 0f;

	#region UnityEvent

	private void Awake() {
		polygonObject = GetComponent<ConvexPolygonObject>();
		polygonObject.SetOriginCallback = OnSetOrigin;
	}

	private void Update() {
		if(hover) {
			if(timer < duration) {
				timer += Time.deltaTime;
				//ポリゴンの更新
				Evaluate(timer / duration);
			}
		} else {
			if(timer > 0f) {
				timer -= Time.deltaTime;
				//ポリゴンの更新
				Evaluate(timer / duration);
			}
		}

	}

	#endregion

	#region Function

	/// <summary>
	/// スケールや再分割度の設定
	/// </summary>
	private void Evaluate(float par) {
		float scale = scaleCurve.Evaluate(par);
		int division = (int)divisionCurve.Evaluate(par);
		float t = tCurve.Evaluate(par);
		ConvexPolygon p = polygonObject.Origin.Scaled(transform.position, scale);
		//polygonObject.UpdatePolygon(AngleSubdivisionOperation.Execute(p, division));
		polygonObject.UpdatePolygon(LerpSubdivisionOperation.Execute(p, division, t));
	}

	#endregion

	#region Callback

	/// <summary>
	/// PolygonObjectに元ポリゴンが設定された時に呼ばれる
	/// </summary>
	private void OnSetOrigin(ConvexPolygon origin) {
		Evaluate(0f);
	}

	#endregion

	#region ICollisionEventHandler

	public void OnPointerEnter(RaycastHit hit) {
		hover = true;
	}

	public void OnPointerExit(RaycastHit hit) {
		hover = false;
	}

	public void OnPointerClick(RaycastHit hit) {
		Debug.Log("Click");
	}

	public void OnPointerDown(RaycastHit hit) {
		
	}

	public void OnPointerUp(RaycastHit hit) {
		
	}

	#endregion
}