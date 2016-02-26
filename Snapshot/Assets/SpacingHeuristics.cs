using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp {
	public class SpacingHeuristics {
		public SpacingHeuristics () {}

		/*
		 * One two, one two this is just a test
		 */
		public static float testHeuristic(List<GameObject> visibleObjects, Camera cam) {
			return 1f;
		}

		/*
		 * Considering the following 3 x 3 rule of thirds grid:
		 *  0 | 1 | 2 
		 * -----------
		 *  3 | 4 | 5 
		 * -----------
		 *  6 | 7 | 8 
		 * 
		 *  Higher scoring images have average or better densities of objects in sector 4.
		 */ 
		public static float avoidsEmptyCenters(List<GameObject> visibleObjects, Camera cam) {
			// First, get average object density for each sector
			List<float> sectorDensity = new List<float>(new float[9]);

			// The value of sectorBoundaries is a quartet of floats such that:
			// value[0] and value[1] are the min and max x values
			// value[2] and value[3] are the min and max y values
			Dictionary<int, List<float>> sectorBoundaries = new Dictionary<int, List<float>> ();
			sectorBoundaries.Add (0, new List<float> (){0.000f, 0.333f, 0.666f, 1.000f});
			sectorBoundaries.Add (1, new List<float> (){0.333f, 0.666f, 0.666f, 1.000f});
			sectorBoundaries.Add (2, new List<float> (){0.666f, 1.000f, 0.666f, 1.000f});
			sectorBoundaries.Add (3, new List<float> (){0.000f, 0.333f, 0.333f, 0.666f});
			sectorBoundaries.Add (4, new List<float> (){0.333f, 0.666f, 0.333f, 0.666f});
			sectorBoundaries.Add (5, new List<float> (){0.666f, 1.000f, 0.333f, 0.666f});
			sectorBoundaries.Add (6, new List<float> (){0.000f, 0.333f, 0.000f, 0.333f});
			sectorBoundaries.Add (7, new List<float> (){0.333f, 0.666f, 0.000f, 0.333f});
			sectorBoundaries.Add (8, new List<float> (){0.666f, 1.000f, 0.000f, 0.333f});

			// Compute object density for each sector (just the sum of every object in that sector)
			foreach (GameObject obj in visibleObjects) {
				foreach (var sectorBoundary in sectorBoundaries) {
					// WorldToViewportPoint gives x and y values between 0 and 1.000
					Vector3 viewportPoint = cam.WorldToViewportPoint (obj.transform.position);
					if (viewportPoint.x >= sectorBoundary.Value[0] && viewportPoint.x < sectorBoundary.Value[1]
						&& viewportPoint.y >= sectorBoundary.Value[2] && viewportPoint.y < sectorBoundary.Value[3]) {
						sectorDensity [sectorBoundary.Key] += 1;
						break;
					}
				}
			}

			// Compute the average sector density for each sector BUT sector 4
			float averageSectorDensity = (sectorDensity [0]
			                            + sectorDensity [1]
			                            + sectorDensity [2]
			                            + sectorDensity [3]
			                            + sectorDensity [5]
			                            + sectorDensity [6]
			                            + sectorDensity [7]
			                            + sectorDensity [8]) / 8f;

			return sectorDensity[4] / averageSectorDensity;
		}
	}
}

