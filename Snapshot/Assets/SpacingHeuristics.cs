using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp {
	public class SpacingHeuristics {
		public SpacingHeuristics () {}

		public static float testHeuristic(List<GameObject> visibleObjects) {
			return 1f;
		}

		public static float newHueristic(List<GameObject> visibleObjects) {
			return 1000f;
		}
	}
}

