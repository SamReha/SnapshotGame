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

	List<Vector3> waypoints = new List<Vector3>{			
		new Vector3 (258f, -30f, 165f),
		new Vector3 (335f, -30f, 106f),
		new Vector3( 270f, -30f, 67f),
		new Vector3( 295f, -30f, 28f),
		new Vector3( 247f, -30f, 14f),
		new Vector3( 209f, -30f, -24f),
		new Vector3( 302f, -30f, -10f),
		new Vector3( 372f, -30f, 50f),
		new Vector3( 319f, -30f, -31f),
		new Vector3( 273f, -30f, -66f),
		new Vector3( 394f, -30f, 30f),
		new Vector3( 393f, -30f, -97f),
		new Vector3( 347f, -30f, -70f),
		new Vector3( 280f, -30f, -97f),
		new Vector3( 198f, -30f, -61f),
		new Vector3( 120f, -30f, -58f),
		new Vector3( 91f, -30f, -37f),
		new Vector3( 107f, -30f, 6f),
		new Vector3( 32f, -30f, -75f),
		new Vector3( 32f, -30f, -22f),
		new Vector3( 38f, -30f, 36f),
		new Vector3( -10f, -30f, 66f),
		new Vector3( 67f, -30f, 77f),
		new Vector3( 125f, -30f, 45f),
		new Vector3( 175f, -30f, 7f),
		new Vector3( 177f, -30f, 43f)
	};


	// Use this for initialization
	void Start () {
		playerPosition = GameObject.FindGameObjectWithTag ("Player").transform;
        anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent> ();
		animalPosition = this.gameObject.transform;
		agent.speed = calm_speed;
		fear_speed = calm_speed * 20;
		string animal = gameObject.name;
		path_waypoints = new Vector3[6];
		List<int> points;
		for (int i = 0; i <= path_waypoints.Length; i++) {
			int x = Random.Range (0, 25);
			while (points.Contains (x)) {
				x = Random.Range (0, 25);
			}
			points.Add (x);
			path_waypoints [i] = waypoints [x];
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
		} else if (agent.speed >= fear_speed) {
            playAnimation("Run");
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
