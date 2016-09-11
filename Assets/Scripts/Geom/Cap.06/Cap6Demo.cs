using UnityEngine;
using System.Collections.Generic;
using Seiro.Scripts.Geometric;

/// <summary>
/// チャプター6のデモシーン
/// 多角形の凸性判定の可視化
/// </summary>
public class Cap6Demo : MonoBehaviour {

	public LineRenderer line;
	public List<Transform> vertices;

	#region UnityEvent

	private void Update () {
		//角数が3未満なら何もしない
		if (vertices.Count < 3) return;

		//線の更新
		UpdateLine ();
	}

	#endregion

	#region Function

	private void UpdateLine () {
		if (line == null) return;

		int size = vertices.Count;
		if (size < 3) return;

		line.SetVertexCount (vertices.Count + 1);

		float baseCCW = GeomUtil.CCW (vertices [0].position, vertices [1].position, vertices [2].position);
		bool notConvex = false;

		for (int i = 0; i < size; ++i) {
			line.SetPosition (i, vertices [i].position);

			//凸性判定
			Vector2 p1 = vertices [i].position;
			Vector2 p2 = vertices [(i + 1) % size].position;
			Vector2 p3 = vertices [(i + 2) % size].position;
			float ccw = GeomUtil.CCW (p1, p2, p3);
			if (baseCCW * ccw <= 0) {
				notConvex = true;
			}
		}
		line.SetPosition (size, vertices [0].position);

		if (notConvex) {
			line.SetColors (Color.red, Color.red);
		} else {
			line.SetColors (Color.white, Color.white);
		}
	}

	#endregion
}