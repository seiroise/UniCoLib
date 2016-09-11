using UnityEngine;
using System;
using System.Collections.Generic;

namespace Seiro.Scripts.Graphics.ChainLine.StandardUpdater {

	/// <summary>
	/// 頂点に寿命を設ける
	/// </summary>
	public class LiveTimeUpdater : IChainLineUpdater {

		private float liveTime;
		public float LiveTime { get { return liveTime; } }

		public LiveTimeUpdater(float liveTime) {
			this.liveTime = liveTime;
		}

		#region IChainLineUpdater

		public void Init(ChainLine orner) {
		}

		public void Update(List<Vertex> vertices) {
			//寿命を過ぎた頂点を削除する
			for(int i = vertices.Count - 1; i >= 0; --i) {
				if(vertices[i].time >= liveTime) vertices.RemoveAt(i);
			}
		}

		#endregion
	}
}