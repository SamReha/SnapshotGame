using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp {
	public class InterestingnessHeuristics {
		

		public static float highestScore = 10f;
		private static float forgetRate = 0.1f;
		//  Maps names to familiarity. +1 for every new instance

		public InterestingnessHeuristics () {
		}

		//  Main Interest Heuristic
		public static float interestAndBoredomHeuristic(GameObject subject, List<GameObject> visibleObjects, Camera cam){
        //  Actively adjusts the high score. Returns a float between 0-1 representing 
	    //  how interesting the objects in the photo are compared to the objects in the memory bank.
		//  "New" objects gain +b/(n*0.75)*(x/m) advantage, where  b = baseScore 
	    //                                                 n = the inclusive number of instances in memory.
		//                                                 x = how far back in memory ( starts at m and counts down)
			//                                                 m = total memory (attention span)
	    //  x/m is the fuzziness of memory, 
		//  b/(n*0.75) is like b/n, except with b/n if every tree is seen is in the photo at once, 
	    //  you'll get the same total score as if you took your first picture of 1 tree.
		//  So the 0.75 weight will make the case where multiple foxes photographed together for the first time in a while
		//  Will be more than the same situation with only 1 fox, But If you take a photo of a fox every day it won't be worth as much.
			float score = 0;
			foreach (GameObject obj in visibleObjects){
				Photographable photoData = obj.GetComponent<Photographable>();
				if (photoData != null) {
					score += photoData.baseScore;
				//  Update the familiarity of the object
				}
			}
			highestScore = Mathf.Max (score, highestScore);
			//Debug.Log ("Interest score is: " + score + "  Highest: " + highestScore + "    -- " + score/highestScore);
			return score/highestScore;
		}
	}
}

