using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	private static AudioManager instance;

	private bool isNoon = true;
	private float deerVolume = 0.0f;
	private float foxVolume = 0.0f;

	void Awake()
	{
		if (instance)
		{
			Debug.LogError("More than one instance.");
		}
		instance = this;

		Fabric.EventManager.Instance.PostEvent("PlayMusic");
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Noon", 1.0f, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Fox", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Deer", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Fox2", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Deer2", 0.0f, null);


		Fabric.EventManager.Instance.PostEvent("PlayWeather");
		Fabric.EventManager.Instance.SetParameter("PlayWeather", "Clear", 0.5f, null);
		Fabric.EventManager.Instance.SetParameter("PlayWeather", "Rain", 0.0f, null);
	}

	// Use this for initialization
	void Start ()
	{
		
	}

	void Update () {
		updateAnimalVolume();
		updateTimeOfDayTrack ();
	}

	public void setDeerVolume(float value)
	{
		deerVolume = value;
		if(isNoon)
		{
			Fabric.EventManager.Instance.SetParameter("PlayMusic", "Deer", deerVolume, null);
		}
		else
		{
			Fabric.EventManager.Instance.SetParameter("PlayMusic", "Deer2", deerVolume, null);
		}
	}

	public void setFoxVolume(float value)
	{
		foxVolume = value;
		if(isNoon)
		{
			Fabric.EventManager.Instance.SetParameter("PlayMusic", "Fox", foxVolume, null);
		}
		else
		{
			Fabric.EventManager.Instance.SetParameter("PlayMusic", "Fox2", foxVolume, null);
		}
	}

	public void setClearVolume(float value)
	{
		Fabric.EventManager.Instance.SetParameter("PlayWeather", "Clear", value, null);
	}

	public void setRainVolume(float value)
	{
		Fabric.EventManager.Instance.SetParameter("PlayWeather", "Rain", value, null);
	}

	public void setNoon()
	{
		isNoon = true;
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Noon", 1.0f, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Dawn", 0.0f, null);

		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Deer2", 0.0f, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Fox2", 0.0f, null);

		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Deer", deerVolume, null);
		Fabric.EventManager.Instance.SetParameter("PlayMusic", "Fox", foxVolume, null);
	}

	public void setDawn()
	{
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
	 *     animal is too far away to to heard and it's layer is set to 0. If the animal is
	 *     closer to the player than the maxThreshold, then the animal is close enough
	 *     that it should be heard at max volume no matter what. If the animal is in between
	 *     the two thresholds, then it uses the formula:
	 *     volume = m(x-a) + b where m = -1/(minThreshold-maxThreshold), x = distance, a = maxThreshold and b = 1.0
	 *     to get a value between 0.0 and 1.0 for distance between minThreshold and maxThreshold
	 */
	private void updateAnimalVolume() {
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
		//float owlDistance;

		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		GameObject deer = GameObject.Find ("/Deer");
		GameObject fox = GameObject.Find ("/fox");
		GameObject owl = GameObject.Find ("/NewOwl");

		if (deer == null) {
			Debug.Log ("AudioManager: updateAnimalVolume(): Can't find deer. Perhaps the name changed?");
		}
		if (fox == null) {
			Debug.Log ("AudioManager: updateAnimalVolume(): Can't find fox. Perhaps the name changed?");
		}
		if (owl == null) {
			Debug.Log ("AudioManager: updateAnimalVolume(): Can't find owl. Perhaps the name changed?");
		}

		deerDistance = Vector3.Distance (player.transform.position, deer.transform.position);
		foxDistance = Vector3.Distance (player.transform.position, fox.transform.position);
		//owlDistance = Vector3.Distance (player.transform.position, owl.transform.position);

		if (deerDistance > minThreshold) {
			setDeerVolume (0f);
		} else if (deerDistance < maxThreshold) {
			setDeerVolume (1f);
		} else {
			// Animal is between thresholds
			float normalizedThreshold = minThreshold-maxThreshold;
			float volume = ((-(1/normalizedThreshold))*(deerDistance-maxThreshold)) + 1;
			setDeerVolume (volume);
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

		/* setOwlVolume not yet implemented
		if (owlDistance > minThreshold) {
			setOwlVolume (0f);
		} else if (owlDistance < maxThreshold) {
			setOwlVolume (1f);
		} else {
			// Animal is between thresholds
			float volume = ((-(1/normalizedThreshold))*(owlDistance-maxThreshold)) + 1;
			setOwlVolume (volume);
		}
		*/
	}

	/*
	 * Simply checks the DayNightCycle component to see what time of day it is, and
	 * sets the Time of Day track as appropriate.
	 */
	private void updateTimeOfDayTrack () {
		GameObject sun = GameObject.Find ("/sun");
		DayNightCycle dayNightCycle = sun.GetComponent<DayNightCycle> ();

		float time = dayNightCycle.getTimeOfDay ();

		if (time >= 0.0 && time < 0.25) {
			setDawn ();
		} else
			setNoon ();
	}
}
