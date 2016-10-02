using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Seiro.Scripts.Graphics.PolyLine2D {

	/// <summary>
	/// 終了イベント
	/// </summary>
	[Serializable]
	public class ExitEvent : UnityEvent<List<Vector2>> { }
}