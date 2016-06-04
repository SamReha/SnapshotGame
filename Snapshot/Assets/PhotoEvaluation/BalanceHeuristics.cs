using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BalanceHeuristics {
	public BalanceHeuristics () {}

	public static float StandardDeviation(GameObject subject, List<GameObject> visibleObjects, Camera cam){
		if (visibleObjects.Count < 2) {
			return 0f;
		} else {
			List<float> screenPercents = new List<float> ();
			float mean = 0f;
			for (int i = 0; i < visibleObjects.Count; i++) {
				float x = GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ().CalcScreenPercentage (visibleObjects [i]);
				screenPercents.Add (x);
				mean += x;
			}
			mean = mean / visibleObjects.Count;

			float variance = 0;
			for (int i = 0; i < screenPercents.Count; i++) {
				float x = screenPercents [i] - mean;
				variance += x * x;
			}
			variance = variance / visibleObjects.Count;
			float standardDeviation = Mathf.Sqrt (variance);
			if (standardDeviation != 0.0f) {
				float score = mean / standardDeviation;
				// NaN protection!
				//if (score == float.NaN) return Mathf.Infinity;
				if (score == float.NaN)
					return 0f;

				// Guard against negative scores
				if (score < 0f) {
					score = 0f;
				}
				return Mathf.Min (score, 10f) * 10f;
			} else {
				return 100f;
			}
		}
	}
	
	public static float AsymmetricBalance(GameObject subject, List<GameObject> visibleObjects, Camera cam) {
		float heuristicWeight = 0f;
		float percentageLeft = 0f;
		float percentageRight = 0f;

		List<Vector3> camSpaceCoords = new List<Vector3>();
		List<Vector3> leftMostCoords = new List<Vector3>();
		List<Vector3> rightMostCoords = new List<Vector3>();

		List<GameObject> leftMostGameObjs = new List<GameObject>();
		List<GameObject> rightMostGameObjs = new List<GameObject>();

		for(int i = 0; i < visibleObjects.Count; i++) {
			Vector3 viewportCoord = visibleObjects[i].transform.TransformPoint(visibleObjects [i].transform.position);

			viewportCoord = cam.WorldToViewportPoint (viewportCoord);
			camSpaceCoords.Add (viewportCoord);
		}

		for(int j = 0; j < camSpaceCoords.Count; j++) {
			if (camSpaceCoords[j].x > 1.0f &&
			    camSpaceCoords[j].y >= 0.0f &&
			    camSpaceCoords[j].y <= 1.0f) {
				rightMostCoords.Add(camSpaceCoords[j]);
			} else {
				leftMostCoords.Add(camSpaceCoords[j]);
			}
		}

		if (rightMostCoords.Count > 0) {
			for (int ii = 0; ii < rightMostCoords.Count; ii++) {
				rightMostGameObjs.Add (visibleObjects [ii]);
			}
		}

		for (int jj = 0; jj < visibleObjects.Count; jj++) {
			if (rightMostGameObjs.Contains(visibleObjects[jj])) {
				continue;
			} else {
				leftMostGameObjs.Add(visibleObjects[jj]);
			}
		}

		if (leftMostGameObjs.Count > 0 && rightMostGameObjs.Count > 0) {
			for (int kk = 0; kk < leftMostGameObjs.Count; kk++) {
				if (percentageLeft + cam.GetComponentInParent<PhotoEval> ().CalcScreenPercentage (leftMostGameObjs [kk]) <= 100f) {
					percentageLeft += cam.GetComponentInParent<PhotoEval> ().CalcScreenPercentage (leftMostGameObjs [kk]);
				} else {
					break;
				}
			}

			for (int kk2 = 0; kk2 < rightMostGameObjs.Count; kk2++) {
				if (percentageRight + cam.GetComponentInParent<PhotoEval> ().CalcScreenPercentage (rightMostGameObjs [kk2]) <= 100f) {
					percentageRight += cam.GetComponentInParent<PhotoEval> ().CalcScreenPercentage (rightMostGameObjs [kk2]);
				} else {
					break;
				}
			}

			float score = 10f;

			if (percentageLeft == percentageRight) {
				return 100f;
			} else if (Mathf.Approximately (percentageLeft, percentageRight)) {
				heuristicWeight = 0.8f;
				score = score * heuristicWeight;
				//Debug.Log("percents: " + percentageLeft + ", " + percentageRight);
				score = Mathf.Max(Mathf.Min(score, 100),0);
				Debug.Log ("Asym score: " + score);
				return score;
			} else {
				float difference = Mathf.Abs ((percentageLeft - percentageRight));

				score = 100;
				//difference = 100f * difference;
				score -= difference;
			//	Debug.Log("percents: " + percentageLeft + ", " + percentageRight);
				Debug.Log ("Balance B(Asym) score: " + score);
				score = Mathf.Max(Mathf.Min(score, 100),0);
				return score;
			}

		} else if (leftMostGameObjs.Count > 0 && rightMostGameObjs.Count == 0) {
			for (int iii = 0; iii < leftMostGameObjs.Count; iii++) {
				if (percentageLeft + cam.GetComponentInParent<PhotoEval>().CalcScreenPercentage (leftMostGameObjs [iii]) <= 100f) {
					percentageLeft += cam.GetComponentInParent<PhotoEval>().CalcScreenPercentage (leftMostGameObjs [iii]);
				} else {
					break;
				}
			}
			//Debug.Log ("percentage: " + percentageLeft);
			float score = 100f;
			heuristicWeight = percentageLeft * 0.1f;
			float difference = 100f * heuristicWeight;
			score -= difference;
			if (score < 0f) {
				return 0f;
			}
			Debug.Log ("Asym score: " + score);
			return score;

		} else if (rightMostGameObjs.Count > 0 && leftMostGameObjs.Count == 0) {
			for (int jjj = 0; jjj < rightMostGameObjs.Count; jjj++) {
				if (percentageRight + cam.GetComponentInParent<PhotoEval> ().CalcScreenPercentage (rightMostGameObjs [jjj]) <= 100f) {
					percentageRight += cam.GetComponentInParent<PhotoEval> ().CalcScreenPercentage (rightMostGameObjs [jjj]);
				} else {
					break;
				}
				//Debug.Log ("percent: " + percentageRight);
			}
			float score = 100f;
			heuristicWeight = percentageRight * 0.1f;
			float difference = 100f * heuristicWeight;
			score -= difference;
			Debug.Log ("Asym score: " + score);
			if (score < 0f) {
				return 0f;
			}
			return score;
		} else {
			return 0f;
		}
	}

	public static float CenteredBalance(GameObject subject, List<GameObject> visibleObjects, Camera cam) {
        if (subject == null) return 0.0f;   // if by some mistake we have no subject, bail out!

        float heuristicWeight = 0.0f;
        Vector3 subjectPosVector = subject.transform.position;
        List<Vector3> visibleObjectCameraSpaceCoords = new List<Vector3>();

        //check to see that that the subject is in the center of the frame
        subjectPosVector = subject.transform.TransformPoint(subject.transform.position);
        subjectPosVector = cam.WorldToViewportPoint(subjectPosVector);

        if ((subjectPosVector.x <= 1.0f && subjectPosVector.x >= 0.0f) &&
            (subjectPosVector.y <= 1.0f && subjectPosVector.y >= 0.0f))
        {
            heuristicWeight = 5.0f;
        }
        else
        {
            heuristicWeight = 5.0f * 0.6f;  // hard coded for tuning purposes
        }

        //transform all positions to viewport space
        for (int i = 0; i < visibleObjects.Count; i++)
        {
            Vector3 cameraSpaceCoordinate = visibleObjects[i].transform.TransformPoint(visibleObjects[i].transform.position);
            cameraSpaceCoordinate = cam.WorldToViewportPoint(cameraSpaceCoordinate);
            visibleObjectCameraSpaceCoords.Add(cameraSpaceCoordinate);
        }

        //get everything on the far left
        List<Vector3> leftmostCoords = new List<Vector3>();
        for (int j = 0; j < visibleObjectCameraSpaceCoords.Count; j++)
        {
            if (visibleObjectCameraSpaceCoords[j].x < 0.0f
                && visibleObjectCameraSpaceCoords[j].y >= 0.0f
                && visibleObjectCameraSpaceCoords[j].y <= 1.0f)
            {
                leftmostCoords.Add(visibleObjectCameraSpaceCoords[j]);
            }
        }

        //get everything on the far right
        List<Vector3> rightmostCoords = new List<Vector3>();
        for (int k = 0; k < visibleObjectCameraSpaceCoords.Count; k++)
        {
            if (visibleObjectCameraSpaceCoords[k].x > 1.0f
                && visibleObjectCameraSpaceCoords[k].y >= 0.0f
                && visibleObjectCameraSpaceCoords[k].y <= 1.0f)
            {
                rightmostCoords.Add(visibleObjectCameraSpaceCoords[k]);
            }
        }

        //Calculate the distances between the of the leftmost and rightmost objects from the subject
        List<float> distancesLeft = new List<float>();
        List<float> distancesRight = new List<float>();

        for (int ii = 0; ii < leftmostCoords.Count; ii++)
        {
            distancesLeft.Add(Vector3.Distance(subjectPosVector, leftmostCoords[ii]));
        }

        for (var jj = 0; jj < rightmostCoords.Count; jj++)
        {
            distancesRight.Add(Vector3.Distance(subjectPosVector, rightmostCoords[jj]));
        }

        // find the largest distances calculated on the left and the rightmost distances
        float largestDistanceLeft = Mathf.Max(distancesLeft.ToArray());
        float largestDistanceRight = Mathf.Max(distancesRight.ToArray());

        if (largestDistanceLeft == largestDistanceRight)
        {
            heuristicWeight += 5.0f;
        }
        else if (Mathf.Approximately(largestDistanceLeft, largestDistanceRight))
        {
            heuristicWeight += 2.0f;
        }
        else
        {
            heuristicWeight += 1.0f;
        }
        heuristicWeight *= 0.1f;

        float actualScore = 100f * heuristicWeight;

        if (actualScore == float.NaN)
        {
            actualScore = 0.0f;
            Debug.Log("CenteredBalance NaN");
        }

        if (actualScore < 0f)
        {
            actualScore = 0f;
        }
		Debug.Log ("Balance C " + actualScore);
		if (actualScore < 0f) {
			return 0f;
		}
        return actualScore;
	}
}
