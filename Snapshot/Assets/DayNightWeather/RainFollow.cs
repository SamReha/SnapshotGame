﻿using UnityEngine;
using System.Collections;

//  Responsible for making the rain particle emitter 
public class RainFollow : MonoBehaviour {

	public ParticleSystem raincloud;
	public WeatherControl weatherBox;

	public Transform playerPosition;
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		if (weatherBox.storming && !raincloud.isPlaying) {
			raincloud.Play ();
		} else if (!weatherBox.storming && raincloud.isPlaying) {
			raincloud.Stop ();
		}

		transform.position = new Vector3(playerPosition.position.x, playerPosition.position.y + 150, playerPosition.position.z);
	}
}
