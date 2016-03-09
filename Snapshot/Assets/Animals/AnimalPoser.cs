using UnityEngine;
using System.Collections;

public class AnimalPoser : MonoBehaviour {

    bool stateChange; // Determines if a behaviour change is needed.
    int behaviourTime = 0; // How long until this animal should change it's behaviour
    float stateValue = 0f; // The value of the state it should be in.
    float waitTime = 1f; // The amount time the program should wait before being able to pick another state.  This is to stop multiple states being picked at once.
    float changeTime = 0f; // The value of time that the script will check before picking a state.
    public Animator anim; // Create an Animator Object for the script.

    // Use this for initialization
    void Start()
    {
        stateChange = false;
        behaviourTime = Random.Range(10, 15); // Pick a random time from 30 seconds to 60 seconds to Start.
        print(behaviourTime);
        anim = GetComponent<Animator>(); //Get the Animator Component attached to the Unity object.
    }
	
	// Update is called once per frame
	void Update () {
	    if((int)Time.time % behaviourTime == 0 && Time.time > changeTime) // Check to see if it is time to change behaviour.  This will happen at start.
        {
            stateChange = true;
        }
        if(stateChange == true)
        {
            behaviourChange();
            changeTime = Time.time + waitTime; // Make sure the script does not call multiple behaviour states.
        }
	}

    void behaviourChange()
    {
        stateValue = Random.value; // Pick a random state for the animal
        if(stateValue <= .5f)
        {
            print("State is Idle");
			// Call the animation from the base layer of the attached Animator Controller
			// and start at the first frame of the animation.
            anim.Play("Idle", -1, 0f); 
            behaviourTime = Random.Range(10, 15); // Pick a new amount of time to wait until next behaviour
        }
        else if(stateValue > .5f && stateValue <= .75f)
        {
            print("State is Move to Destination");
            anim.Play("Run", -1, 0f);
            behaviourTime = Random.Range(10, 15);
        }
        else
        {
            print("State is Meandering");
            anim.Play("Fox_Curious", -1, 0f);
            behaviourTime = Random.Range(10, 15);
        }
        stateChange = false;
    }
}
