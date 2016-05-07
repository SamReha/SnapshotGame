using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BalanceHeuristics {
	public BalanceHeuristics () {}

	public static float StandardDeviation(GameObject subject, List<GameObject> visibleObjects, Camera cam){
		List<float> screenPercents = new List<float> ();
		float mean = 0f;
		Debug.Log ("Objects: " + visibleObjects.Count);
		for(int i = 0; i < visibleObjects.Count; i++){
			float x = GameObject.Find("Camera Prefab").GetComponent<PhotoEval>().CalcScreenPercentage(visibleObjects[i]);
			screenPercents.Add (x);
			mean += x;
		}
      	mean = mean / visibleObjects.Count;
		List<float> deviation = new List<float>();
		float variance = 0;
		for(int i = 0; i < screenPercents.Count; i++){
			float x = screenPercents[i] - mean;
			variance += x * x;
		}
      		variance = variance / visibleObjects.Count;
		float standardDeviation = Mathf.Sqrt(variance);
		if (standardDeviation != 0.0f) {
			float score = mean / standardDeviation;
            // NaN protection!
            //if (score == float.NaN) return Mathf.Infinity;
            if (score == float.NaN) return 0f;

            // Guard against negative scores
            if (score < 0f) {
                score = 0f;
            }

            return Mathf.Min(score, 10f);
		} else {
			return 10f;
		}
	}

	public static float CenteredBalance(GameObject subject, List<GameObject> visibleObjects, Camera cam) {
		float heuristicWeight = 0.0f;

		//check to see that that the subject is in the center of the frame
		subject.transform.position = subject.transform.TransformPoint(subject.transform.position);
		subject.transform.position = cam.WorldToViewportPoint(subject.transform.position);

		Vector3 subjectPosVector = subject.transform.position; // to clean the next statement up

		if ((subjectPosVector.x <= 1.0f && subjectPosVector.x >= 0.0f) &&
		    (subjectPosVector.y <= 1.0f && subjectPosVector.y >= 0.0f)) {
			heuristicWeight = 5.0f;
		} else {
			heuristicWeight = 5.0f * 0.6f;  // hard coded for tuning purposes
		}

		//get objects on the left
		List<GameObject> leftmostObjs = new List<GameObject>(); //create list for leftmost objects

		//transform all positions to viewport space
		for(int i = 0; i < visibleObjects.Count; i++) {
			if(visibleObjects[i] != subject) {
				visibleObjects[i].transform.position = visibleObjects[i].transform.TransformPoint(visibleObjects[i].transform.position);
				visibleObjects [i].transform.position = cam.WorldToViewportPoint(visibleObjects[i].transform.position);
			}
		}

		//get everything on the far left
		for(int j = 0; j < visibleObjects.Count; j++) {
			if(visibleObjects[j] != subject) {
				if(visibleObjects[j].transform.position.x < 0.0f) {
					if(visibleObjects[j].transform.position.y >= 0.0f &&
						visibleObjects[j].transform.position.y <= 1.0f) {
						leftmostObjs.Add(visibleObjects[j]);
					}
				}
			}
		}

		//get everything on the far right
		List<GameObject> rightmostObjs = new List<GameObject>(); //create list form rightmost objects

		for(int k = 0; k < visibleObjects.Count; k++) {
			if(visibleObjects[k] != subject) {
				if(visibleObjects[k].transform.position.x > 1.0f) {
					if(visibleObjects[k].transform.position.y >= 0.0f &&
						visibleObjects[k].transform.position.y <= 1.0f) {
						rightmostObjs.Add(visibleObjects[k]);
					}
				}
			}
		}

		//Calculate the distances between the of the leftmost and rightmost objects from the subject
		List<float> distancesLeft = new List<float>();
		List<float> distancesRight = new List<float>();

		for(int ii = 0; ii < leftmostObjs.Count; ii++) {
			distancesLeft.Add(Vector3.Distance(subjectPosVector, leftmostObjs[ii].transform.position));
		}
			
		for(var jj = 0; jj < rightmostObjs.Count; jj++) {
			distancesRight.Add(Vector3.Distance(subjectPosVector, rightmostObjs[jj].transform.position));
		}
		// find the largest distances calculated on the left and the rightmost distances
		float largestDistanceLeft = Mathf.Max(distancesLeft.ToArray());
		float largestDistanceRight = Mathf.Max(distancesRight.ToArray());

		if(largestDistanceLeft == largestDistanceRight) {
			heuristicWeight += 5.0f;
		} else if(Mathf.Approximately(largestDistanceLeft, largestDistanceRight)) {
			heuristicWeight += 2.0f;
		} else {
			heuristicWeight += 1.0f;
		}
		heuristicWeight *= 0.1f;

		float actualScore = 10f * heuristicWeight;

		if(actualScore == float.NaN) {
			actualScore = 0.0f;
			Debug.Log("CenteredBalance NaN");
		}

        if (actualScore < 0f) {
            actualScore = 0f;
        }

		Debug.Log ("Balance is: " + actualScore);
		return actualScore;
	}
}
