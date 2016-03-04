using UnityEngine;
using System.Collections;

public class MainMenuUI : MonoBehaviour {

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}

	public void goToPark() {
		Application.LoadLevel ("SSV0.0");
	}

	public void goToShop() {
		Application.LoadLevel ("camera_shop");
	}

	public void goToBlog() {
		Application.LoadLevel ("blog");
	}
}
