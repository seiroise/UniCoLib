using UnityEngine;
using System;

namespace Seiro.Scripts.Graphics.PolyLine2D.Snap {

	/// <summary>
	/// グリッド状に対してのスナップ処理
	/// </summary>
	public class GridSnap : BaseSnap{

		private Vector2 center;
		private float interval;	//グリッド間隔
	}
}