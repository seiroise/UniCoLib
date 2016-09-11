using UnityEngine;
using System;
using System.Collections.Generic;

namespace Seiro.Scripts.Graphics.ChainLine.StandardUpdater {

	/// <summary>
	/// 頂点に寿命を設ける
	/// </summary>
	public class LiveTimeUpdater : IChainLineUpdater {

		private ChainLine owner;
		private float liveTime;
		public float LiveTime { get { return liveTime; } }
		private bool vertsZeroWithDeath;

		public LiveTimeUpdater(float liveTime, bool vertsZeroWithDeath = false) {
			this.liveTime = liveTime;
			this.vertsZeroWithDeath = vertsZeroWithDeath;
		}

		#region IChainLineUpdater

		public void Init(ChainLine owner) {
			this.owner = owner;
		}

		public void Update(List<Vertex> vertices) {
			//寿命を過ぎた頂点を削除する
			for(int i = vertices.Count - 1; i >= 0; --i) {
				if(vertices[i].time >= liveTime) vertices.RemoveAt(i);
			}

			//全ての頂点が寿命を迎えた場合
			if(vertsZeroWithDeath && vertices.Count == 0) {
				owner.VertsZeroWithDeath = true;
			}
		}

		#endregion
	}
}