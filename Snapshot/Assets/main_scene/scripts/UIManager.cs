using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
	private AudioSource pauseSource;

	public GameObject PanelPause;
	public GameObject PanelBag;
	public GameObject PanelControls;
    public SettingsManager settingsManager;
	public FirstPersonController player;
    public AchievementManager manager;
	public bool isPaused = false;
	public bool bagIsOpen = false;
    public bool cameraUP;
    public bool viewControls = false;

	// Use this for initialization
	void Start () {
        manager.loadAchievements();

		pauseSource = GetComponent<AudioSource> ();

		pauseSource.ignoreListenerPause = true;
		pauseSource.Play (); 
		pauseSource.Pause ();

        PanelControls.SetActive(viewControls);
        PanelPause.SetActive(isPaused);
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Cancel")) {
			isPaused = !isPaused;
		}
        if (Input.GetKeyDown(KeyCode.K)) {
            bagIsOpen = !bagIsOpen;
        }
        if (Input.GetButtonUp("View Controls")) {
            viewControls = !viewControls;
        }

        if (!isPaused) {
            PanelControls.SetActive(viewControls);
        }

        setPause(isPaused);
        OpenBag (bagIsOpen);
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
        player.m_MouseLook.SetCursorLock(!pauseState);
        PanelPause.SetActive(pauseState);
        AudioListener.pause = pauseState;

        if (pauseState) {
            // Do logic to pause
            Time.timeScale = 0.0f;

            pauseSource.Pause();

            PanelControls.SetActive(false);

            //Cursor.lockState = CursorLockMode.None;
        } else {
            // Do logic to unpause
            Time.timeScale = 1.0f;

            pauseSource.UnPause();

            PanelControls.SetActive(viewControls);
            settingsManager.exitSettings();

            //Cursor.lockState = CursorLockMode.Locked;
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
