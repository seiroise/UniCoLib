using UnityEngine;

/// <summary>
/// ボロノイサイトオブジェクト
/// </summary>
public class WaitedSiteObject : MonoBehaviour {

	[Header("Parameter")]
	[SerializeField, Range(0.1f, 10f)]
	private float wait = 1f;
	public float Wait { get { return wait; } }
}