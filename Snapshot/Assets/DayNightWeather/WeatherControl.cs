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

	//  Minor variables
	int transitionTimer;
	float originalTimeSet; 
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

		currentWeather = sunset;
		SetCurrentWeather (sunset, 1);
	}

	void SetCurrentWeather( WeatherProfile nextProfile , int stepsToTransition){
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
		timeOfDay = GetComponent<DayNightCycle>().timeOfDay;
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

		if (transitionTimer < 0) {
			//  Change the skybox depending on the time of the day
			//  Debug.Log("Time of day: " + timeOfDay);
			if (timeOfDay <= 0.05f && timeZone != 0) {
				//  Night
				SetCurrentWeather (cloudynight, 800);
				timeZone = 0;
			} else if (timeOfDay > 0.05f && timeOfDay <= .58f && timeZone != 1) {
				//  Sunrise/ sunset
				SetCurrentWeather (sunset, 800);
				timeZone = 1;
			} else if (timeOfDay > 0.58f && timeZone != 2) {
				//  Day
				SetCurrentWeather (sunny, 800);
				timeZone = 2;
			}
		}

	}
}
