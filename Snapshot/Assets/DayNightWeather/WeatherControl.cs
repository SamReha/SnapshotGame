using UnityEngine;
using System.Collections;

public class WeatherControl : MonoBehaviour {

	public WeatherProfile currentWeather;

	public int chanceOfRain = 20;

	private float sunriseTrigger = 0.0f;
	private float dayTrigger = 0.02f;
	private float sunsetTrigger = 0.375f;
	private float nightTrigger = 0.45f;

	WeatherProfile cloudynight;
	public Texture cloudynight_top;
	public Texture cloudynight_front;
	public Texture cloudynight_bottom;
	public Texture cloudynight_left;
	public Texture cloudynight_right;
	public Texture cloudynight_back;
	public Gradient cloudynight_dayNightColor;
	public float cloudynight_maxIntensity;
	public float cloudynight_minIntensity;
	public float cloudynight_maxAmbient;
	public float cloudynight_minAmbient;
	public Gradient cloudynight_nightDayFogColor;
	public AnimationCurve cloudynight_fogDensityCurve;
	public float cloudynight_fogScale;
	public float cloudynight_dayAtmosphereThickness;
	public float cloudynight_nightAtmosphereThickness;

	WeatherProfile overcast;
	public Texture overcast_top;
	public Texture overcast_front;
	public Texture overcast_bottom;
	public Texture overcast_left;
	public Texture overcast_right;
	public Texture overcast_back;
	public Gradient overcast_dayNightColor;
	public float overcast_maxIntensity;
	public float overcast_minIntensity;
	public float overcast_maxAmbient;
	public float overcast_minAmbient;
	public Gradient overcast_nightDayFogColor;
	public AnimationCurve overcast_fogDensityCurve;
	public float overcast_fogScale;
	public float overcast_dayAtmosphereThickness;
	public float overcast_nightAtmosphereThickness;

	WeatherProfile sunny;
	public Texture sunny_top;
	public Texture sunny_front;
	public Texture sunny_bottom;
	public Texture sunny_left;
	public Texture sunny_right;
	public Texture sunny_back;
	public Gradient sunny_dayNightColor;
	public float sunny_maxIntensity;
	public float sunny_minIntensity;
	public float sunny_maxAmbient;
	public float sunny_minAmbient;
	public Gradient sunny_nightDayFogColor;
	public AnimationCurve sunny_fogDensityCurve;
	public float sunny_fogScale;
	public float sunny_dayAtmosphereThickness;
	public float sunny_nightAtmosphereThickness;

	WeatherProfile sunset;
	public Texture sunset_top;
	public Texture sunset_front;
	public Texture sunset_bottom;
	public Texture sunset_left;
	public Texture sunset_right;
	public Texture sunset_back;
	public Gradient sunset_dayNightColor;
	public float sunset_maxIntensity;
	public float sunset_minIntensity;
	public float sunset_maxAmbient;
	public float sunset_minAmbient;
	public Gradient sunset_nightDayFogColor;
	public AnimationCurve sunset_fogDensityCurve;
	public float sunset_fogScale;
	public float sunset_dayAtmosphereThickness;
	public float sunset_nightAtmosphereThickness;

	WeatherProfile sunrise;
	public Texture sunrise_top;
	public Texture sunrise_front;
	public Texture sunrise_bottom;
	public Texture sunrise_left;
	public Texture sunrise_right;
	public Texture sunrise_back;
	public Gradient sunrise_dayNightColor;
	public float sunrise_maxIntensity;
	public float sunrise_minIntensity;
	public float sunrise_maxAmbient;
	public float sunrise_minAmbient;
	public Gradient sunrise_nightDayFogColor;
	public AnimationCurve sunrise_fogDensityCurve;
	public float sunrise_fogScale;
	public float sunrise_dayAtmosphereThickness;
	public float sunrise_nightAtmosphereThickness;

	//  Minor variables
	int transitionTimer;
	float originalTimeSet; 
	Material skyMat;

	public bool storming;

	private bool pm = true;

