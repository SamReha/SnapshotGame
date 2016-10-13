using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioManager : MonoBehaviour {
	public DayNightCycle dayNightCycle;

	private static AudioManager instance;

	float timeOfDay;
	public static string timeLabel;
	private bool isNoon = true;
	private float deerVolume = 0.0f;
	private float foxVolume = 0.0f;
	private float owlVolume = 0.0f;
	public  static bool exitToMenu = false;

	void Awake() {
		if (instance) {
			AudioManager.Destroy (AudioManager.getInstance ());
			Debug.LogError("More than one instance.");
		}
		instance = this;

		if (!exitToMenu) {
			Fabric.EventManager.Instance.PostEvent ("PlayMusic");
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Dawn", 1.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Morning", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Noon", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Afternoon", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Night", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox0", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer0", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl0", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox1", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer1", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl1", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox2", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer2", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl2", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox3", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer3", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl3", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox4", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer4", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl4", 0.0f, null);


			Fabric.EventManager.Instance.PostEvent ("PlayWeather");
			setWeatherVolume ("Clear", 0.5f);
			setWeatherVolume ("Rain", 0.0f);
			setWeatherVolume ("Cloudy", 0.0f);
			setWeatherVolume ("Overcast", 0.0f);
		}
	}

	// Use this for initialization
	void Start () {
		exitToMenu = exitToMenu; //NOT A REDUNDANT STATEMENT needed for the value to remain persistant - Jamie
		timeLabel = "Dawn";
	}
		

	void Update() {
		timeOfDay = dayNightCycle.getTimeOfDay ();

		if (timeOfDay >= 0f && timeOfDay < 0.125f) {
			timeLabel = "Dawn";
			setTimeVolume ();
		} else if (timeOfDay >= 0.125f && timeOfDay < 0.25f) {
			timeLabel = "Morning";
			setTimeVolume ();
		} else if (timeOfDay >= 0.25f && timeOfDay < 0.375f) {
			timeLabel = "Noon";
			setTimeVolume ();
		} else if (timeOfDay >= 0.375f && timeOfDay < 0.50f) {
			timeLabel = "Afternoon";
			setTimeVolume ();
		} else {
			timeLabel = "Night";
			setTimeVolume ();
		}
		updateAnimalVolume ();
	}

	private void silenceAll() {
		silenceDeer ();
		silenceFox ();
		silenceOwl ();
		silenceTime ();

		silenceWeather ();
	}

	public void silenceDeer(){
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer0", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer1", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer2", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer3", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer4", 0f, null);
	}

	public void setDeerVolume(float value)
	{
		deerVolume = value;

		switch (timeLabel) {
		case "Dawn":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer0", deerVolume, null);
			break;
		case "Morning":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer1", deerVolume, null);
			break;
		case "Noon":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer2", deerVolume, null);
			break;
		case "Afternoon":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer3", deerVolume, null);
			break;
		case "Night":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer4", deerVolume, null);
			break;
		default:
			silenceDeer ();
			break;
		}
	}

	public void silenceFox(){
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox0", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox1", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox2", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox3", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox4", 0f, null);
	}

	public void setFoxVolume(float value) {
		foxVolume = value;
		switch (timeLabel) {
		case "Dawn":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox0", foxVolume, null);
			break;
		case "Morning":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox1", foxVolume, null);
			break;
		case "Noon":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox2", foxVolume, null);
			break;
		case "Afternoon":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox3", foxVolume, null);
			break;
		case "Night":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox4", foxVolume, null);
			break;
		default:
			silenceFox ();
			break;
		}
	}

	public void silenceOwl(){
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl0", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl1", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl2", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl3", 0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl4", 0f, null);
	}

	public void setOwlVolume(float value) {
		owlVolume = value;
		switch (timeLabel) {
		case "Dawn":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl0", owlVolume, null);
			break;
		case "Morning":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl1", owlVolume, null);
			break;
		case "Noon":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl2", owlVolume, null);
			break;
		case "Afternoon":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl3", owlVolume, null);
			break;
		case "Night":
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl4", owlVolume, null);
			break;
		default:
			silenceOwl ();
			break;
		}
	}

	public void setExitToMenu(bool toggle) {
		exitToMenu = toggle;
	}

	public void setClearVolume(float value)
	{
		Fabric.EventManager.Instance.SetParameter("PlayWeather", "Clear", value, null);
	}

	public void setRainVolume(float value)
	{
		Fabric.EventManager.Instance.SetParameter("PlayWeather", "Rain", value, null);
	}

	public void silenceWeather (){
		setWeatherVolume ("Clear", 0.0f);
		setWeatherVolume ("Rain", 0.0f);
		setWeatherVolume ("Cloudy", 0.0f);
		setWeatherVolume ("Overcast", 0.0f);
	}

	public void setWeatherVolume(string weather, float value) {
		Fabric.EventManager.Instance.SetParameter("PlayWeather", weather, value, null);
	}

	public void silenceTime(){
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Dawn", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Morning", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Noon", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Afternoon", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Night", 0.0f, null);
	}

	public void setTimeVolume (){
		Fabric.EventManager.Instance.SetParameter ("PlayMusic", timeLabel, 1.0f, null);

		switch (timeLabel) {
		case "Dawn":
			silenceDeer ();
			silenceFox ();
			silenceOwl ();
			//Fabric.EventManager.Instance.SetParameter("PlayMusic", "Dawn", 1.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Morning", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Noon", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Afternoon", 0.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox0", foxVolume, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer0", deerVolume, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl0", owlVolume, null);

			break;
		case "Morning":
			silenceDeer ();
			silenceFox ();
			silenceOwl ();
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Dawn", 0.0f, null);
		   // Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Morning", 1.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox1", foxVolume, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer1", deerVolume, null);	
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl1", owlVolume, null);
			break;
		case "Noon":
			silenceDeer ();
			silenceFox ();
			silenceOwl ();
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Morning", 0.0f, null);
			//Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Noon", 1.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox2", foxVolume, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer2", deerVolume, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl2", owlVolume, null);
			break;
		case "Afternoon":
			silenceDeer ();
			silenceFox ();
			silenceOwl ();
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Noon", 0.0f, null);
			//Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Afternoon", 1.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox3", foxVolume, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer3", deerVolume, null);			
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl3", owlVolume, null);
			break;
		case "Night":
			silenceDeer ();
			silenceFox ();
			silenceOwl ();
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Afternoon", 0.0f, null);
			//Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Night", 1.0f, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Fox4", foxVolume, null);
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Deer4", deerVolume, null);			
			Fabric.EventManager.Instance.SetParameter ("PlayMusic", "Owl4", owlVolume, null);
			break;
		default:
			silenceDeer ();
			silenceFox ();
			silenceOwl ();
			break;
		}
	}

	public void setNoon() {
		isNoon = true;
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Noon", 1.0f, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Dawn", 0.0f, null);

		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Deer2", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Fox2", 0.0f, null);

		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Deer", deerVolume, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Fox", foxVolume, null);
	}

	public void setDawn() {
		isNoon = false;
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Noon", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Dawn", 1.0f, null);

		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Deer", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Fox", 0.0f, null);

		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Deer2", deerVolume, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Fox2", foxVolume, null);
	}

	public static AudioManager getInstance()
	{
		return instance;
	}

	/*
  	 * Sets each animals' volume as appropriate using the following rule:
  	 *     If the animal is further away from the player than the minThreshold, then that
  	 *     animal is too far away to be heard and it's layer is set to 0. If the animal is
  	 *     closer to the player than the maxThreshold, then the animal is close enough
  	 *     that it should be heard at max volume no matter what. If the animal is in between
  	 *     the two thresholds, then it uses the formula:
  	 *     volume = m(x-a) + b where m = -1/(minThreshold-maxThreshold), x = distance, a = maxThreshold and b = 1.0
  	 *     to get a value between 0.0 and 1.0 for distance between minThreshold and maxThreshold
  	 */
	private void updateAnimalVolume() {
		//Debug.Log (exitToMenu);
		/*
  		 * We can abstract this by placing all animals in a separate render layer and then
  		 * using a Physics.OverlapSphere to get all animals within a maximum distance. Then
  		 * we could simply get each animals' name and pass that to the audio manager with a
  		 * volume, and the audio manager can handle mapping animal names to specific layers. 
  		 * 		- Sam
  		 */
		float minThreshold = 60f;
		float maxThreshold = 25f;
		float deerDistance;
		float foxDistance;
		float owlDistance;

		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		GameObject deer = GameObject.Find ("Deer");
		GameObject fox = GameObject.Find ("Fox");
		GameObject owl = GameObject.Find ("Owl");

		if (deer == null) {
			Debug.Log ("AudioManager: updateAnimalVolume(): Can't find deer. Perhaps the name changed?");
			//RK: Adding return statements if something goes wrong prevents 
			//    a nullreferenceexception when deer distance is calculated
			return;  
		}
		if (fox == null) {
			Debug.Log ("AudioManager: updateAnimalVolume(): Can't find fox. Perhaps the name changed?");
			return;
		}
		if (owl == null) {
			Debug.Log ("AudioManager: updateAnimalVolume(): Can't find owl. Perhaps the name changed?");
			return;
		}

		deerDistance = Vector3.Distance (player.transform.position, deer.transform.position);
		foxDistance = Vector3.Distance (player.transform.position, fox.transform.position);
		owlDistance = Vector3.Distance (player.transform.position, owl.transform.position);

		if (deerDistance > minThreshold) {
			setDeerVolume (0f);
		} else if (deerDistance < maxThreshold) {
			setDeerVolume (1f);
		} else {
			// Animal is between thresholds
				float normalizedThreshold = minThreshold - maxThreshold;
				float volume = ((-(1 / normalizedThreshold)) * (deerDistance - maxThreshold)) + 1;
				setDeerVolume(volume);
		}

		if (foxDistance > minThreshold) {
			setFoxVolume (0f);
		} else if (foxDistance < maxThreshold) {
			setFoxVolume (1f);
		} else {
			// Animal is between thresholds
			float normalizedThreshold = minThreshold-maxThreshold;
			float volume = ((-(1/normalizedThreshold))*(foxDistance-maxThreshold)) + 1;
			setFoxVolume (volume);
		}
			
  		if (owlDistance > minThreshold) {
  			setOwlVolume (0f);
  		} else if (owlDistance < maxThreshold) {
  			setOwlVolume (1f);
  		} else {
  			// Animal is between thresholds
			float normalizedThreshold = minThreshold-maxThreshold;
  			float volume = ((-(1/normalizedThreshold))*(owlDistance-maxThreshold)) + 1;
  			setOwlVolume (volume);
   		}
	}

	private void updateTimeOfDayTracks () {

		if (timeLabel == "Dawn") {

		} else if (timeLabel == "Morning") {

		} else if (timeLabel == "Noon") {

		} else if (timeLabel == "Afternoon") {

		} else { // Else it's night

		}
	}
}
