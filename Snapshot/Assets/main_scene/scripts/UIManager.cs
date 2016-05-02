using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
	private AudioSource pauseSource;

	public GameObject PanelPause;
	public GameObject PanelBag;
	public GameObject PanelControls;
	public GameObject MovementTip;
	public GameObject BasicCameraTip;
	public GameObject SeeControlsTip;
	public FirstPersonController player;
  public AchievementManager manager;
	public bool isPaused;
	public bool isOpen;
	public bool cameraUP;

<<<<<<< HEAD
  private PlayerProfile playerData;
	private float timeAfterTip = 0;

=======
>>>>>>> parent of fb6422e... Tutorial Finalized
	// Use this for initialization
	void Start () {
        manager.loadAchievements();
		isPaused = false;
		isOpen = false;

		pauseSource = GetComponent<AudioSource> ();
		playerData = player.GetComponentInChildren<PlayerProfile> ();

		pauseSource.ignoreListenerPause = true;
		pauseSource.Play ();
		pauseSource.Pause ();
		PanelControls.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Cancel")) {
			isPaused = !isPaused;
		}
		if (Input.GetKeyDown (KeyCode.K)) {
			isOpen = !isOpen;
		}
		//  Tutorial logic ---------------------
		if (Input.GetButtonUp ("View Controls")) {
			playerData.tutFlagViewControls = true;
			PanelControls.SetActive(!PanelControls.activeSelf);
		}
		if (Input.GetButtonDown ("Horizontal") ||
			Input.GetButtonDown ("Vertical") ) {
			playerData.tutFlagMovement = true;
		}
<<<<<<< HEAD

=======
>>>>>>> parent of fb6422e... Tutorial Finalized
		if (Input.GetButtonDown ("Take Photo")) {
			playerData.tutFlagSnap = true;
		}

		//  If the player has not moved yet, show a message
		MovementTip.SetActive(!playerData.tutFlagMovement);
		BasicCameraTip.SetActive(!playerData.tutFlagSnap);
		SeeControlsTip.SetActive(!playerData.tutFlagViewControls);
		//  End tutorial logic ------------------

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
