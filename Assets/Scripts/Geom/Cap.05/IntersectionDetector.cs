using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Seiro.Scripts.Geometric;

public interface IntersectionDetector {
	ICollection<Intersection> Execute (List<LineSegment> segments);
}