using System;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp {
	public class InterestingnessHeuristics {
		// private static float forgetRateTotal = 1f;
		// private static float forgetRateIndividual = 0.1f;
		// Maps names to familiarity. +1 for every new instance

		public InterestingnessHeuristics () {}

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
			//  
			//  First, load the player profile
			PlayerProfile playerData = GameObject.FindGameObjectWithTag ("PlayerCam").GetComponent<PlayerProfile>();
			if (playerData == null) {
				Debug.Log ("Could not load playerData from InterestingnessHeuristics");
				return 0;  //  "Eject button"
			}
			float score = 0;
			foreach (GameObject obj in visibleObjects){
				Photographable photoData = obj.GetComponent<Photographable>();
				if (photoData != null) {
					score += photoData.baseScore;
					//  Update the familiarity of the object
				}
			}
			playerData.maxInterestTotal = Mathf.Max (score, playerData.maxInterestTotal);
			//Debug.Log ("Interest score is: " + score + "  Highest: " + highestScore + "    -- " + score/highestScore);

			// Guard against potential negative score
			float finalScore = score / playerData.maxInterestTotal;
			if (finalScore < 0f) {
				finalScore = 0f;
			} else if (finalScore > 100f) {
				finalScore = 100f;
			}

			return finalScore;
		}

		//  Adds up raw interest scores
		public static float rawThresholdInterest(GameObject subject, List<GameObject> visibleObjects, Camera cam){
			//  Adds up all of the object's interestingness if they are above "ignoreUnder" and caps the total after "capOver"
			float ignoreUnder = 0f;
			float capOver = 100f;
			float score = 0;
			foreach (GameObject obj in visibleObjects){
				Photographable photoData = obj.GetComponent<Photographable>();
				if (photoData != null) {
					if (photoData.baseScore >= ignoreUnder){ 
						score += photoData.baseScore;
					}
					//  Update the familiarity of the object
				}
			}
			score = Mathf.Min (score, capOver);
			//Debug.Log ("Interest score is: " + score + "  Highest: " + highestScore + "    -- " + score/highestScore);

			// Guard against potential negative score
			float finalScore = score / capOver;

			if (finalScore > 100f) {
				finalScore = 100f;
			}
			return finalScore;
		}


		public static float mostInterestingObjectHeuristic(GameObject subject, List<GameObject> visibleObjects, Camera cam){
			//  Works exactly like interest and boredom, except it only evaluates the score of the most 

			//  Load the player profile
			PlayerProfile playerData = GameObject.FindGameObjectWithTag ("PlayerCam").GetComponent<PlayerProfile>();
			if (playerData == null) {
				Debug.Log ("Could not load playerData from InterestingnessHeuristics");
				return 0;  //  "Eject button"
			}
			float score = 0;
			foreach (GameObject obj in visibleObjects){
				Photographable photoData = obj.GetComponent<Photographable>();
				if (photoData != null) {
					score += photoData.baseScore;
					//  Update the familiarity of the object
				}
			}
			playerData.maxInterestIndividual = Mathf.Max (score, playerData.maxInterestIndividual);
			//Debug.Log ("Interest score is: " + score + "  Highest: " + highestScore + "    -- " + score/highestScore);

			// Guard against potential negative score
			float finalScore = score / playerData.maxInterestIndividual;
			if (finalScore < 0f) {
				finalScore = 0f;
			} else if (finalScore > 100f) {
				finalScore = 100f;
			}

			return finalScore;
		}
	}
}

