using UnityEngine;
using System.Collections;

public class SnapshotCam : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (1)) {
			Application.CaptureScreenshot ("test.png");
			print ("Oh Snapshot");
		}
		if (Input.GetMouseButtonDown (0)) {
			GameObject.FindGameObjectWithTag ("Main Camera");
		}
	}
}
