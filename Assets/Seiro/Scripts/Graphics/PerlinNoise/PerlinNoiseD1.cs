using UnityEngine;

namespace Seiro.Scripts.Graphics.PerlinNoise {

	/// <summary>
	/// 一次元パーリンノイズ
	/// </summary>
	public class PerlinNoiseD1 {

		#region Static Function

		/// <summary>
		/// ノイズの生成
		/// </summary>
		public static float[] Noise(int ctrlCount, int noiseScaling, float amplitude) {

			float[] gradients;
			float[] noise;

			//勾配
			gradients = new float[ctrlCount];
			for(int i = 0; i < ctrlCount; ++i) {
				gradients[i] = Random.Range(-1f, 1f);
			}
			//ノイズ生成
			int noiseSize = (ctrlCount - 1) * noiseScaling;
			noise = new float[noiseSize];
			for(int t = 0; t < noiseSize; ++t) {
				float x = (float)t / noiseSize * (ctrlCount - 1);
				int i0 = t / noiseScaling;
				int i1 = i0 + 1;
				float g0 = gradients[i0];
				float g1 = gradients[i1];

				float u0 = x - i0;
				float u1 = u0 - 1;
				float n0 = g0 * u0;
				float n1 = g1 * u1;

				float nx = n0 * (1 - F(u0)) + n1 * F(u0);
				//振幅をかけた値が最終的な値
				noise[t] = nx * amplitude;
			}

			return noise;
		}

		/// <summary>
		/// オクターブノイズの生成
		/// </summary>
		public static float[] Noise(int ctrlCount, int noiseScaling, int octave) {

			//重ね合わせた波形
			float amplitude = 1f;
			float[] sum = Noise(ctrlCount, noiseScaling, amplitude);
			int nextCtrlCount = ctrlCount;

			//波形の重ね合わせ処理
			for(int i = 0; i < octave; ++i) {
				nextCtrlCount *= 2;
				amplitude *= 0.5f;
				float[] noise = Noise(nextCtrlCount, noiseScaling, amplitude);

				//波形の重ね合わせ処理
				for(int j = 0; j < sum.Length; ++j) {
					noise[j * 2 + 0] += sum[j];
					noise[j * 2 + 1] += sum[j];
				}
				sum = noise;
			}
			return sum;
		}

		/// <summary>
		/// パーリンノイズ補間用五次多項式
		/// </summary>
		private static float F(float t) {
			return (t * t * t)* (t * (6 * t - 15) + 10);
		}

		#endregion
	}
}