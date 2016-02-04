using UnityEngine;
using System.Collections;

public class WeatherControl : MonoBehaviour {

	WeatherProfile currentWeather;

	WeatherProfile cloudynight;
	public Texture cloudynight_top;
	public Texture cloudynight_front;
	public Texture cloudynight_bottom;
	public Texture cloudynight_left;
	public Texture cloudynight_right;
	public Texture cloudynight_back;

	WeatherProfile overcast;
	public Texture overcast_top;
	public Texture overcast_front;
	public Texture overcast_bottom;
	public Texture overcast_left;
	public Texture overcast_right;
	public Texture overcast_back;

	//  Minor variables
	int transitionTimer;
	int originalTimeSet; 
	Material skyMat;

	// Use this for initialization
	void Start () {
		skyMat = RenderSettings.skybox;
		//  Create the weather profiles
		cloudynight = new WeatherProfile (cloudynight_top,
			cloudynight_front, cloudynight_bottom,
			cloudynight_left, cloudynight_right,
			cloudynight_back
		);
		overcast = new WeatherProfile (overcast_top,
			overcast_front, overcast_bottom,
			overcast_left, overcast_right,
			overcast_back
		);

		SetCurrentWeather (overcast, 0);
	}

	void SetCurrentWeather( WeatherProfile nextProfile , int stepsToTransition){
		currentWeather = nextProfile;
		transitionTimer = stepsToTransition;  //  Transition timer decrements every step.
		originalTimeSet = stepsToTransition;  //  Keeps track of the original time to animate the skybox
	}

	// Update is called once per frame
	void Update () {
        //  Decrement the steps timer
		transitionTimer--;
		if (transitionTimer > 0) {
			float progress = (originalTimeSet - transitionTimer) / originalTimeSet; /*  timePassed/total */
			skyMat.SetFloat ("_Blend", progress);
		} else {
			skyMat.SetFloat ("_Blend", 1);
		}
	}
}
