using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class UIManager : MonoBehaviour {
	public GameObject PanelPause;
	public GameObject PanelBag;
	public FirstPersonController player;
	public bool isPaused;
	public bool isOpen;

	// Use this for initialization
	void Start () {
		isPaused = false;
		isOpen = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Cancel")) {
			isPaused = !isPaused;
		}
		if (Input.GetKeyDown (KeyCode.K)) {
			isOpen = !isOpen;
		}
		OpenBag (isOpen);
		setPause(isPaused);
	}

	public void OpenBag(bool bagState){
		//player.m_MouseLook.enabled = !bagState;
		player.m_MouseLook.SetCursorLock (!bagState);

		PanelBag.SetActive(bagState);
		if (bagState) {
			Time.timeScale = 0.0f;
		} else {
			Time.timeScale = 1.0f;
		}
	}

	public void setPause(bool pauseState) {
		//player.m_MouseLook.enabled = !pauseState;
		player.m_MouseLook.SetCursorLock (!pauseState);

		PanelPause.SetActive(pauseState);
		if (pauseState) {
			Time.timeScale = 0.0f;
		} else {
			Time.timeScale = 1.0f;
		}
	}

	// A handy method for when passing args is difficult
	public void unPause() {
		isPaused = false;
	}
}
