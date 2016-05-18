using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Photographable : MonoBehaviour {
	public Collider selfCollider;
	private Camera cam;
	private Plane[] planes;

	public float baseScore;
	public float percentOccluded;

	// Use this for initialization
	void Awake () {
		baseScore = 1;
		selfCollider = GetComponent<Collider>();

        cam = GameObject.Find("Camera Prefab").GetComponentInChildren<Camera>();
	}

	// Update is called once per frame
	void Update () {
		planes = GeometryUtility.CalculateFrustumPlanes(cam);

        List<GameObject> visibleObjects = GameObject.Find("Camera Prefab").GetComponent<PhotoEval>().visibleObjs;
        if (GeometryUtility.TestPlanesAABB (planes, selfCollider.bounds)) {
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