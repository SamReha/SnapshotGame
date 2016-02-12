using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class UIManager : MonoBehaviour {
	public GameObject PanelPause;
	public FirstPersonController player;
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
		// Pass true to include inactive mouselooks
		//MouseLook[] mLooks = player.GetComponentsInChildren<MouseLook>(true);
		/*foreach (MouseLook mLook in mLooks){
			// enables MouseLook when pauseState is false and vice versa
			mLook.enabled = !pauseState;
		}*/
		player.m_MouseLook.enabled = !pauseState;
		player.m_MouseLook.SetCursorLock (!pauseState);

		PanelPause.SetActive(pauseState);
		if (pauseState) {
			Time.timeScale = 0.0f;
		} else {
			Time.timeScale = 1.0f;
		}

		Debug.Log ("It's working!!");
	}

	// A handy method for when passing args is difficult
	public void unPause() {
		isPaused = false;
	}
}
