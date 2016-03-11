using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BalanceHeuristics {
	public BalanceHeuristics () {}

	public static float TestHeuristic(GameObject subject, List<GameObject> visibleObjects, Camera cam) {
		return 1f;
	}

	public static float StandardDeviation(GameObject subject, List<GameObject> visibleObjects, Camera cam){
		List<float> screenPercents = new List<float> ();
		float mean = 0f;
		for(int i = 0; i < visibleObjects.Count; i++){
			float x = GameObject.Find("Camera Prefab").GetComponent<PhotoEval>().CalcScreenPercentage(visibleObjects[i]);
			mean += x;
		}
      	mean = mean / visibleObjects.Count;
		List<float> deviation = new List<float>();
		float subjectDeviation = 0;
		if (subject != null){
		    subjectDeviation = Mathf.Abs(GameObject.Find("Camera Prefab").GetComponent<PhotoEval>().CalcScreenPercentage (subject) - mean);
		}
		float variance = 0; 
		for(int i = 0; i < screenPercents.Count; i++){
			float x = screenPercents[i] - mean;
			variance += x * x;
		}
      		variance = variance / visibleObjects.Count;
		float standardDeviation = Mathf.Sqrt(variance);
		if (standardDeviation != 0.0f) {
			float score = subjectDeviation / standardDeviation;

			// NaN protection!
			if (score == float.NaN) return 0f;
			return score;
		} else {
			return 0.0f;
		}
	}
}
