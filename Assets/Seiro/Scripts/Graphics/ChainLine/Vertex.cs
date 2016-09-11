using UnityEngine;
using System;

namespace Seiro.Scripts.Graphics.ChainLine {

	/// <summary>
	/// 頂点
	/// </summary>
	[Serializable]
	public class Vertex {
		public Vector3 pos;     //頂点
		public Color color;     //色
		public float time;      //経過時間

		#region Constructors

		public Vertex(Vector3 pos) : this(pos, Color.white) { }

		public Vertex(Vector3 pos, Color color) {
			this.pos = pos;
			this.color = color;
			this.time = 0f;
		}

		#endregion
	}
}