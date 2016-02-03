using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Visible : MonoBehaviour {

	public List<string> visibleObjs;

	// Use this for initialization
	void Start () {
		visibleObjs = new List<string> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.G)) {
			for (int i = 0; i < visibleObjs.Count; i++) {
				print (visibleObjs [i]);
			}
			if (visibleObjs.Count == 0) {
				print ("Empty");
			}
		}
	}
}
