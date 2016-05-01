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

	WeatherProfile sunny;
	public Texture sunny_top;
	public Texture sunny_front;
	public Texture sunny_bottom;
	public Texture sunny_left;
	public Texture sunny_right;
	public Texture sunny_back;

	WeatherProfile sunset;
	public Texture sunset_top;
	public Texture sunset_front;
	public Texture sunset_bottom;
	public Texture sunset_left;
	public Texture sunset_right;
	public Texture sunset_back;

	WeatherProfile sunrise;
	public Texture sunrise_top;
	public Texture sunrise_front;
	public Texture sunrise_bottom;
	public Texture sunrise_left;
	public Texture sunrise_right;
	public Texture sunrise_back;

	//  Minor variables
	int transitionTimer;
	float originalTimeSet; 
	Material skyMat;

	private bool pm = true;

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
		sunny = new WeatherProfile (sunny_top,
			sunny_front, sunny_bottom,
			sunny_left, sunny_right,
			sunny_back
		);
		sunset = new WeatherProfile (sunset_top,
			sunset_front, sunset_bottom,
			sunset_left, sunset_right,
			sunset_back
		);
		sunrise = new WeatherProfile (sunrise_top,
			sunrise_front, sunrise_bottom,
			sunrise_left, sunrise_right,
			sunrise_back
		);

		currentWeather = cloudynight;
		SetCurrentWeather (cloudynight, 1);
	}

	void SetCurrentWeather( WeatherProfile nextProfile , int stepsToTransition){
		
		//  Don't waste time if the current profile is requested
		//if (nextProfile.Equals (currentWeather)) { return; }

		WeatherProfile oldProf = currentWeather;
		skyMat.SetTexture("_FrontTex", oldProf.front);
		skyMat.SetTexture("_BackTex", oldProf.back);
		skyMat.SetTexture("_DownTex", oldProf.bottom);
		skyMat.SetTexture("_LeftTex", oldProf.left);
		skyMat.SetTexture("_RightTex", oldProf.right);
		skyMat.SetTexture("_UpTex", oldProf.top);
		currentWeather = nextProfile;
		skyMat.SetTexture("_FrontTex2", currentWeather.front);
		skyMat.SetTexture("_BackTex2", currentWeather.back);
		skyMat.SetTexture("_DownTex2", currentWeather.bottom);
		skyMat.SetTexture("_LeftTex2", currentWeather.left);
		skyMat.SetTexture("_RightTex2", currentWeather.right);
		skyMat.SetTexture("_UpTex2", currentWeather.top);
		transitionTimer = stepsToTransition;  //  Transition timer decrements every step.
		originalTimeSet = stepsToTransition;  //  Keeps track of the original time to animate the skybox
	}

	// Update is called once per frame
	void Update () {
		float timeOfDay;
		int timeZone;
		timeOfDay = GetComponent<DayNightCycle>().getTimeOfDay();
		timeZone = -1;  //  Prevents weather switcher from calling twice
        //  Decrement the steps timer
		transitionTimer--;
		//Debug.Log ("Time:   " + timeOfDay);
		if (transitionTimer >= 0) {
			float progress = (originalTimeSet - transitionTimer) / originalTimeSet; /*  timePassed/total */
			skyMat.SetFloat ("_Blend", progress);
		} else {
			skyMat.SetFloat ("_Blend", 0);
		}

		//Debug.Log ("PM: " + pm + " TOD: " + timeOfDay + " Tick: " + transitionTimer);

		float sunriseTrigger = 0.0f;
		float dayTrigger = 0.02f;
		float sunsetTrigger = 0.375f;
		float nightTrigger = 0.45f;

		//  If the weathercontroller is not already busy mreging weathers
		if (transitionTimer < 0) {
			if (timeOfDay > 0.45f) {
				if (timeOfDay > 0) {
					pm = true;
				} else {
					pm = false;
				}
			} else {
				pm = false;
			}

			//  Change the skybox depending on the time of the day
			//  Debug.Log("Time of day: " + timeOfDay);
			if (timeOfDay >= nightTrigger && pm) {
				//  Night
				Debug.Log ("Night: " + timeOfDay);
				SetCurrentWeather (cloudynight, 1600);
			} else if (timeOfDay >= sunriseTrigger && timeOfDay < dayTrigger && !pm) {
				//  Sunrise
				Debug.Log ("Sunrise: " + timeOfDay);
				SetCurrentWeather (sunrise, 1200);
			} else if (timeOfDay >= dayTrigger && timeOfDay < sunsetTrigger && !pm) {
				//  Day
				Debug.Log ("Day: " + timeOfDay);
				SetCurrentWeather (sunny, 1200);
			} else if (timeOfDay >= sunsetTrigger && timeOfDay < nightTrigger && !pm) {
				//  sunset
				Debug.Log ("Sunset: " + timeOfDay);
				SetCurrentWeather (sunset, 200);
			} 
		}

	}
}
