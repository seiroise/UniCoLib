using System;
using System.Collections.Generic;
using UnityEngine;

namespace Seiro.Scripts.Graphics.ChainLine.StandardUpdater {

	/// <summary>
	/// 軌跡のように線を更新する
	/// </summary>
	public class TrailUpdater : IChainLineUpdater{

		private Transform target;

		public TrailUpdater(Transform target) {
			this.target = target;
		}

		#region IChainLineUpdater

		public void Init(ChainLine owner) {
		}

		public void Update(List<Vertex> vertices) {
			//新しい頂点を追加する
			if(target) {
				vertices.Add(new Vertex(target.transform.position));
			}
		}

		#endregion
	}
}