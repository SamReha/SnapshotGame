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
        Vector3 subjectPosVector = subject.transform.position;
        List<Vector3> visibleObjectCameraSpaceCoords = new List<Vector3>();

        //check to see that that the subject is in the center of the frame
        subjectPosVector = subject.transform.TransformPoint(subject.transform.position);
        subjectPosVector = cam.WorldToViewportPoint(subjectPosVector);

		if ((subjectPosVector.x <= 1.0f && subjectPosVector.x >= 0.0f) &&
		    (subjectPosVector.y <= 1.0f && subjectPosVector.y >= 0.0f)) {
			heuristicWeight = 5.0f;
		} else {
			heuristicWeight = 5.0f * 0.6f;  // hard coded for tuning purposes
		}

		//transform all positions to viewport space
		for (int i = 0; i < visibleObjects.Count; i++) {
            Vector3 cameraSpaceCoordinate = visibleObjects[i].transform.TransformPoint(visibleObjects[i].transform.position);
            cameraSpaceCoordinate = cam.WorldToViewportPoint(cameraSpaceCoordinate);
            visibleObjectCameraSpaceCoords.Add(cameraSpaceCoordinate);
		}

        //get everything on the far left
        List<Vector3> leftmostCoords = new List<Vector3>();
        for (int j = 0; j < visibleObjectCameraSpaceCoords.Count; j++) {
			if (visibleObjectCameraSpaceCoords[j].x < 0.0f
                && visibleObjectCameraSpaceCoords[j].y >= 0.0f
                && visibleObjectCameraSpaceCoords[j].y <= 1.0f) {
                leftmostCoords.Add(visibleObjectCameraSpaceCoords[j]);
			}
		}

		//get everything on the far right
		List<Vector3> rightmostCoords = new List<Vector3>();
		for (int k = 0; k < visibleObjectCameraSpaceCoords.Count; k++) {
			if (visibleObjectCameraSpaceCoords[k].x > 1.0f
                && visibleObjectCameraSpaceCoords[k].y >= 0.0f
                && visibleObjectCameraSpaceCoords[k].y <= 1.0f) {
                rightmostCoords.Add(visibleObjectCameraSpaceCoords[k]);
			}
		}

		//Calculate the distances between the of the leftmost and rightmost objects from the subject
		List<float> distancesLeft = new List<float>();
		List<float> distancesRight = new List<float>();

		for(int ii = 0; ii < leftmostCoords.Count; ii++) {
			distancesLeft.Add(Vector3.Distance(subjectPosVector, leftmostCoords[ii]));
		}
			
		for(var jj = 0; jj < rightmostCoords.Count; jj++) {
			distancesRight.Add(Vector3.Distance(subjectPosVector, rightmostCoords[jj]));
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
        
		return actualScore;
	}
}
