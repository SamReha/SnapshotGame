using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericAgent : MonoBehaviour {

	public Transform playerPosition;
	Transform animalPosition;
	NavMeshAgent agent;
	public Vector3[] path_waypoints;// A list of waypoints that form the path of the creature
	public float meandering_radius = 15f; //  controls the size of the area around a path waypoint that an animal meanders within
	public float waypoint_detection_range = 5f; //  controls the size of the area around a path waypoint that an animal meanders within
	public float too_close = 10;
	public float safe_distance = 15;
	float fear_speed;
	public float calm_speed = 0.7f;
	public int meanderingExpirationTime = 100;
	public int poseExpirationTime = 5; //  Should be less than meandering time
	int meanderingExpirationCountdown = -1;  //  Set to negative to indicate inactive countdown
	int currentPathIndex = 0; //  Keeps track of which wandering waypoint in the list is active
	double oneSecond = 62.5;
	bool meandering = false;  //  Start the creature's wandering behavior to get to the first path
	bool afraid = false; 
	bool surprised = false; //  The first frame the animal is afraid, surprised is true
	int posingTimer = -1;
	//public float poseChance = 1f;
    Vector3 direct_waypoint;  //  This is the position that the animal always tries to follow
    public Animator anim;

	// Use this for initialization
	void Start () {
		playerPosition = GameObject.FindGameObjectWithTag ("Player").transform;
        anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent> ();
		animalPosition = this.gameObject.transform;
		agent.speed = calm_speed;
		fear_speed = calm_speed * 20;
		string animal = gameObject.name;

		if (animal == "Fox") {
			path_waypoints = new Vector3[8];
			path_waypoints [0] = (new Vector3 (-60f, -30.0f, 85f));
			path_waypoints [1] = (new Vector3 (-287f, -30.0f, 212.5f));
			path_waypoints [2] = (new Vector3 (-303f, -30.0f, 253f));
			path_waypoints [3] = (new Vector3 (-398f, -30.0f, 191f));
			path_waypoints [4] = (new Vector3 (-270f, -30.0f, 120f));
			path_waypoints [5] = (new Vector3 (-273f, -30.0f, -15f));
			path_waypoints [6] = (new Vector3 (-78f, -30.0f, -40f));
			path_waypoints [7] = (new Vector3 (81f, -30.0f, 22f));
		} else if (animal == "Deer") { 
			path_waypoints = new Vector3[9];
			path_waypoints [0] = (new Vector3 ( 67f, -30.0f, 27f));
			path_waypoints [1] = (new Vector3 ( 132f, -30.0f, 29f));
			path_waypoints [2] = (new Vector3 ( 194f, -30.0f, 148f));
			path_waypoints [3] = (new Vector3 ( 175f, -30.0f, 244f));
			path_waypoints [4] = (new Vector3 ( 278f, -30.0f, 256f));
			path_waypoints [5] = (new Vector3 ( 327f, -30.0f, 190f));
			path_waypoints [6] = (new Vector3 ( 229f, -30.0f, 45f));
			path_waypoints [7] = (new Vector3 ( 176f, -30.0f, -28f));
			path_waypoints [8] = (new Vector3 ( 157f, -30.0f, 21f));


		} else if (animal == "Owl") {
			path_waypoints = new Vector3[7];
			path_waypoints [0] = (new Vector3 ( -159f, -20.9f, 79f));
			path_waypoints [1] = (new Vector3 ( -289f, -20.9f, 139f));
			path_waypoints [2] = (new Vector3 ( -364f, -20.9f, 93f));
			path_waypoints [3] = (new Vector3 ( -356f, -20.9f, -7f));
			path_waypoints [4] = (new Vector3 ( -274f, -20.9f, -53f));
			path_waypoints [5] = (new Vector3 ( -152f, -20.9f, -58f));
			path_waypoints [6] = (new Vector3 ( -83f, -20.9f, 11f));

		} else {
			//  Load in the path of the creature here
			path_waypoints = new Vector3[15];// <--- Don't forget to update this number as the waypoint list grows
			path_waypoints [0] = (new Vector3 (-60f, -30.0f, 85f));
			path_waypoints [1] = (new Vector3 (-287f, -30.0f, 212.5f));
			path_waypoints [2] = (new Vector3 (-303f, -30.0f, 253f));
			path_waypoints [3] = (new Vector3 (-398f, -30.0f, 191f));
			path_waypoints [4] = (new Vector3 (-270f, -30.0f, 120f));
			path_waypoints [5] = (new Vector3 (-273f, -30.0f, -15f));
			path_waypoints [6] = (new Vector3 (-78f, -30.0f, -40f));
			path_waypoints [7] = (new Vector3 (81f, -30.0f, 22f));
			path_waypoints [8] = (new Vector3 (245f, -30.0f, 81f));
			path_waypoints [9] = (new Vector3 (324f, -30.0f, 124f));
			path_waypoints [10] = (new Vector3 (285f, -30.0f, -34f));
			path_waypoints [11] = (new Vector3 (101f, -30.0f, -131f));
			path_waypoints [12] = (new Vector3 (-65f, -30.0f, -92f));
			path_waypoints [13] = (new Vector3 (-165f, -30.0f, -44f));
			path_waypoints [14] = (new Vector3 (14f, -30.0f, 25f));
		}

		//  After loading in the path of the creature, set the direct Waypoint to the first in the array
		direct_waypoint = path_waypoints[currentPathIndex];
		agent.SetDestination (direct_waypoint);
	}

	// Update is called once per frame
	void Update () {
		oneSecond--;
		//Debug.Log("oneSecond: " + oneSecond + " meanderingCD: " + meanderingExpirationCountdown);
		if (oneSecond <= 0) {
			posingTimer--;//  Decrement the timer continuously
			meanderingExpirationCountdown--;//  Decrement the timer continuously
			oneSecond = 62.5;
		}

		float distFromPlayer = Vector3.Distance (transform.position, playerPosition.position);

		//  Change speed if animal is scared
		if (distFromPlayer <= too_close){
			if (!afraid && !surprised) {
				surprised = true;
			} else {
				surprised = false;
			}
			afraid = true;
			agent.speed = fear_speed;
		} else if (distFromPlayer >= safe_distance) {
			afraid = false;
			agent.speed = calm_speed;
		}
		//  Stop the animal if posing
		if (posingTimer >= 0) {
			//  Animal is posing
			agent.speed = 0;
		} 

		if (surprised) {
            playAnimation("Run");
			WanderToNextWaypoint ();
		}  
		if (meandering) { //  If the animal has reached their destination waypoint,
			float distanceToDirect = Vector3.Distance(animalPosition.position,  direct_waypoint);
			if (distanceToDirect < waypoint_detection_range) {
				//  If the animal reaches their direct waypoint while meandering,
				//  try to strike a pose.
				if (posingTimer < 0) {
					posingTimer = poseExpirationTime;
					//  Meandering hasn't ended but posing has, set the new waypoint to meander to
					direct_waypoint = GetNearbyWaypoint(path_waypoints[currentPathIndex]);
				} 
			} 
			if (meanderingExpirationCountdown <= 0) {
				//  If the timer has reached 0, it's time to start wandering to the next waypoint
				WanderToNextWaypoint ();
			} 
		} else { //  wandering behavior. 
			posingTimer = -1; //  Never pose when wandering
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
		agent.SetDestination (direct_waypoint);

		//  After we figure out the speed of creature, use it to determine what animation to play
		if (agent.speed <= 0) {
			playAnimation ("Idle");
		} else {
			playAnimation ("Walk");
		}
	}

	Vector3 GetNearbyWaypoint(Vector3 center){
    //  returns a new Transform that is near the given position
		return new Vector3(center.x + Random.Range(-meandering_radius, meandering_radius), 
                           center.y, center.z + Random.Range(-meandering_radius, meandering_radius));
	}

	void WanderToNextWaypoint() {
		//  Set wandering behavior
		currentPathIndex++; //  Tell the animal to go to the next waypoint in the path
		if (currentPathIndex > path_waypoints.Length - 1)
			currentPathIndex = 0; //  Loop the animal's trail
		meandering = false;
		//playOnAnimFinish ("Walk");
	}

	void playAnimation(string state, float normal_time = 0f ) {
		if (!anim.GetCurrentAnimatorStateInfo (0).IsName (state)) {
			anim.Play (state, -1, normal_time);
		}
	}
}
