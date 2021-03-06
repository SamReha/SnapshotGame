﻿using UnityEngine;
using System;
using System.Collections;

public class DayNightCycle : MonoBehaviour {
    //  Allows for control over the change in sunlight color over time
    public Gradient nightDayColor;

    //  Limits on the sunlight brightness
    public float maxIntensity = 3f;
    public float minIntensity = 0f;

    //  Sun disappears slightly after the horizon
    public float minPoint = 0.2f;
    //public float maxAmbient = 1f;
    //public float minAmbient = .15f;
	public float minAmbientPoint = -0.2f;
    //public Gradient nightDayFogColor;
    //public AnimationCurve fogDensityCurve;
    //public float fogScale = 1f;
    //public float dayAtmosphereThickness = 0.4f;
    //public float nightAtmosphereThickness = 0.87f;

    //  Change the speed of day/night independently
    public float dayRotateSpeed;
    public float nightRotateSpeed;
    //  Adjust the skySpeed constant to speed up/slow down the time of day
    float skySpeed = .16666f;

    Light mainLight;
    Skybox sky;
    Material skyMat;
    public ParticleSystem stars;
    public Transform moon;
	public Material moonMaterial;
	public float constantTime;
	WeatherControl weatherController;

	// Use this for initialization
	void Start () {
		mainLight = GetComponent<Light>();
		weatherController = GetComponent<WeatherControl>();
        skyMat = RenderSettings.skybox;

		//skySpeed = 0.01;
	}
	

	void Update () {

        //  how much time the day takes up
        float tRange = 1 - minPoint;
        //  timeOfDay returns a value between 1 and -1
        //  where 1 is when the sun is at the highest point
	    constantTime = Vector3.Dot(mainLight.transform.forward, Vector3.down);
		float clampedTime = Mathf.Clamp01((constantTime - minPoint) / tRange);

		//Debug.Log ("Time of day is: " + timeOfDay);

        //  Linearly change the sun's brightness depending on the time
		float i = ((maxIntensity - minIntensity) * clampedTime) * weatherController.currentWeather.minAmbient;
        RenderSettings.ambientIntensity = i;

        //  Set the sunlight to wherever the normalized sun position falls on the gradient.
		mainLight.color = nightDayColor.Evaluate(clampedTime);

        //  Set the ambient light to the same color as the sunlight
        RenderSettings.ambientLight = mainLight.color;
		moonMaterial.SetColor ("_EmissionColor", mainLight.color);
		skyMat.color = mainLight.color;

        //  Determine fog color/density depending on the time of the day
		RenderSettings.fogColor = weatherController.currentWeather.nightDayFogColor.Evaluate(clampedTime);
		RenderSettings.fogDensity = weatherController.currentWeather.fogDensityCurve.Evaluate(clampedTime) * weatherController.currentWeather.fogScale;

        //  Linearly change the atmosphere thickness
		i = ((weatherController.currentWeather.dayAtmosphereThickness - weatherController.currentWeather.nightAtmosphereThickness) * clampedTime) + weatherController.currentWeather.nightAtmosphereThickness;
        skyMat.SetFloat("_AtmosphereThickness", i);
		//  Blend the skyboxes

        //  Seperates the speed of night and day
        if (constantTime > 0) {
            transform.RotateAround(Vector3.zero, Vector3.right, dayRotateSpeed * Time.deltaTime * skySpeed);
            moon.transform.RotateAround(Vector3.zero, Vector3.right, dayRotateSpeed * Time.deltaTime * skySpeed);
			stars.startLifetime = 0;
        } else {
            transform.RotateAround(Vector3.zero, Vector3.right, nightRotateSpeed * Time.deltaTime * skySpeed);
			moon.transform.RotateAround(Vector3.zero, Vector3.right, nightRotateSpeed * Time.deltaTime * skySpeed);
			stars.startLifetime = 10;
        }
		//  Rotate the stars slower than the sun to give a sense of distance
		stars.transform.rotation = this.gameObject.transform.rotation;
    }

	/*
 	 * Preferred to simply accessing timeOfDay, as timeOfDay doesn't differentiate between
 	 * dusk and dawn.
 	 * 
 	 * Returns 0.00 -> 1.00 where
 	 *     0.00 = dawn
 	 *     0.25 = noon
 	 *     0.50 = sunset
 	 *     0.75 = midnight
 	 *     1.00 = dawn (again)
 	 *     It's probably impossible for 0.00 or 1.00 precisely to be returned, so always
 	 *     check what time it is by a range.
 	 */
	public float getTimeOfDay() {
		float sunXCoord = mainLight.transform.up.z; // THIS IS NOT A TYPO
		float sunYCoord = mainLight.transform.up.y;
		double radianTime = Math.Atan2(sunXCoord, sunYCoord);
		double realTime = -(radianTime / (2 * Math.PI)) + 0.5;
		return (float)realTime;
	}
}
