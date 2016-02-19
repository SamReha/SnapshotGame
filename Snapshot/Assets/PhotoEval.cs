using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Visible : MonoBehaviour {
	public GameObject terrain;
	public List<GameObject> visibleObjs;
	List<Ray> raysMissed;
	List<Ray> raysHit;
	List<Ray> raysBlocked;
	List<Ray> raysGrounded;
	public int raySampleResolution = 1;  //  minimum value is 1, which is the slowest but most accurate

	// Use this for initialization
	void Start () {
		visibleObjs = new List<GameObject> ();
		raysMissed = new List<Ray> ();
		raysHit = new List<Ray> ();
		raysBlocked = new List<Ray> ();
		raysGrounded = new List<Ray> ();
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
		foreach (Ray rayCasted in raysMissed){
			Debug.DrawRay (rayCasted.origin, rayCasted.direction, Color.blue);
		}
		foreach (Ray rayCasted in raysHit){
			Debug.DrawRay (rayCasted.origin, rayCasted.direction, Color.green);
		}
		foreach (Ray rayCasted in raysBlocked){
			Debug.DrawRay (rayCasted.origin, rayCasted.direction, Color.red);
		}
		foreach (Ray rayCasted in raysGrounded){
			Debug.DrawRay (rayCasted.origin, rayCasted.direction, Color.gray);
		}
	}

	public SortedList<float, GameObject> UnobstructedObjs( List<GameObject> objectsToCheck ){
		//  Remove duplicates from the list before dealing with it.
		List<GameObject> objsToCheck = objectsToCheck.Distinct ().ToList ();
		SortedList<float, GameObject> returnList = new SortedList<float, GameObject>(); 
		//  Clear the old list of rays
		raysMissed = new List<Ray>();
		raysBlocked = new List<Ray>();
		raysHit = new List<Ray>(); 
		raysGrounded = new List<Ray>(); 
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
			int blockedVerts = 0;
			//  Sort which vertices are visible
			for (int i = 0; i < totalVerts; i += raySampleResolution) {  //  For every n'th vertex, launch a ray
				//  second pass omits obstructed objects
				//  raycast to each vertex on the object
				RaycastHit hitInfo = new RaycastHit (); 
				//  The vertex group is scaled to 90% of the actual object size to prevent edge vetices from missing their mark.
				Vector3 dir = new Vector3(vertices[i].x*obj.Value.transform.lossyScale.x*0.9f, 
					vertices[i].y*obj.Value.transform.lossyScale.y*0.9f, 
					vertices[i].z*obj.Value.transform.lossyScale.z*0.9f) + 
					obj.Value.transform.position - transform.position;  //  points toward the 
				Ray obstructionRay = new Ray (transform.position, dir);
				//  true if a collision is found
				if (Physics.Raycast (obstructionRay, out hitInfo)) {
					//  Check to see if the object collided by the ray is the target
					if (hitInfo.collider.gameObject.Equals(obj.Value)){
						//  The ray has made it to the object unobstructed!
						raysHit.Add(obstructionRay);
					    visibleVerts++;
					} else if (hitInfo.collider.gameObject.Equals(terrain)){
						//  If the ray hit the ground, don't count it for or against visibility
						raysGrounded.Add(obstructionRay);
					} else {
						//  the ray hit a different Object before, obstructing the view
						raysBlocked.Add(obstructionRay); //  Keeps track of all the rays blocked on all objects
						blockedVerts++; //  Keeps track of the current object's blocked count
					}
				} else {
					//  If no collision is found
					raysMissed.Add (obstructionRay);
				}
			}
			totalVerts = blockedVerts + visibleVerts;
			//  Remove completely covered instances
			if (visibleVerts <= 0){
				Debug.Log ("Raycast missed. Removing " + obj.Value.name + " from the list.");
				obj.Value.GetComponent<Photographable>().percentOccluded = 0;
				returnList.Remove (obj.Key);
			} else {
				Debug.Log(	"Object " + obj.Value.name + " visibility: " + (float)visibleVerts*100f/(totalVerts) + "%");
				obj.Value.GetComponent<Photographable> ().percentOccluded = (float)visibleVerts / (totalVerts);
			}
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