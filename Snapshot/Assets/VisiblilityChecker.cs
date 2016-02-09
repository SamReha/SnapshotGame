using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisiblilityChecker : MonoBehaviour {
	
	public GameObject anObject;
    public Collider anObjCollider;
    private Camera cam;
    private Plane[] planes;
	
	public Renderer r;

	// Use this for initialization
	void Start () {
		r = GetComponent<Renderer> ();
		r.enabled = true;
		anObject = gameObject;
		anObjCollider = GetComponent<Collider>();
		cam = GameObject.Find ("PlayerCam").GetComponent<Camera> ();
	}

	void OnBecameVisible(){
		List<GameObject> v = (List<GameObject>)GameObject.Find ("Camera Prefab").GetComponent<Visible> ().visibleObjs;
		//  organize the list by distance to cam
		v.Add (gameObject );
	}

	void OnBecameInvisible(){
		List<GameObject> v = (List<GameObject>)GameObject.Find ("Camera Prefab").GetComponent<Visible> ().visibleObjs;
		v.Remove (gameObject);
	}

	// Update is called once per frame
	void Update () {
		
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        
		//if (GeometryUtility.TestPlanesAABB(planes, anObjCollider.bounds))
        //    Debug.Log(anObject.name + " has been detected!");
        //else
        //    Debug.Log("Nothing has been detected");
	}
}
