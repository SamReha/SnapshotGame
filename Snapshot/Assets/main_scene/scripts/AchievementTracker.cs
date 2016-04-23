using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class AchievementTracker : MonoBehaviour {
    public AchievementManager manager;
    public FirstPersonController player;

    private float timeOfLastUpdate;
    private Vector3 lastKnownPosition;
    private float distanceTraveled;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - timeOfLastUpdate >= 1.0f) {
            distanceTraveled = Vector3.Distance(lastKnownPosition, player.transform.position);
            lastKnownPosition = player.transform.position;
            manager.AddProgressToAchievement("Day Tripper", distanceTraveled);
            manager.AddProgressToAchievement("Marathoner", distanceTraveled);
            manager.AddProgressToAchievement("Seasoned Hiker", distanceTraveled);
            manager.AddProgressToAchievement("Master Backpacker", distanceTraveled);
            timeOfLastUpdate = Time.time;
        }
	}
}
