using UnityEngine;
using Seiro.Scripts.Geometric;

/// <summary>
/// チャプター2.1のデモシーン
/// 外積判定の可視化
/// </summary>
public class Cap2_1Demo : MonoBehaviour {

	public Transform p1, p2, p3;
	public LineRenderer line;

	private void Update () {
		float ccw = GeomUtil.CCW (p1.position, p2.position, p3.position);

		//Line Color
		Color c = Color.white;
		if (ccw < 0) {
			c = Color.blue;
		} else if (ccw > 0) {
			c = Color.red;
		}
		line.SetColors (c, c);

		//Draw Line
		line.SetVertexCount (3);
		line.SetPosition (0, p1.position);
		line.SetPosition (1, p2.position);
		line.SetPosition (2, p3.position);
	}
}