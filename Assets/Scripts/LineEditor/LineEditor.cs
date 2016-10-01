using UnityEngine;
using System;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.PolyLine2D;
using Seiro.Scripts.Utility;
using Seiro.Scripts.Graphics;
using Seiro.Scripts.Geometric;

/// <summary>
/// 線作成エディタ
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LineEditor : MonoBehaviour {

	[Header("Input Parameter")]
	public Camera cam;
	public int addButton = 0;                       //追加ボタン番号
	public int removeButton = 1;                    //削除ボタン番号
	private const float MDEPSILON = 0.01f;          //マウス差分の検出閾値
	public KeyCode clearButton = KeyCode.Escape;    //全消し

	[Header("Line Parameter")]
	public float width = 1f;
	public Color color = Color.white;

	[Header("Additional Line")]
	public AdditionalLineProponent additionalLine;

	[Header("Add Animation")]
	public float lerpT = 10f;
	private float nowDistance = 0f;
	private float targetDistance = 0f;
	private const float LEPSILON = 0.1f;    //追加時アニメーション補間閾値
	private bool lerped = false;

	[Header("Remove Effect")]
	public ParticleSystem removeParticle;
	public float density = 1f;
	public float speed = 0.5f;

	[Header("Exception Removal")]
	public bool doublePointRemoval = false;     //連続同一頂点の除去
	public float doublePointThreshold = 0.05f;  //連続同一点の認識閾値
	public bool crossLineRemoval = false;       //交差線分の除去

	//コールバック
	public Action<Vector2> addVertexCallback;	//追加
	public Action removeVertexCallback;			//削除
	public Action doublePointCallback;			//連続同一点
	public Action crossLineCallback;			//交差線分

	//内部パラメータ
	private MeshFilter mf;
	private PolyLine2D polyLine;            //線
	public PolyLine2D PolyLine { get { return polyLine; } }
	private PolyLine2D noticeLine;          //予告線
	private bool draw = false;              //描画フラグ

	#region UnityEvent

	private void Awake() {
		mf = GetComponent<MeshFilter>();
		polyLine = new PolyLine2D();
		noticeLine = new PolyLine2D();
	}

	private void Update() {
		CheckInput();
		UpdateAnimation();

		Draw();
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
				draw = true;
			}
		}

		//頂点追加
		if(Input.GetMouseButtonDown(addButton)) {
			draw |= AddVertex(mousePos);
			if(additionalLine != null && snaped) {
				additionalLine.SnapCallback();
			}
		}

		//頂点削除
		if(Input.GetMouseButtonDown(removeButton)) {
			draw |= RemoveAtLastVertex();
		}

		//マウスの移動量
		Vector2 md = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		if(md.magnitude > MDEPSILON) {
			draw |= UpdateNoticeLine(mousePos);
		}

		//全消し
		if(Input.GetKeyDown(clearButton)) {
			FlushVertices();
		}
	}

	/// <summary>
	/// 補間アニメーションの更新
	/// </summary>
	private void UpdateAnimation() {
		if(!lerped) return;
		nowDistance = Mathf.Lerp(nowDistance, targetDistance, lerpT * Time.deltaTime);
		if(Mathf.Abs(targetDistance - nowDistance) < LEPSILON) {
			nowDistance = targetDistance;
			lerped = false;
		}
		//描画
		draw = true;
	}

	/// <summary>
	/// 描画
	/// </summary>
	private void Draw() {
		if(!draw) return;
		EasyMesh[] eMeshes = new EasyMesh[2];
		eMeshes[0] = polyLine.MakeSubLineRange(0f, nowDistance, width, color);
		eMeshes[1] = noticeLine.MakeLine(width, color);
		mf.mesh = EasyMesh.ToMesh(eMeshes);
		draw = false;
	}

	/// <summary>
	/// 頂点の追加
	/// </summary>
	private bool AddVertex(Vector2 point) {

		//例外検出
		if(ExceptionDetector(point)) {
			return false;
		}

		polyLine.Add(point);

		//予告線の基点を変更
		if(noticeLine.GetVertexCount() == 0) {
			noticeLine.Add(point);
		} else {
			noticeLine.Change(0, point);
		}

		//アニメーションの距離更新
		targetDistance = polyLine.TotalDistance;
		lerped = true;

		//コールバック
		if(addVertexCallback != null) {
			addVertexCallback(point);
		}

		return true;
	}

	/// <summary>
	/// 頂点追加前の例外検出。例外を検出した場合はtrueを返す
	/// </summary>
	private bool ExceptionDetector(Vector2 point) {
		int count = polyLine.GetVertexCount();

		//同一点の検出
		if(doublePointRemoval) {
			if(count > 1) {
				Vector2 prevPoint = polyLine.GetVertex(count - 1);
				float dis = (point - prevPoint).magnitude;
				if(dis < doublePointThreshold) {
					if(doublePointCallback != null) doublePointCallback();
					return true;
				}
			}
		}

		//線分の交差判定
		if(crossLineRemoval) {
			if(count > 2) {
				//検出線分の作成
				LineSegment line = new LineSegment(polyLine.GetVertex(count - 1), point);
				for(int i = 0; i < count - 2; ++i) {
					//比較線分の作成
					LineSegment sample = new LineSegment(polyLine.GetVertex(i), polyLine.GetVertex(i + 1));
					if(line.Intersects(sample)) {
						if(crossLineCallback != null) crossLineCallback();
						return true;
					}
				}
			}
		}

		return false;
	}

	/// <summary>
	/// 頂点の削除
	/// </summary>
	private bool RemoveVertex(int index) {

		int size = polyLine.GetVertexCount();
		if(size == 0) return false;

		Vector2 p1 = polyLine.GetVertex(index);
		polyLine.Remove(index);
		--size;

		//予告線と補助線の変更
		if(size == 0) {
			noticeLine.Clear();
		} else {
			Vector2 point = polyLine.GetVertex(size - 1);
			noticeLine.Change(0, point);

			//エフェクトの生成
			EmitRemoveEffect(point, p1, density);
		}

		//アニメーションの距離更新
		targetDistance = polyLine.TotalDistance;
		lerped = true;

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
			noticeLine.Add(point);
		} else {
			noticeLine.Change(1, point);
		}

		return true;
	}

	/// <summary>
	/// 削除エフェクトの生成
	/// </summary>
	private void EmitRemoveEffect(Vector2 p1, Vector2 p2, float density) {
		if(!removeParticle) return;
		//方向と距離の取得
		Vector2 dir = p2 - p1;
		float dis = dir.magnitude;
		Debug.Log(dis);
		dir.Normalize();

		//エフェクトの生成
		float tempDis = 0f;
		do {
			removeParticle.Emit(
				p1 + dir * tempDis,                             //座標
				FuncBox.RandomVector2() * speed,                //速度
				UnityEngine.Random.Range(width * 0.8f, width),  //大きさ
				UnityEngine.Random.Range(0.2f, 1f),             //時間
				color                                           //色
			);
			tempDis += density;
		} while(tempDis < dis);

		removeParticle.Emit(
			p1 + dir * dis,                                 //座標
			FuncBox.RandomVector2() * speed,                //速度
			UnityEngine.Random.Range(width * 0.8f, width),  //大きさ
			UnityEngine.Random.Range(0.2f, 1f),             //時間
			color                                           //色
		);

	}

	/// <summary>
	/// 溜まっている頂点リストを取得して消去、再描画
	/// </summary>
	public List<Vector2> FlushVertices() {
		List<Vector2> verts = polyLine.GetVertices();

		//エフェクト
		if(removeParticle) {
			for(int i = 0; i < verts.Count - 1; ++i) {
				EmitRemoveEffect(verts[i], verts[i + 1], density);
			}
		}

		//消去
		polyLine.Clear();
		noticeLine.Clear();

		//コールバック
		if(removeVertexCallback != null) {
			removeVertexCallback();
		}

		//描画
		draw = true;

		return verts;
	}

	#endregion
}