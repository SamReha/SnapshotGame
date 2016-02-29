using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PhotoEval : MonoBehaviour {
	public GameObject terrain;
	public List<GameObject> visibleObjs;
	List<Ray> raysMissed;
	List<Ray> raysHit;
	List<Ray> raysBlocked;
	List<Ray> raysGrounded;
	public int raySampleResolution = 1;  //  minimum value is 1, which is the slowest but most accurate

	public Vector3 viewPos;
	Vector3[] corners = new Vector3[8];
	public List<float> percentInFrame;
	public List<float> percentCentered;
	private Camera cam;

	public float balance = -1f;
	public float spacing = -1f;
	public float interest = -1f;

	// Key: heuristic function Value: weight
	Dictionary<System.Func<List<GameObject>, Camera, float>, float> spacingHeuristicMap;
	Dictionary<System.Func<List<GameObject>, Camera, float>, float> balanceHeuristicMap;
	Dictionary<System.Func<List<GameObject>, Camera, float>, float> interestHeuristicMap;

	// Use this for initialization
	void Start () {
		visibleObjs = new List<GameObject> ();
		raysMissed = new List<Ray> ();
		raysHit = new List<Ray> ();
		raysBlocked = new List<Ray> ();
		raysGrounded = new List<Ray> ();
		percentInFrame = new List<float> ();
		percentCentered = new List<float> ();
		cam = GameObject.Find ("PlayerCam").GetComponent<Camera> ();

		// Heuristic Setup
		spacingHeuristicMap = new Dictionary<System.Func<List<GameObject>, Camera, float>, float>();
		spacingHeuristicMap.Add (AssemblyCSharp.SpacingHeuristics.avoidsEmptyCenters, 1f);

		balanceHeuristicMap = new Dictionary<System.Func<List<GameObject>, Camera, float>, float>();
		balanceHeuristicMap.Add (BalanceHeuristics.StandardDeviation, 1f);

		interestHeuristicMap = new Dictionary<System.Func<List<GameObject>, Camera, float>, float>();
		interestHeuristicMap.Add (AssemblyCSharp.InterestingnessHeuristics.interestAndBoredomHeuristic, 1f);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonUp ("Take Photo")) {
			SortedList<float, GameObject> unobstructedList = UnobstructedObjs (visibleObjs);
			visibleObjs = unobstructedList.Values.ToList();
			/*
			foreach(GameObject obj in unobstructedList.Values){
				Debug.Log (obj.name);
			}
			*/
			if (unobstructedList.Count == 0) {
				print ("Empty");
			}
			for (int i = 0; i < visibleObjs.Count; i++) {
				GameObject go = visibleObjs [i];
				Debug.Log ("Object: " + go.name);
				Corners (go);
				viewPos = cam.WorldToViewportPoint (go.transform.position);
				Debug.Log ("Position: " + viewPos.ToString("F4"));
				CalcObjPercentage (corners, go);
				Debug.Log ("Percent in Frame: " + percentInFrame);
				//IsFramed (i);
				Debug.Log ("Centered: " + percentCentered);
			}
			//  Subject might return null if visibleObj's is empty
			GameObject subject = getSubject (visibleObjs);

			// Evaluate spacing
			spacing = evaluateHeuristics(visibleObjs, spacingHeuristicMap);
			interest = evaluateHeuristics(visibleObjs, interestHeuristicMap);
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

	public void PhotoValues (){
		SortedList<float, GameObject> unobstructedList = UnobstructedObjs (visibleObjs);
		/*
			foreach(GameObject obj in unobstructedList.Values){
				Debug.Log (obj.name);
			}
			*/
		if (unobstructedList.Count == 0) {
			print ("Empty");
		}
		//  RK: Changed iteration from all of the objects within the frame 
		//  to only those which are not blocked by other objects
		int i = 0;
		foreach ( KeyValuePair<float, GameObject> kvp in unobstructedList) {
			i++;  //  i is a temporary solution. I suggest we work around using indecies
			GameObject go = kvp.Value;
			Debug.Log ("Object: " + go.name);
			Corners (go);
			viewPos = cam.WorldToViewportPoint (go.transform.position);
			Debug.Log ("Position: " + viewPos.ToString("F4"));
			CalcObjPercentage (corners, go);
			Debug.Log ("Percent in Frame: " + percentInFrame);
			IsFramed (i);
			Debug.Log ("Centered: " + percentCentered);
		}
	}

	int framePosition() {
		float x = viewPos.x;
		float y = viewPos.y;

		if(x >= 0.3f || x <= 0.36f && y >= 0.3f || y <= 0.36f){
			return 1;
		}
		else if(x >= 0.63f || x <= 0.69f && y >= 0.3f || y <= 0.36f){
			return 2;
		}
		else if(x >= 0.3f || x <= 0.36f && y >= 0.63f || y <= 0.69f){
			return 3;
		}else if(x >= 0.63f || x <= 0.69f && y >= 0.63f || y <= 0.69f){
			return 4;
		}else if(x >= 0.47f || x <= 0.53f && y >= 0.47f || y <= 0.53f){
			return 5;
		}else if(x <= 0.33f && y <= 0.33f){
			return 6;
		}else if(x <= 0.66f && y <= 0.33f){
			return 7;
		}else if(y <= 0.33f){
			return 8;
		}else if(x <= 0.33f && y <= 0.66f){
			return 9;
		}else if(x <= 0.66f && y <= 0.66f){
			return 10;
		}else if(y <= 0.66f){
			return 11;
		}else if(x >= 0.33f){
			return 12;
		}else if(x >= 0.66f){
			return 13;
		}else{
			return 14;
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
				Vector3 rotatedVertex = obj.Value.transform.rotation * vertices [i];
				Vector3 dir = new Vector3( rotatedVertex.x*obj.Value.transform.lossyScale.x*0.9f, 
					rotatedVertex.y*obj.Value.transform.lossyScale.y*0.9f, 
					rotatedVertex.z*obj.Value.transform.lossyScale.z*0.9f) + 
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

	// Roughly estimates the percentage of an object that is on screen using the AABB
	void CalcObjPercentage(Vector3[] v, GameObject go){
		float percent = 100;
		int[] cornerLocations = new int[v.Length];	// An int array holds the positions of the corners of the bounding box
		for (int i = 0; i < v.Length; i++) {
			v[i] = go.transform.TransformPoint(v[i]);
			v[i] = cam.WorldToViewportPoint(v[i]);
			cornerLocations [i] = BoundsChecker (v [i]);
		}
		bool isAllInFrame = true;	// Edge case object completely in frame
		bool noneInFrame = true;	// Edge case object completely out of frame

		// Testing for isAllInFrame
		for (int i = 0; i < v.Length; i++) {
			if (cornerLocations [i] != 5) {
				isAllInFrame = false;
				break;
			}
		}
		// Testing for noneInFrame
		for (int i = 0; i < v.Length; i++) {
			if (cornerLocations[i] == 5) {
				noneInFrame = false;
				break;
			}
		}
		if (isAllInFrame) {
			// If all points are in frame object 100% in frame
			return;
		} else if (noneInFrame) {
			// If all points are out of frame 0%
			percent = 0.0f;
			// Unless center is in frame. Given arbitrary value at this point may need adjustment.
			if (BoundsChecker (viewPos) == 5) {
				percent = 50f;
			}
			percentInFrame.Add (percent);
			return;
		} else { 
			// Caculate distance of all objects from edge of frame
			for (int i = 0; i < v.Length; i++) {
				percent  = CalcObjPoint (cornerLocations [i], v [i], 25f, -1.0f, 2.0f, -1.0f, 2.0f);
			}
			// Edge case can go below 0% dependent on proximity
			if (percent < 0) {
				percent = 0;
				percentInFrame.Add (percent);
			}
		}
	}

	// Checks the relative positions of a vector compared to the viewport and assigns a location based on a 3x3 grid;
	//	1	2	3
	//	4	5	6
	//	7	8	9
	// 5 is viewport
	int BoundsChecker(Vector3 v){
		if (v.z < 0.0f) {
			return 0;	// If the vector is behind the camera
		} else if ((v.x <= 1.0f && v.x >= 0.0f) && (v.y <= 1.0f && v.y >= 0.0f)) {
			return 5; // If the vector is in viewport
		} else {
			if (v.x < 0.0f) {
				if (v.y < 0.0f) {
					return 7;	// Bottom left
				} else if (v.y > 1.0f) {
					return 1;	// Top Left
				} else {
					return 4;	// Middle left
				}
			} else if (v.x > 1.0f) {
				if (v.y < 0.0f) {
					return 9;	// Bottom Right
				} else if (v.y > 1.0f) {
					return 3;	// Top Right
				} else {
					return 6;	// Middle Right
				}
			} else {
				if (v.y < 0.0f) {
					return 8;	// Bottom Middle
				} else {
					return 2;	// Top Middle
				}
			}
		}
	}

	// Calculates the relative position of a vector in comparison to the cameras viewport
	float CalcObjPoint(int i, Vector3 v, float value, float minX, float maxX, float minY, float maxY){

		float x = v.x;
		float y = v.y;
		float percent = 100f;

		// There is a case for each point in the grid calculated with BoundChecker except 5
		if (i == 0) {
			// If the point is behind the camera
			// Currently does nothing since having a point like this implies player is inside of object
		} else if (i == 1) {
			// Check if inside the min/max values. If not subtract whole total
			if (x < minX || y > maxY) {
				percent -= value;
			} else {
				//Calculate distance from frame corner
				float p = value * (Mathf.Sqrt (x * x + y * y) / Mathf.Sqrt (minX * minX + maxY * maxY));
				percent -= p;
			}
		} else if (i == 2) {
			// Check if inside the min/max values. If not subtract whole total
			if (y > maxY) {
				percent -= value;
			} else {
				//Calculate distance from frame corner
				float p = value * (y / maxY);
				percent -= p;
			}
		} else if (i == 3) {
			// Check if inside the min/max values. If not subtract whole total
			if (x > maxX || y > maxY) {
				percent -= value;
			} else {
				//Calculate distance from frame corner
				float p = value * (Mathf.Sqrt (x * x + y * y) / Mathf.Sqrt (maxX * maxX + maxY * maxY));
				percent -= p;
			}
		} else if (i == 4) {
			// Check if inside the min/max values. If not subtract whole total
			if (x < minX) {
				percent -= value;
			} else {
				//Calculate distance from frame 
				float p = value * (x / minX);
				percent -= p;
			}
		} else if (i == 6) {
			// Check if inside the min/max values. If not subtract whole total
			if (x > maxX) {
				percent -= value;
			} else {
				//Calculate distance from frame
				float p = value * (x / maxX);
				percent -= p;
			}
		} else if (i == 7) {
			// Check if inside the min/max values. If not subtract whole total
			if (x < minX || y < minY) {
				percent -= value;
			} else {
				//Calculate distance from frame corner
				float p = value * (Mathf.Sqrt (x * x + y * y) / Mathf.Sqrt (minX * minX + minY * minY));
				percent -= p;
			}
		} else if (i == 8) {
			// Check if inside the min/max values. If not subtract whole total
			if (y < minY) {
				percent -= value;
			} else {
				//Calculate distance from frame
				float p = value * (y / minY);
				percent -= p;
			}
		} else if (i == 9) {
			// Check if inside the min/max values. If not subtract whole total
			if (x > maxX || y < minY) {
				percent -= value;
			} else {
				//Calculate distance from frame corner
				float p = value * (Mathf.Sqrt (x * x + y * y) / Mathf.Sqrt (maxX * maxX + minY * minY));
				percent -= p;
			}
		}
		return percent;
	}


	// Checks how centered within the camera viewport the object is
	// At the moment all values calculated are arbitrary
	void IsFramed(int i){
		float x = viewPos.x;
		float y = viewPos.y;
		Debug.Log ("Percent in frame: size " + percentInFrame.Count + " while i is " + i);
		if (percentInFrame[i] > 0) {
			if (x <= 0.6f && x >= 0.4f) {
				if (y <= 0.6f && y >= 0.4f) {
					percentCentered.Add (percentInFrame [i]);	// If the object essentially centered 
				} else if (y > 0.6f && y <= 1.0f || y < 0.4f && y >= 0.0f) {
					percentCentered.Add (percentInFrame [i] * 0.7f);	// If the object is centered along x but not y
				} else {
					percentCentered.Add (0.0f);	// If the object is outside of frame
				}
			} else if (x > 0.6f && x <= 1.0f || x < 0.4f && x >= 0.0f) {
				if (y <= 0.6f && y >= 0.4f) {
					percentCentered.Add (percentInFrame [i] * 0.7f);	// If the object is is centered along y but not x
				} else if (y > 0.6f && y <= 1.0f || y < 0.4f && y >= 0.0f) {
					percentCentered.Add (percentInFrame [i] * 0.3f);	// If the object is not centered on x or y but stil in frame
				} else {
					percentCentered.Add (0.0f);	// If the object is outside of frame 
				}
			} else {
				percentCentered.Add(0.0f);	// If the object is outside of frame
			}
		}
	}

	// Calculates how much of the screen the object is taking up
	// Currently only used to calculated bounding box corners
	public float CalcScreenPercentage(GameObject go) {

		float minX = float.PositiveInfinity;
		float minY = float.PositiveInfinity;
		float maxX = float.NegativeInfinity;
		float maxY = float.NegativeInfinity;

		Corners (go);

		for (int i = 0; i < corners.Length; i++) {
			Vector3 corner = transform.TransformPoint(corners[i]);
			corner = cam.WorldToScreenPoint(corner);
			if (corner.x > maxX) {
				maxX = corner.x;
			}
			if (corner.x < minX) {
				minX = corner.x;
			}
			if (corner.y > maxY) {
				maxY = corner.y;
			}
			if (corner.y < minY) {
				minY = corner.y;
			}
			minX = Mathf.Clamp(minX, 0, Screen.width);
			maxX = Mathf.Clamp(maxX, 0, Screen.width);
			minY = Mathf.Clamp(minY, 0, Screen.height);
			maxY = Mathf.Clamp(maxY, 0, Screen.height);
		}

		float width = maxX - minX;
		float height = maxY - minY;
		float area = width * height;
		float percentage = area / (Screen.width * Screen.height) * 100.0f;
		return percentage;
	}

	void Corners(GameObject go){
		Bounds bounds = go.GetComponent<MeshFilter> ().mesh.bounds;
		Vector3 v3Center = bounds.center;
		Vector3 v3Extents = bounds.extents;

		corners[0]  = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top left corner A
		corners[1]  = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z - v3Extents.z);  // Front top right corner D 
		corners[2]  = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom left corner E
		corners[3]  = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z - v3Extents.z);  // Front bottom right corner H
		corners[4]  = new Vector3(v3Center.x - v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top left corner B
		corners[5]  = new Vector3(v3Center.x + v3Extents.x, v3Center.y + v3Extents.y, v3Center.z + v3Extents.z);  // Back top right corner C
		corners[6]  = new Vector3(v3Center.x - v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom left corner F
		corners[7]  = new Vector3(v3Center.x + v3Extents.x, v3Center.y - v3Extents.y, v3Center.z + v3Extents.z);  // Back bottom right corner G
	}

	/*
	 * Returns the most likely subject based on the metrics derived for a photo object. Note that it changes the order
	 * of visibleObjs, but we can use OrderBy() in place of Sort() to avoid that side-effect.
	 */
	GameObject getSubject(List<GameObject> visibleObjs) {
		if (visibleObjs.Count <= 0) {
			return null;
		}
		visibleObjs.Sort (delegate(GameObject left, GameObject right) {
			// Exapand this to consider other photo object metrics
			Photographable leftPhotoObject = left.GetComponent<Photographable>();
			Photographable rightPhotoObject = right.GetComponent<Photographable>();

			if (leftPhotoObject.baseScore > rightPhotoObject.baseScore
				&& leftPhotoObject.percentOccluded < rightPhotoObject.percentOccluded) {
				return 1;
			} else if (leftPhotoObject.baseScore < rightPhotoObject.baseScore
				&& leftPhotoObject.percentOccluded > rightPhotoObject.percentOccluded) {
				return -1;
			} else return 0;
		});

		return visibleObjs[0];
	}

	/*
	 * Used for ALL heuristic evaluations - spacing, balance, interestingness, whatevs!
	 * All you have to do is write your heuristic functions and store them in a heuristic / weight mapping dictionary
	 * and this will do the rest.
	 */
	float evaluateHeuristics(List<GameObject> visibleObjs, Dictionary<System.Func<List<GameObject>, Camera, float>, float> heuristcs) {
		float metric = 0f;

		foreach (var func in heuristcs) {
			metric += func.Key (visibleObjs, cam) * func.Value;
		}

		return metric;
	}
}