	// Use this for initialization
	void Start () {
		storming = false;
		skyMat = RenderSettings.skybox;
		//  Create the weather profiles
		cloudynight = new WeatherProfile ("cloudy", cloudynight_top,
			cloudynight_front, cloudynight_bottom,
			cloudynight_left, cloudynight_right,
			cloudynight_back, cloudynight_dayNightColor, cloudynight_maxIntensity, cloudynight_minIntensity, cloudynight_maxAmbient, cloudynight_minAmbient,
			cloudynight_nightDayFogColor, cloudynight_fogDensityCurve, cloudynight_fogScale, cloudynight_dayAtmosphereThickness,
			cloudynight_nightAtmosphereThickness 
		);
		overcast = new WeatherProfile ("overcast", overcast_top,
			overcast_front, overcast_bottom,
			overcast_left, overcast_right,
			overcast_back, overcast_dayNightColor, overcast_maxIntensity, overcast_minIntensity, overcast_maxAmbient, overcast_minAmbient,
			overcast_nightDayFogColor, overcast_fogDensityCurve, overcast_fogScale, overcast_dayAtmosphereThickness,
			overcast_nightAtmosphereThickness 
		);
		sunny = new WeatherProfile ("sunny", sunny_top,
			sunny_front, sunny_bottom,
			sunny_left, sunny_right,
			sunny_back, sunny_dayNightColor, sunny_maxIntensity, sunny_minIntensity, sunny_maxAmbient, sunny_minAmbient,
			sunny_nightDayFogColor, sunny_fogDensityCurve, sunny_fogScale, sunny_dayAtmosphereThickness,
			sunny_nightAtmosphereThickness 
		);
		sunset = new WeatherProfile ("sunset", sunset_top,
			sunset_front, sunset_bottom,
			sunset_left, sunset_right,
			sunset_back, sunset_dayNightColor, sunset_maxIntensity, sunset_minIntensity, sunset_maxAmbient, sunset_minAmbient,
			sunset_nightDayFogColor, sunset_fogDensityCurve, sunset_fogScale, sunset_dayAtmosphereThickness,
			sunset_nightAtmosphereThickness 
		);
		sunrise = new WeatherProfile ("sunrise", sunrise_top,
			sunrise_front, sunrise_bottom,
			sunrise_left, sunrise_right,
			sunrise_back, sunrise_dayNightColor, sunrise_maxIntensity, sunrise_minIntensity, sunrise_maxAmbient, sunrise_minAmbient,
			sunrise_nightDayFogColor, sunrise_fogDensityCurve, sunrise_fogScale, sunrise_dayAtmosphereThickness,
			sunrise_nightAtmosphereThickness 
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
		timeOfDay = GetComponent<DayNightCycle>().getTimeOfDay();

        //  Decrement the steps timer
		transitionTimer--;
		if (transitionTimer >= 0) {
			float progress = (originalTimeSet - transitionTimer) / originalTimeSet; /*  timePassed/total */
			skyMat.SetFloat ("_Blend", progress);
		} else {
			skyMat.SetFloat ("_Blend", 0);
		}

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
				if (Random.Range (0, 100) > 100-chanceOfRain) {
					SetCurrentWeather (overcast, 1600);
					storming = true;
				} else {
					SetCurrentWeather (cloudynight, 1600);
					storming = false;
				}
			} else if (timeOfDay >= sunriseTrigger && timeOfDay < dayTrigger && !pm) {
				//  Sunrise
				if (Random.Range (0, 100) > 100-chanceOfRain) {
					SetCurrentWeather (overcast, 1200);
					storming = true;
				} else {
					SetCurrentWeather (sunrise, 1200);
					storming = false;
				}
			} else if (timeOfDay >= dayTrigger && timeOfDay < sunsetTrigger && !pm) {
				//  Day
				if (Random.Range (0, 100) > 100-chanceOfRain) {
					SetCurrentWeather (overcast, 1200);
					storming = true;
				} else {
					SetCurrentWeather (sunny, 1200);
					storming = false;
				}
			} else if (timeOfDay >= sunsetTrigger && timeOfDay < nightTrigger && !pm) {
				//  sunset
				if (Random.Range (0, 100) > 100-chanceOfRain) {
					SetCurrentWeather (overcast, 200);
					storming = true;
				} else {
					SetCurrentWeather (sunset, 200);
					storming = false;
				}
			} 
		}

	}
}
