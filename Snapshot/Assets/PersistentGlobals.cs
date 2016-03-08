using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentGlobals : MonoBehaviour {

	//  When the player leaves the park, his photos are loaded into uploadPending;
	//  The list is cleared after the player chooses which photos to upload.
	public List<Photo> pics = new List<Photo>();


	private bool created = false;  //  This is a singleton

	void Awake(){
		if (!created) {
			DontDestroyOnLoad (this.gameObject);
			created = true;
		} else {
			//  If this is a duplicate from a scene reload, destroy
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
