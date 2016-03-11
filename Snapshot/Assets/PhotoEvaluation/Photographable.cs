﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Photographable : MonoBehaviour {
	
	public GameObject anObject;
	public Collider anObjCollider;
	private Camera cam;
	private Plane[] planes;

	public float baseScore;
	public float percentOccluded;

	public Renderer r;

	// Use this for initialization
	void Start () {
		baseScore = 1;
		r = GetComponent<Renderer> ();
		r.enabled = true;
		anObject = gameObject;
		anObjCollider = GetComponent<Collider>();
		cam = GameObject.Find ("PlayerCam").GetComponent<Camera> ();
	}



	// Update is called once per frame
	void Update () {
		planes = GeometryUtility.CalculateFrustumPlanes(cam);

		if (GeometryUtility.TestPlanesAABB (planes, anObjCollider.bounds)) {
			List<GameObject> v = GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ().visibleObjs;
			if (!v.Contains (gameObject)) {
				v.Add (gameObject);
			}
		} else {
			List<GameObject> v = GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ().visibleObjs;
			if (v.Contains (gameObject)) {
				v.Remove (gameObject);
			}
		}
	}


}