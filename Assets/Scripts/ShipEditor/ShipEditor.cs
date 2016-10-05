using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.PolyLine2D;
using Seiro.Scripts.Geometric.Polygon.Concave;
using Seiro.Scripts.Graphics;
using Seiro.Scripts.EventSystems.Interface.Circle;

/// <summary>
/// 機体エディタ
/// </summary>
public class ShipEditor : MonoBehaviour {

	[Header("LineEditor")]
	public PolyLine2DEditor lineEditor;

	[Header("PolyObject")]
	public ConcavePolygonObject polyObjPrefab;
	private Dictionary<GameObject, ConcavePolygonObject> polyObjDic;

	[Header("Marker")]
	public SUICirclePool markerPool;

	[Header("UI")]
	public Button drawButton;
	public MeshImage meshImage;

	[Header("Renderer")]
	public MeshFilter mf;

	#region UnityEvent

	private void Awake() {
		polyObjDic = new Dictionary<GameObject, ConcavePolygonObject>();
	}

	private void Start() {
		lineEditor.onMakerExit.AddListener(OnMakerExit);
		drawButton.onClick.AddListener(OnDrawButtonClicked);
	}

	#endregion

	#region Callback

	/// <summary>
	/// LineEditorのMaker終了イベント
	/// </summary>
	private void OnMakerExit(List<Vector2> vertices) {
		if(vertices == null) return;
		if(vertices.Count < 4) return;
		vertices.RemoveAt(vertices.Count - 1);
		//とりまテスト
		ConcavePolygonObject polyObj = Instantiate<ConcavePolygonObject>(polyObjPrefab);
		polyObj.name = polyObj.name;
		ConcavePolygon polygon = new ConcavePolygon(vertices);
		//ランチャーの解析
		PartsPolygon pPoly = new PartsPolygon(polygon);
		List<Vector2> launchers = pPoly.ParseLauncher();
		for(int i = 0; i < launchers.Count; ++i) {
			markerPool.PopItem(launchers[i]).Visible();
		}
		//メッシュ化
		meshImage.SetMesh(polygon.ToEasyMesh(Color.white).ToMesh());
		polyObj.SetPolygon(polygon);
		polyObj.onClick.AddListener(OnPolygonClick);
		polyObjDic.Add(polyObj.gameObject, polyObj);
	}

	/// <summary>
	/// ポリゴンオブジェクトをクリック
	/// </summary>
	/// <param name="gObj">G object.</param>
	private void OnPolygonClick(GameObject gObj) {
		//とりまテスト
		if(!polyObjDic.ContainsKey(gObj)) return;
		ConcavePolygonObject polyObj = polyObjDic[gObj];
		List<Vector2> vertices = new List<Vector2>();
		foreach(var e in polyObj.EMesh.verts) {
			vertices.Add(e);
		}
		vertices.Add(vertices[0]);
		lineEditor.EnableAdjuster(vertices, true);
	}

	/// <summary>
	/// 描画ボタンのクリック
	/// </summary>
	private void OnDrawButtonClicked() {
		lineEditor.EnableMaker();
	}

	#endregion
}