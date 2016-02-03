using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisiblilityChecker : MonoBehaviour {

	public Renderer r;

	// Use this for initialization
	void Start () {
		r = GetComponent<Renderer> ();
		r.enabled = true;
	}

	void OnBecameVisible(){
		List<string> v = GameObject.Find ("Camera Prefab").GetComponent<Visible> ().visibleObjs;
		v.Add (gameObject.name);
	}

	void OnBecameInvisible(){
		List<string> v = GameObject.Find ("Camera Prefab").GetComponent<Visible> ().visibleObjs;
		v.Remove (gameObject.name);
	}

	// Update is called once per frame
	void Update () {
	}
}
