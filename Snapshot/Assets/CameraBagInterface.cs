using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class CameraBagInterface : MonoBehaviour {

	public int bagSize = 8;

	// Use this for initialization
	void Start () {
		GameObject grid = (GameObject)GameObject.Instantiate (Resources.Load ("Grid Panel " + bagSize));
		grid.transform.parent = GameObject.Find ("Base Panel").transform;
		grid.GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 0);
	}

	// Update is called once per frame
	void Update () {
	
	}


}

