using UnityEngine;
using System.Collections;

public class Photographable : MonoBehaviour {
	//  Parent class for any object to be analyzed in a photo
	public float baseScore;
	public float percentOccluded;
	// Use this for initialization
	public Photographable( float score ){
		baseScore = score;
	}
}
