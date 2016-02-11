using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class UIManager : MonoBehaviour {
	public GameObject PanelPause;
	public bool isPaused;

	// Use this for initialization
	void Start () {
		isPaused = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Cancel")) {
			isPaused = !isPaused;
		}
		setPause(isPaused);
	}

	public void setPause(bool pauseState) {
		GameObject player = GameObject.FindWithTag("Player");
		// For some reason, this line causes who menu to break and seems to trigger
		// null pointer excep in visibility checker as well.
		//player.GetComponent<FirstPersonController>().enabled = pauseState;

		PanelPause.SetActive(pauseState);
		if (pauseState) {
			Time.timeScale = 0.0f;
		} else
			Time.timeScale = 1.0f;
	}

	// A handy method for when passing args is difficult
	public void unPause() {
		isPaused = false;
	}
}
