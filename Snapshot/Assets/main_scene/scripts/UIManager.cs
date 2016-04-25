using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
	private AudioSource pauseSource;

	public GameObject PanelPause;
	public GameObject PanelBag;
	public GameObject PanelControls;
	public FirstPersonController player;
    public AchievementManager manager;
	public bool isPaused;
	public bool isOpen;
	public bool cameraUP;

	// Use this for initialization
	void Start () {
        manager.loadAchievements();
		isPaused = false;
		isOpen = false;

		pauseSource = GetComponent<AudioSource> ();

		pauseSource.ignoreListenerPause = true;
		pauseSource.Play (); 
		pauseSource.Pause ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Cancel")) {
			isPaused = !isPaused;
		}
		if (Input.GetKeyDown (KeyCode.K)) {
			isOpen = !isOpen;
		}
		if (Input.GetButtonUp ("View Controls")) {
			PanelControls.SetActive(!PanelControls.activeSelf);
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
			AudioListener.pause = true;

			pauseSource.UnPause ();
		} else {
			Time.timeScale = 1.0f;
			AudioListener.pause = false;

			pauseSource.Pause ();
		}
	}

	// A handy method for when passing args is difficult
	public void unPause() {
		isPaused = false;
	}

	public void exitPark() {
        manager.saveAchievements();
		//  Upload pictures from the camera to the photo buffer. 
		pauseSource.Stop();
		SceneManager.LoadScene("main_menu");
		AudioManager.getInstance().setExitToMenu (true);
	}
}
