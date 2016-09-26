using UnityEngine;
using System.Collections;
using Seiro.Scripts.Graphics;

/// <summary>
/// ある座標に対してスナップ処理を行う基底クラス
/// </summary>
public abstract class BaseSnap {

	protected float snapForce;

	public BaseSnap(float snapForce) {
		this.snapForce = snapForce;
	}

	public abstract bool Snap(Vector2 input, out Vector2 output);

	public abstract EasyMesh GetEasyMesh(Color color);
}