using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.PolyLine2D;
using Seiro.Scripts.Geometric.Polygon.Concave;
using Seiro.Scripts.Graphics;

/// <summary>
/// 機体エディタ
/// </summary>
public class ShipEditor : MonoBehaviour {

	[Header("LineEditor")]
	public PolyLine2DEditor lineEditor;

	[Header("UI")]
	public Button drawButton;

	[Header("Renderer")]
	public MeshFilter mf;

	private List<EasyMesh> eMeshes;

	#region UnityEvent

	private void Awake() {
		eMeshes = new List<EasyMesh>();
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
		Debug.Log("OnMarkerExit");
		//とりまテスト
		ConcavePolygon poly = new ConcavePolygon(vertices);
		EasyMesh eMesh = poly.ToEasyMesh(Color.gray);
		eMeshes.Add(eMesh);
		mf.mesh = EasyMesh.ToMesh(eMeshes.ToArray());
	}

	/// <summary>
	/// 描画ボタンのクリック
	/// </summary>
	private void OnDrawButtonClicked() {
		lineEditor.EnableMaker();
	}

	#endregion
}