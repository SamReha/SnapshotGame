using UnityEngine;
using System.Collections;

public class LightningController : MonoBehaviour {
	
	public WeatherControl weatherBox;
	public Light flash;
	private int lightningTimer = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		int chance = Random.Range (30,1001);
		if (chance > 995 && lightningTimer < -100) {
			lightningTimer = 5;
		}
		
		if (lightningTimer > 0) {
			flash.enabled = true;
		} else {
			flash.enabled = false;
		}

		lightningTimer -= 1;
	}
}
