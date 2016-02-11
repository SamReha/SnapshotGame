using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Visible : MonoBehaviour {

	public List<GameObject> visibleObjs;

	// Use this for initialization
	void Start () {
		visibleObjs = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.G)) {
			SortedList<float, GameObject> unobstructedList = UnobstructedObjs (visibleObjs);
			foreach(GameObject obj in unobstructedList.Values){
				Debug.Log (obj.name);
			}
			if (unobstructedList.Count == 0) {
				print ("Empty");
			}
		}
	}
	
	public Vector3[] gameObjectToVertexArray (GameObject go) {
		MeshFilter[] meshList = go.GetComponentsInChildren<MeshFilter> ();
		List<Vector3> vertexList = new List<Vector3> ();

		foreach (MeshFilter filter in meshList) {
			vertexList.AddRange(filter.mesh.vertices);
		}
		return vertexList.ToArray ();
	}

	public SortedList<float, GameObject> UnobstructedObjs( List<GameObject> objsToCheck ){
		SortedList<float, GameObject> returnList = new SortedList<float, GameObject>();
		foreach(GameObject obj in objsToCheck){
        //  first pass sorts the objects by distance
			float distance = Vector3.Distance (transform.position, obj.transform.position);
			returnList.Add (distance, obj);
			//  raytrace for obstructed objects. Remove if found
			//  IN PROGRESS
		}
		foreach(GameObject obj in returnList.Values){
                //  second pass omits obstructed objects
                //  Calculate the visibility of the object
                        Vector3[] vertices = gameObjectToVertexArray(obj);
                        int totalVerts = vertices.Length;
                        int visibleVerts = 0;
                        //  Sort which vertices are visible
                        foreach(Vector3 vertex in vertices){
				RaycastHit hitInfo = new RaycastHit();
				Ray obstructionChecker = new Ray (transform.position, //  origin of the ray
					Quaternion.LookRotation (vertex).eulerAngles);
				if ( Physics.Raycast( obstructionChecker, out hitInfo, 999999f)){
					Debug.Log ("Raycasted " + obj.name);
					visibleVerts++;
				}  //  Distance to trace
                        }
	                Debug.Log("Object " + obj.name + " visibility: " + (float)visibleVerts/totalVerts);
		}
		return returnList;
	}
}
