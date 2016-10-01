using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Utility;
using Seiro.Scripts.Geometric.Polygon;
using Seiro.Scripts.Graphics;

/// <summary>
/// ポリゴンの作成/調整/管理を行う
/// </summary>
public class PolygonEditor : MonoBehaviour {

	[Header("Maker")]
	public PolygonMaker maker;

	[Header("PolygonObject")]
	public Transform polyObjParent;					//多角形オブジェクトの親
	public ConcavePolygonObject polyObjPrefab;		//多角形オブジェクトのプレハブ
	private List<ConcavePolygonObject> polyObjList;	//多角形オブジェクトリスト

	[Header("Camera")]
	public Camera cam;

	[Header("UI")]
	public RadialMenu controlMenu;

	#region UnityEvent

	private void Awake() {
		//初期化
		polyObjList = new List<ConcavePolygonObject>();

		//各機能の無効化
		maker.DisableEditor();
	}

	private void Start() {
		//コールバックの設定
		//機能
		maker.endCallback = OnDrawEndPolygon;
		//UI
		controlMenu.AddClickCallback("Draw", OnDrawButtonClicked);
	}

	private void Update() {
		if(Input.GetMouseButtonDown(1)) {
			if(controlMenu != null) {
				Vector2 mPos = FuncBox.GetMousePosition(cam);
				controlMenu.Visible(mPos);
			}
		}
	}

	#endregion

	#region Function

	/// <summary>
	/// 多角形に追加
	/// </summary>
	private void AddPolygon(ConcavePolygon polygon) {
		//多角形オブジェクトの生成
		ConcavePolygonObject polyObj = InstantiatePolyObj();
		polyObj.SetPolygon(polygon);

		//追加
		polyObjList.Add(polyObj);
	}

	/// <summary>
	/// 多角形オブジェクトの生成
	/// </summary>
	private ConcavePolygonObject InstantiatePolyObj() {
		GameObject gObj = (GameObject)Instantiate(polyObjPrefab.gameObject, Vector3.zero, Quaternion.identity);
		gObj.transform.SetParent(polyObjParent);
		gObj.name = polyObjPrefab.name;
		return gObj.GetComponent<ConcavePolygonObject>();
	}

	#endregion

	#region EditorCallback

	/// <summary>
	/// ポリゴンの完成
	/// </summary>
	private void OnDrawEndPolygon(ConcavePolygon polygon) {
		//多角形オブジェクトとしてリストに追加
		AddPolygon(polygon);
	}

	#endregion

	#region UICallback

	/// <summary>
	/// 描画ボタンのクリック
	/// </summary>
	private void OnDrawButtonClicked(GameObject gObj) {
		maker.EnableEditor();
	}

	#endregion
}