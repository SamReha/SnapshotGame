using UnityEngine;
using System.Collections;

public class Photographable {
	//  Parent class for any object to be analyzed in a photo
	public float baseScore;
	// Use this for initialization
	public Photographable( float score ){
		baseScore = score;
	}
}
