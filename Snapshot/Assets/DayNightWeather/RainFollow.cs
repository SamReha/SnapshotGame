using UnityEngine;
using System.Collections;

//  Responsible for making the rain particle emitter 
public class RainFollow : MonoBehaviour {

	public Transform playerPosition;
	public ParticleSystem raincloud;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		raincloud.transform.position.Set (playerPosition.position.x, raincloud.transform.position.y , playerPosition.position.z);
	}
}
