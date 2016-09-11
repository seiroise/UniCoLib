using UnityEngine;
using Seiro.Scripts.Geometric;

/// <summary>
/// チャプター2のデモシーン
/// 交差判定の可視化
/// </summary>
public class Cap2Demo : MonoBehaviour {

	public Transform sa1, sa2;
	public LineRenderer aLine;
	private LineSegment segA;

	public Transform sb1, sb2;
	public LineRenderer bLine;
	private LineSegment segB;

	private void Start () {
		segA = new LineSegment (sa1.position, sa2.position);
		segB = new LineSegment (sb1.position, sb2.position);
	}

	private void Update () {
		//Update Segment
		segA.p1 = sa1.position;
		segA.p2 = sa2.position;
		segB.p1 = sb1.position;
		segB.p2 = sb2.position;

		//Update Intersection
		if (segA.Intersects (segB)) {
			aLine.SetColors (Color.red, Color.red);
			bLine.SetColors (Color.red, Color.red);
		} else {
			aLine.SetColors (Color.white, Color.white);
			bLine.SetColors (Color.white, Color.white);
		}

		//Draw Line
		aLine.SetVertexCount (2);
		aLine.SetPosition (0, segA.p1);
		aLine.SetPosition (1, segA.p2);
		bLine.SetVertexCount (2);
		bLine.SetPosition (0, segB.p1);
		bLine.SetPosition (1, segB.p2);
	}
}