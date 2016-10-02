using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Seiro.Scripts.Graphics.ChainLine;
using Seiro.Scripts.Graphics.ChainLine.StandardUpdater;
using Seiro.Scripts.Graphics.PerlinNoise;

/// <summary>
/// チャプター2
/// パーリンノイズとボロノイ図でダンジョン生成
/// </summary>
public class Chap1_PerlinNoiseD1_Demo : MonoBehaviour {

	[Header("Visualize")]
	public ChainLineFactory lineFactory;
	public float liveTime = 1f;
	public Gradient gradient;

	[Header("Noise Option")]
	[Range(2, 16)]
	public int ctrlCount = 4;
	[Range(2, 16)]
	public int noiseScale = 8;
	[Range(1f, 30f)]
	public float amplitude = 10f;

	private List<ChainLine> lines;

	#region UnityEvent

	private void Start() {
		lines = new List<ChainLine>();
	}

	private void FixedUpdate() {

		for(int i = 0; i < 2; ++i) {
			int octave = i;
			float[] noise = PerlinNoiseD1.Noise(ctrlCount, noiseScale, octave);

			ChainLine line = lineFactory.CreateLine(null, new LiveTimeUpdater(liveTime, true), new LiveTimeGradientUpdater(gradient));
			for(int j = 0; j < noise.Length; ++j) {
				line.AddVertex(new Vector3(j / (float)noise.Length * 100f, noise[j] * amplitude + (i * amplitude)));
			}
		}
	}

	#endregion

}