using UnityEngine;
using System;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.PolyLine2D;
using Seiro.Scripts.Utility;
using Seiro.Scripts.Graphics;

/// <summary>
/// 線作成エディタ
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LineEditor : MonoBehaviour {

	[Header("Input Parameter")]
	public Camera cam;
	public int addButton = 0;       //追加ボタン番号
	public int removeButton = 1;    //削除ボタン番号

	[Header("Line Parameter")]
	public float width = 1f;
	public Color color = Color.white;

	[Header("Additional Line")]
	public AdditionalLineProponent additionalLine;

	//コールバック
	public Action<Vector2> addVertexCallback;
	public Action removeVertexCallback;

	//内部パラメータ
	private MeshFilter mf;
	private PolyLine2D polyLine;            //線
	public PolyLine2D PolyLine { get { return polyLine; } }
	private PolyLine2D noticeLine;          //予告線
	private const float MDEpsilon = 0.01f;  //マウス差分の検出閾値

	#region UnityEvent

	private void Awake() {
		mf = GetComponent<MeshFilter>();
		polyLine = new PolyLine2D(width, color);
		noticeLine = new PolyLine2D(width, color * 0.5f);
	}

	private void Update() {
		CheckInput();
	}

	#endregion

	#region Function

	/// <summary>
	/// 入力確認
	/// </summary>
	private void CheckInput() {

		if(cam == null) return;
		Vector3 mousePos = FuncBox.GetMousePosition(cam);

		//スナップ判定
		bool snaped = false;
		if(additionalLine != null) {
			Vector2 snapPos;
			if(additionalLine.Snap(mousePos, out snapPos)) {
				mousePos = snapPos;
				snaped = true;
			}
		}

		bool update = false;

		//頂点追加
		if(Input.GetMouseButtonDown(addButton)) {
			update |= AddVertex(mousePos);
			if(additionalLine != null && snaped) {
				additionalLine.SnapCallback();
			}
		}

		//頂点削除
		if(Input.GetMouseButtonDown(removeButton)) {
			update |= RemoveAtLastVertex();
		}

		//マウスの移動量
		Vector2 md = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		if(md.magnitude > MDEpsilon) {
			update |= UpdateNoticeLine(mousePos);
		}

		if(update) {
			Draw();
		}
	}

	/// <summary>
	/// 描画
	/// </summary>
	private void Draw() {
		EasyMesh[] eMeshes = new EasyMesh[2];
		eMeshes[0] = polyLine.Cache;
		eMeshes[1] = noticeLine.Cache;
		mf.mesh = EasyMesh.ToMesh(eMeshes);
	}

	/// <summary>
	/// 頂点の追加
	/// </summary>
	private bool AddVertex(Vector2 point) {

		polyLine.AddVertex(point);

		//予告線の基点を変更
		if(noticeLine.GetVertexCount() == 0) {
			noticeLine.AddVertex(point);
		} else {
			noticeLine.ChangeVertex(0, point);
		}

		//コールバック
		if(addVertexCallback != null) {
			addVertexCallback(point);
		}

		return true;
	}

	/// <summary>
	/// 頂点の削除
	/// </summary>
	private bool RemoveVertex(int index) {

		int size = polyLine.GetVertexCount();
		if(size == 0) return false;

		polyLine.RemoveVertex(index);
		--size;

		//予告線と補助線の変更
		if(size == 0) {
			noticeLine.Clear();
		} else {
			Vector2 point;
			polyLine.GetVertex(size - 1, out point);
			noticeLine.ChangeVertex(0, point);
		}

		//コールバック
		if(removeVertexCallback != null) {
			removeVertexCallback();
		}

		return true;
	}

	/// <summary>
	/// 最後の頂点を削除する
	/// </summary>
	private bool RemoveAtLastVertex() {
		return RemoveVertex(polyLine.GetVertexCount() - 1);
	}

	/// <summary>
	/// 予告線の更新
	/// </summary>
	private bool UpdateNoticeLine(Vector2 point) {

		int count = noticeLine.GetVertexCount();
		if(count <= 0) {
			noticeLine.Clear();
		} else if(count == 1) {
			noticeLine.AddVertex(point);
		} else {
			noticeLine.ChangeVertex(1, point);
		}

		return true;
	}

	/// <summary>
	/// 溜まっている頂点リストを取得して消去、再描画
	/// </summary>
	public List<Vector2> FlushVertices() {
		List<Vector2> temp = polyLine.GetVertices();
		List<Vector2> vertices = new List<Vector2>();
		for(int i = 0; i < temp.Count; ++i) {
			vertices.Add(temp[i]);
		}
		//消去
		polyLine.Clear();
		noticeLine.Clear();
		//描画
		Draw();
		return vertices;
	}

	#endregion
}