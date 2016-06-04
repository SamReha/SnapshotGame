using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp {
	public class SpacingHeuristics {
		public SpacingHeuristics () {}

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
		public static float avoidsEmptyCenters(GameObject subject, List<GameObject> visibleObjects, Camera cam) {
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
			
			if (averageSectorDensity < 0.125f) {
				averageSectorDensity = 0.06f;
			}

			float score = sectorDensity [4] / averageSectorDensity;

            // Guard against potential negative values
            if (score < 0f) {
                return 0f;
            }

			return score;
		}

		public static float bottomThird(GameObject subject, List<GameObject> visibleObjects, Camera cam) {
			List<float> rowDensity = new List<float>(new float[3]);

			// The value of rowBoundaries is a pair of floats such that:
			// value[0] and value[1] are the min and max x values
			// value[2] and value[3] are the min and max y values
			Dictionary<int, List<float>> rowBoundaries = new Dictionary<int, List<float>> ();
			rowBoundaries.Add (0, new List<float> (){0.000f, 0.333f});
			rowBoundaries.Add (1, new List<float> (){0.333f, 0.666f});
			rowBoundaries.Add (2, new List<float> (){0.666f, 1.000f});

			// Compute object density for each sector (just the sum of every object in that sector)
			foreach (GameObject obj in visibleObjects) {
				foreach (var rowBoundary in rowBoundaries) {
					// WorldToViewportPoint gives x and y values between 0 and 1.000
					Vector3 viewportPoint = cam.WorldToViewportPoint (obj.transform.position);
					if (viewportPoint.y >= rowBoundary.Value[0] && viewportPoint.y < rowBoundary.Value[1]) {
						rowDensity [rowBoundary.Key] += 1;
						break;
					}
				}
			}

			// Compute the average sector density for each sector BUT sector 4
			float averageSectorDensity = (rowDensity [1]
				+ rowDensity [2]) / 2f;

			if (averageSectorDensity < 0.125f) {
				averageSectorDensity = 0.06f;
			}

			float score = rowDensity [0] / averageSectorDensity;

			// Guard against potential negative values
			if (score < 0f) {
				return 0f;
			}

			return score;
		}

		public static float hotSpots(GameObject subject, List<GameObject> visibleObjects, Camera cam) {
			List<float> hotSpotDensity = new List<float>(new float[25]);

			// The value of sectorBoundaries is a quartet of floats such that:
			// value[0] and value[1] are the min and max x values
			// value[2] and value[3] are the min and max y values
			Dictionary<int, List<float>> hotSpotBoundaries = new Dictionary<int, List<float>> ();
			hotSpotBoundaries.Add ( 0, new List<float> (){0.000f, 0.200f, 0.000f, 0.200f});
			hotSpotBoundaries.Add ( 1, new List<float> (){0.200f, 0.400f, 0.000f, 0.200f});
			hotSpotBoundaries.Add ( 2, new List<float> (){0.400f, 0.600f, 0.000f, 0.200f});
			hotSpotBoundaries.Add ( 3, new List<float> (){0.600f, 08.00f, 0.000f, 0.200f});
			hotSpotBoundaries.Add ( 4, new List<float> (){0.800f, 1.000f, 0.000f, 0.200f});
			hotSpotBoundaries.Add ( 5, new List<float> (){0.000f, 0.200f, 0.200f, 0.400f});
			hotSpotBoundaries.Add ( 6, new List<float> (){0.200f, 0.400f, 0.200f, 0.400f});
			hotSpotBoundaries.Add ( 7, new List<float> (){0.400f, 0.600f, 0.200f, 0.400f});
			hotSpotBoundaries.Add ( 8, new List<float> (){0.600f, 08.00f, 0.200f, 0.400f});
			hotSpotBoundaries.Add ( 9, new List<float> (){0.800f, 1.000f, 0.200f, 0.400f});
			hotSpotBoundaries.Add (10, new List<float> (){0.000f, 0.200f, 0.400f, 0.600f});
			hotSpotBoundaries.Add (11, new List<float> (){0.200f, 0.400f, 0.400f, 0.600f});
			hotSpotBoundaries.Add (12, new List<float> (){0.400f, 0.600f, 0.400f, 0.600f});
			hotSpotBoundaries.Add (13, new List<float> (){0.600f, 08.00f, 0.400f, 0.600f});
			hotSpotBoundaries.Add (14, new List<float> (){0.800f, 1.000f, 0.400f, 0.600f});
			hotSpotBoundaries.Add (15, new List<float> (){0.000f, 0.200f, 0.600f, 0.800f});
			hotSpotBoundaries.Add (16, new List<float> (){0.200f, 0.400f, 0.600f, 0.800f});
			hotSpotBoundaries.Add (17, new List<float> (){0.400f, 0.600f, 0.600f, 0.800f});
			hotSpotBoundaries.Add (18, new List<float> (){0.600f, 08.00f, 0.600f, 0.800f});
			hotSpotBoundaries.Add (19, new List<float> (){0.800f, 1.000f, 0.600f, 1.000f});
			hotSpotBoundaries.Add (20, new List<float> (){0.000f, 0.200f, 0.800f, 1.000f});
			hotSpotBoundaries.Add (21, new List<float> (){0.200f, 0.400f, 0.800f, 1.000f});
			hotSpotBoundaries.Add (22, new List<float> (){0.400f, 0.600f, 0.800f, 1.000f});
			hotSpotBoundaries.Add (23, new List<float> (){0.600f, 08.00f, 0.800f, 1.000f});
			hotSpotBoundaries.Add (24, new List<float> (){0.800f, 1.000f, 0.800f, 1.000f});

			// Compute object density for each sector (just the sum of every object in that sector)
			foreach (GameObject obj in visibleObjects) {
				foreach (var hotSpotBoundary in hotSpotBoundaries) {
					// WorldToViewportPoint gives x and y values between 0 and 1.000
					Vector3 viewportPoint = cam.WorldToViewportPoint (obj.transform.position);
					if (viewportPoint.x >= hotSpotBoundary.Value[0] && viewportPoint.x < hotSpotBoundary.Value[1]
						&& viewportPoint.y >= hotSpotBoundary.Value[2] && viewportPoint.y < hotSpotBoundary.Value[3]) {
						hotSpotDensity [hotSpotBoundary.Key] += 1;
						break;
					}
				}
			}

			// Compute the average sector density for each sector BUT sector 4
			float averageSectorDensity = (hotSpotDensity [0]
				+ hotSpotDensity [1]
				+ hotSpotDensity [2]
				+ hotSpotDensity [3]
				+ hotSpotDensity [4]
				+ hotSpotDensity [5]
				+ hotSpotDensity [7]
				+ hotSpotDensity [9]
				+ hotSpotDensity [10]
				+ hotSpotDensity [11]
				+ hotSpotDensity [12]
				+ hotSpotDensity [13]
				+ hotSpotDensity [14]
				+ hotSpotDensity [15]
				+ hotSpotDensity [17]
				+ hotSpotDensity [19]
				+ hotSpotDensity [20]
				+ hotSpotDensity [21]
				+ hotSpotDensity [22]
				+ hotSpotDensity [23]
				+ hotSpotDensity [24]) / 21f;

			if (averageSectorDensity < 0.125f) {
				averageSectorDensity = 0.06f;
			}

			float score = (hotSpotDensity [6]
				+ hotSpotDensity [8]
				+ hotSpotDensity [16]
				+ hotSpotDensity [18]) / averageSectorDensity;

			// Guard against potential negative values
			if (score < 0f) {
				return 0f;
			}

			return score;
		}
	}
}

