using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.ChainLine;

public class Cap17Demo : MonoBehaviour {

	[Header("File")]
	public string filename;
	public char separator = ',';
	[Header("Visualize")]
	public ChainLineFactory lineFactory;
	private ChainLine chainLine;
	public float liveTime = 2f;
	public Gradient gradient = new Gradient();
	public Color subColor = Color.white;

	private void Start() {
		StartCoroutine(Co());
	}

	private IEnumerator Co() {
		string fullPath = Application.streamingAssetsPath + "/" + filename;
		List<Vector3> vertices = new List<Vector3>();

		using (TextReader r = new StreamReader(fullPath)) {
			ChainLine mainLine = lineFactory.CreateLine(null, liveTime, gradient);
			ChainLine subLine = lineFactory.CreateLine(null, subColor);

			string line = null;
			string[] result;

			float max = 16777215f;

			while((line = r.ReadLine()) != null) {
				//頂点
				result = line.Split(separator);
				Vector3 pos = new Vector3(
					ValueAdjust(float.Parse(result[0]), max), 
					ValueAdjust(float.Parse(result[1]), max),
					ValueAdjust(float.Parse(result[2]), max)) * 0.00001f;
				
				//線二頂点を追加
				mainLine.AddVertex(pos);
				subLine.AddVertex(pos, subColor);
				yield return 0f;
			}
		}
	}

	/// <summary>
	/// 数値の範囲調整
	/// </summary>
	private float ValueAdjust(float v, float max) {
		if(v + 2000000f > max) {
			v -= max;
		}
		return v;
	}
}