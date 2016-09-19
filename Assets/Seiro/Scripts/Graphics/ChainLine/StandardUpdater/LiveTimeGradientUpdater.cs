using UnityEngine;
using System;
using System.Collections.Generic;

namespace Seiro.Scripts.Graphics.ChainLine.StandardUpdater {
	public class LiveTimeGradientUpdater : IChainLineUpdater {

		private Gradient gradient;
		private LiveTimeUpdater liveTimeUpdater;

		public LiveTimeGradientUpdater(Gradient gradient) {
			this.gradient = gradient;
		}

		#region IChainLineUpdater

		public void Init(ChainLine owner) {
			liveTimeUpdater = (LiveTimeUpdater)owner.GetUpdater<LiveTimeUpdater>();
		}

		public void Update(List<Vertex> vertices) {
			float liveTime = liveTimeUpdater.LiveTime;
			//頂点の色を変更する
			for(int i = 0; i < vertices.Count; ++i) {
				vertices[i].color = 
					gradient.Evaluate(vertices[i].time / liveTime);
			}
		}

		#endregion
	}
}