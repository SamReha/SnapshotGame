﻿using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour {
    //  Allows for control over the change in sunlight color over time
    public Gradient nightDayColor;

    //  Limits on the sunlight brightness
    public float maxIntensity = 3f;
    public float minIntensity = 0f;

    //  Sun disappears slightly after the horizon
    public float minPoint = -0.2f;

    public float maxAmbient = 1f;
    public float minAmbient = .15f;
    public float minAmbientPoint = -0.2f;

    public Gradient nightDayFogColor;
    public AnimationCurve fogDensityCurve;
    public float fogScale = 1f;

    public float dayAtmosphereThickness = 0.4f;
    public float nightAtmosphereThickness = 0.87f;
    //  Change the speed of day/night independently
    public float dayRotateSpeed;
    public float nightRotateSpeed;
    //  Adjust the skySpeed constant to speed up/slow down the time of day
    float skySpeed = 1;

    Light mainLight;
    Skybox sky;
    Material skyMat;
    public ParticleSystem stars;
    public Transform moon;

	// Use this for initialization
	void Start () {
        mainLight = GetComponent<Light>();
        skyMat = RenderSettings.skybox;
	}
	

	void Update () {

        //  how much time the day takes up
        float tRange = 1 - minPoint;

        //  timeOfDay returns a value between 1 and -1
        //  where 1 is when the sun is at the highest point
        float timeOfDay = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minPoint) / tRange);

		skyMat.SetFloat("_Blend", timeOfDay);
		Debug.Log ("Time of day is: " + timeOfDay);

        //  Linearly change the sun's brightness depending on the time
        float i = ((maxIntensity - minIntensity) * timeOfDay) * minAmbient;
        RenderSettings.ambientIntensity = i;

        //  Set the sunlight to wherever the normalized sun position falls on the gradient.
        mainLight.color = nightDayColor.Evaluate(timeOfDay);

        //  Set the ambient light to the same color as the sunlight
        RenderSettings.ambientLight = mainLight.color;

        //  Determine fog color/density depending on the time of the day
        RenderSettings.fogColor = nightDayFogColor.Evaluate(timeOfDay);
        RenderSettings.fogDensity = fogDensityCurve.Evaluate(timeOfDay) * fogScale;

        //  Linearly change the atmosphere thickness
        i = ((dayAtmosphereThickness - nightAtmosphereThickness) * timeOfDay) + nightAtmosphereThickness;
        skyMat.SetFloat("_AtmosphereThickness", i);
		//  Blend the skyboxes

        //  Seperates the speed of night and day
        if (timeOfDay > 0) {
            transform.RotateAround(Vector3.zero, Vector3.right, dayRotateSpeed * Time.deltaTime * skySpeed);
            moon.transform.RotateAround(Vector3.zero, Vector3.right, dayRotateSpeed * Time.deltaTime * skySpeed);
        } else {
            transform.RotateAround(Vector3.zero, Vector3.right, nightRotateSpeed * Time.deltaTime * skySpeed);
            moon.transform.RotateAround(Vector3.zero, Vector3.right, nightRotateSpeed * Time.deltaTime * skySpeed);
        }
		//  Rotate the stars slower than the sun to give a sense of distance
		stars.transform.rotation = this.gameObject.transform.rotation;


        //  For debugging purposes, map keys to control day/night cycle speed
        if (Input.GetKeyDown(KeyCode.Z)) skySpeed *= 0.5f;
        if (Input.GetKeyDown(KeyCode.X)) skySpeed *= 2f;
        
    }
}
