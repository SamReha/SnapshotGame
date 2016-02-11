using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisiblilityChecker : MonoBehaviour {
	
	public GameObject anObject;
	public Collider anObjCollider;
	private Camera cam;
	private Plane[] planes;

	public Vector3 viewPos;
	Vector3[] corners = new Vector3[8];
	public float percent = 0;
	public float framed = 0;

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
		
		float p = CalcScreenPercentage();
		//Debug.Log(gameObject.name + " uses " + p + " of the screen");
      
		viewPos = cam.WorldToViewportPoint(gameObject.transform.position);
		//print (viewPos.ToString("F4"));


		planes = GeometryUtility.CalculateFrustumPlanes(cam);

		if (GeometryUtility.TestPlanesAABB (planes, anObjCollider.bounds)) {
			List<GameObject> v = GameObject.Find ("Camera Prefab").GetComponent<Visible> ().visibleObjs;
			if (!v.Contains (gameObject)) {
				v.Add (gameObject);
			}
		} else {
			List<GameObject> v = GameObject.Find ("Camera Prefab").GetComponent<Visible> ().visibleObjs;
			if (v.Contains (gameObject)) {
				v.Remove (gameObject);
			}
		}
		CalcObjPercentage (corners);
		IsFramed ();
		print (framed);
	}

	// Roughly estimates the percentage of an object that is on screen using the AABB
	void CalcObjPercentage(Vector3[] v){
		percent = 100;
		int[] cornerLocations = new int[v.Length];	// An int array holds the positions of the corners of the bounding box
		for (int i = 0; i < v.Length; i++) {
			v[i] = transform.TransformPoint(v[i]);
			v[i] = cam.WorldToViewportPoint(v[i]);
			cornerLocations [i] = BoundsChecker (v [i]);
		}
		bool isAllInFrame = true;	// Edge case object completely in frame
		bool noneInFrame = true;	// Edge case object completely out of frame

		// Testing for isAllInFrame
		for (int i = 0; i < v.Length; i++) {
			isAllInFrame = false;
			break;
		}
		// Testing for noneInFrame
		for (int i = 0; i < v.Length; i++) {
			noneInFrame = false;
			break;
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
			return;
		} else { 
			// Caculate distance of all objects from edge of frame
			for (int i = 0; i < v.Length; i++) {
				CalcObjPoint (cornerLocations [i], v [i], 25f, -1.0f, 2.0f, -1.0f, 2.0f);
			}
			// Edge case can go below 0% dependent on proximity
			if (percent < 0) {
				percent = 0;
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
	void CalcObjPoint(int i, Vector3 v, float value, float minX, float maxX, float minY, float maxY){

		float x = v.x;
		float y = v.y;

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
	}


	// Checks how centered within the camera viewport the object is
	// At the moment all values calculated are arbitrary
	void IsFramed(){
		
		float x = viewPos.x;
		float y = viewPos.y;

		if (percent > 0) {
			if (x <= 0.6f && x >= 0.4f) {
				if (y <= 0.6f && y >= 0.4f) {
					framed = percent;	// If the object essentially centered 
				} else if (y > 0.6f && y <= 1.0f || y < 0.4f && y >= 0.0f) {
					framed = percent * 0.7f;	// If the object is centered along x but not y
				} else {
					framed = 0.0f;	// If the object is outside of frame
				}
			} else if (x > 0.6f && x <= 1.0f || x < 0.4f && x >= 0.0f) {
				if (y <= 0.6f && y >= 0.4f) {
					framed = percent * 0.7f;	// If the object is is centered along y but not x
				} else if (y > 0.6f && y <= 1.0f || y < 0.4f && y >= 0.0f) {
					framed = percent * 0.3f;	// If the object is not centered on x or y but stil in frame
				} else {
					framed = 0.0f;	// If the object is outside of frame 
				}
			} else {
				framed = 0.0f;	// If the object is outside of frame
			}
		}
	}

	// Calculates how much of the screen the object is taking up
	// Currently only used to calculated bounding box corners
	float CalcScreenPercentage() {
		
		float minX = float.PositiveInfinity;
		float minY = float.PositiveInfinity;
		float maxX = float.NegativeInfinity;
		float maxY = float.NegativeInfinity;

		Bounds bounds = gameObject.GetComponent<MeshFilter> ().mesh.bounds;
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
}