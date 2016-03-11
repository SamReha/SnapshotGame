using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericAgent : MonoBehaviour {

	Transform animalPosition;
	NavMeshAgent agent;
	public Vector3[] path_waypoints;// A list of waypoints that form the path of the creature
	public float meandering_radius = 15f; //  controls the size of the area around a path waypoint that an animal meanders within
	public float waypoint_detection_range = 5f; //  controls the size of the area around a path waypoint that an animal meanders within
	public int meanderingExpirationTime = 100;
	int meanderingExpirationCountdown = -1;  //  Set to negative to indicate inactive countdown
	int currentPathIndex = 0; //  Keeps track of which wandering waypoint in the list is active
	double oneSecond = 62.5;
	bool meandering = false;  //  Start the creature's wandering behavior to get to the first path
    Vector3 direct_waypoint;  //  This is the position that the animal always tries to follow
    public Animator anim;

	// Use this for initialization
	void Start () {
		Debug.Log ("Nav Mesh Angent Start");
        anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent> ();
		animalPosition = this.gameObject.transform;
		//  Load in the path of the creature here
		path_waypoints = new Vector3[3];// <--- Don't forget to update this number as the waypoint list grows
		path_waypoints[0] =  (new Vector3(-208.5f, -10.0f, 194.4f));
		path_waypoints[1] =  (new Vector3(187.2f, -10.0f, 214.4f));
		path_waypoints[2] =  (new Vector3(187.2f, -10.0f, -76.7f));
		//  After loading in the path of the creature, set the direct Waypoint to the first in the array
		direct_waypoint = path_waypoints[currentPathIndex];
		agent.SetDestination (direct_waypoint);
	}

	// Update is called once per frame
	void Update () {
		oneSecond--;
		//Debug.Log("oneSecond: " + oneSecond + " meanderingCD: " + meanderingExpirationCountdown);
		if (oneSecond <= 0) {
			meanderingExpirationCountdown--;//  Decrement the timer continuously
			oneSecond = 62.5;
		}
		if (meandering){ //  If the animal has reached their destination waypoint,
			float distanceToDirect = Vector3.Distance(animalPosition.position,  direct_waypoint);
            anim.Play("Run", -1, 0f);
			if (distanceToDirect < waypoint_detection_range){
				//  If the animal reaches their direct waypoint while meandering, don't stop moving.
				//  instead, give the animal another position to go to.
				direct_waypoint = GetNearbyWaypoint(path_waypoints[currentPathIndex]);
			}
			if (meanderingExpirationCountdown <= 0){
            //  If the timer has reached 0, it's time to start wandering to the next waypoint
				currentPathIndex++; //  Tell the animal to go to the next waypoint in the path
				if (currentPathIndex > path_waypoints.Length - 1) currentPathIndex = 0; //  Loop the animal's trail
				meandering = false;
                anim.Play("Run", -1, 0f);
			}
		} else { //  wandering behavior. 
			direct_waypoint = path_waypoints[currentPathIndex];
			//  Get the distance from the path wayPoint
			float distanceToPath = Vector3.Distance(animalPosition.position, path_waypoints[currentPathIndex]);
			if (distanceToPath < waypoint_detection_range) {
				//  reset the timer that will eventually stop the meandering behavior
				meanderingExpirationCountdown = meanderingExpirationTime;
				direct_waypoint = GetNearbyWaypoint(path_waypoints[currentPathIndex]);
				meandering = true;
			}
		}
		//  Finally, after deciding where to go tell the NavMeshAgent where to go
		//agent.SetDestination (direct_waypoint);
	}

	Vector3 GetNearbyWaypoint(Vector3 center){
    //  returns a new Transform that is near the given position
		return new Vector3(center.x + Random.Range(-meandering_radius, meandering_radius), 
			                 center.y, center.z + Random.Range(-meandering_radius, meandering_radius));
	}
		
}
