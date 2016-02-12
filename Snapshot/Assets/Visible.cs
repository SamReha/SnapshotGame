using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Visible : MonoBehaviour {

	public List<GameObject> visibleObjs;
	List<Ray> obstructionChecker;
	public int raySampleResolution = 4;  //  minimum value is 1, which is the slowest but most accurate

	// Use this for initialization
	void Start () {
		visibleObjs = new List<GameObject> ();
		obstructionChecker = new List<Ray> ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.G)) {
			SortedList<float, GameObject> unobstructedList = UnobstructedObjs (visibleObjs);
			/*
			foreach(GameObject obj in unobstructedList.Values){
				Debug.Log (obj.name);
			}
			*/
			if (unobstructedList.Count == 0) {
				print ("Empty");
			}
		}
		foreach (Ray rayCasted in obstructionChecker){
			Debug.DrawRay (rayCasted.origin, rayCasted.direction, Color.blue);
		}
	}

	public SortedList<float, GameObject> UnobstructedObjs( List<GameObject> objectsToCheck ){
		//  Remove duplicates from the list before dealing with it.
		List<GameObject> objsToCheck = objectsToCheck.Distinct ().ToList ();
		SortedList<float, GameObject> returnList = new SortedList<float, GameObject>(); 
		//  Clear the old list of rays
		obstructionChecker = new List<Ray>(); 
		foreach(GameObject obj in objsToCheck){
			//  first pass sorts the objects by distance
			//Debug.Log("Position is " + transform.position);
			float distance = Vector3.Distance (transform.position, obj.transform.position);
			returnList.Add (distance, obj);
		}
		//  raytrace for obstructed objects. Remove if found
		//  IN PROGRESS
		foreach(KeyValuePair<float, GameObject> obj in returnList){ //  For each object in question
			Vector3[] vertices = gameObjectToVertexArray(obj.Value); //  Break the object down into vertex data
			int totalVerts = vertices.Length;
			int visibleVerts = 0;
			//  Sort which vertices are visible
			for (int i = 0; i < totalVerts; i += raySampleResolution) {  //  For every n'th vertex, launch a ray
				//  second pass omits obstructed objects
				//  raycast to each vertex on the object
				RaycastHit hitInfo = new RaycastHit (); 
				Vector3 dir = new Vector3(vertices[i].x*obj.Value.transform.lossyScale.x, 
					                      vertices[i].y*obj.Value.transform.lossyScale.y, 
					                      vertices[i].z*obj.Value.transform.lossyScale.z) + 
					                      obj.Value.transform.position - transform.position;  //  points toward the 
				Ray obstructionRay = new Ray (transform.position, dir);
				obstructionChecker.Add (obstructionRay);
				//  Debug.DrawLine (transform.position, hitInfo.point, Color.green);
				//  Debug.DrawRay (transform.position, rayDirection, Color.red, 20, true);
				if (!Physics.Raycast (obstructionRay, out hitInfo)) {
					//  on raycast hit
					Debug.Log ("Raycasted " + obj.Value.name);
					visibleVerts++;
				}
			}
			//  Remove completely covered instances
			if (visibleVerts <= 0){
				Debug.Log ("Raycast missed. Removing " + obj.Value.name + " from the list.");
				returnList.Remove (obj.Key);
			}
			Debug.Log(	"Object " + obj.Value.name + " visibility: " + visibleVerts + "/" + 
				totalVerts/raySampleResolution + "=" + (float)visibleVerts/(totalVerts/raySampleResolution));
		}
		return returnList;
	}
	
	public Vector3[] gameObjectToVertexArray (GameObject go) {
		MeshFilter[] meshList = go.GetComponentsInChildren<MeshFilter> ();
		List<Vector3> vertexList = new List<Vector3> ();
		foreach (MeshFilter filter in meshList) {
			vertexList.AddRange(filter.mesh.vertices);
		}
		return vertexList.ToArray ();
	}
}
