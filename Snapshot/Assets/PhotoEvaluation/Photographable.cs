using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Photographable : MonoBehaviour {
	private Collider selfCollider;
	private Camera cam;
	private Plane[] planes;
	private Bounds bounds;

	public float baseScore = 1;
	public float percentOccluded;

	// Use this for initialization
	void Awake () {
		// Do what we can to guarantee we have a bounds object
		selfCollider = GetComponent<Collider>();
		if (selfCollider == null) {
			selfCollider = GetComponent<BoxCollider> ();
		}

		if (selfCollider == null) {
			bounds = GetComponent<MeshFilter> ().mesh.bounds;
		} else {
			bounds = selfCollider.bounds;
		}

        cam = GameObject.Find("Camera Prefab").GetComponentInChildren<Camera>();
	}

	// Update is called once per frame
	void Update () {
		planes = GeometryUtility.CalculateFrustumPlanes(cam);

		List<GameObject> visibleObjects = GameObject.Find("Camera Prefab").GetComponent<PhotoEval>().visibleObjs;
		if (GeometryUtility.TestPlanesAABB (planes, bounds)) {
			if (!visibleObjects.Contains (gameObject)) {
				visibleObjects.Add (gameObject);
			}
		} else {
			if (visibleObjects.Contains (gameObject)) {
				visibleObjects.Remove (gameObject);
			}
		}
	}
}