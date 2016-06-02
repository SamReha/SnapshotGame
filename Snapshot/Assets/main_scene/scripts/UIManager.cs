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
	public float movementTipTime = 3;
	public float basicCameraTipTime = 7;
	public float seeControlsTipTime = 15;
	public FirstPersonController player;
    public AchievementManager manager;
	public bool isPaused;
	public bool isOpen;
	public bool cameraUP;

    PlayerProfile playerData;
	private float timeAfterTip = 0;

	private float tutTimer;

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

		MovementTip.SetActive(false);
		BasicCameraTip.SetActive(false);
		SeeControlsTip.SetActive(false);

		tutTimer = playerData.timeElapsedInPark;
	}

	// Update is called once per frame
	void Update () {
		tutTimer += Time.deltaTime;
		playerData.timeElapsedInPark = tutTimer;

		if (Input.GetButtonDown("Cancel")) {
			isPaused = !isPaused;
		}
		if (Input.GetKeyDown (KeyCode.K)) {
			isOpen = !isOpen;
		}

		//  Tutorial logic ---------------------
		if (tutTimer > movementTipTime &&
			!playerData.tutFlagRun){
			Debug.Log ("Movement flag active");
			MovementTip.SetActive(true);
		}
		if (playerData.tutFlagRun && tutTimer - timeAfterTip > 5f && !playerData.tutFlagAim) {
			Debug.Log ("BasicCam flag active");
			BasicCameraTip.SetActive(true);
		}
		if (playerData.tutFlagAim && tutTimer - timeAfterTip > 5f && !playerData.tutFlagViewControls) {
			Debug.Log ("ADVANCED flag active");
			SeeControlsTip.SetActive(true);
		}

		//Debug.Log ("Movement flag: " + playerData.tutFlagMovement);
		//Debug.Log ("Camera flag: " + playerData.tutFlagSnap);
		//Debug.Log ("Advanced flag: " + playerData.tutFlagViewControls);
		//Debug.Log ("elapsed time: " + tutTimer ); Don't you dare leave a log like this in! It floods the console. >:(

		if (Input.GetButtonDown ("Horizontal") ||
			Input.GetButtonDown ("Vertical") && tutTimer > movementTipTime) {
			playerData.tutFlagRun = true;
			timeAfterTip = tutTimer;
			MovementTip.SetActive(false);
			playerData.save ();
		}

		if (Input.GetButtonDown ("Camera Switch")) {
			playerData.tutFlagAim = true;
			timeAfterTip = tutTimer;
			BasicCameraTip.SetActive(false);
			playerData.save ();

		}
		if (Input.GetButtonUp ("View Controls")) {
			playerData.tutFlagViewControls = true;
			//  Shows controls the first time the button is pressed (fix)
			PanelControls.SetActive(!PanelControls.activeSelf);
			SeeControlsTip.SetActive(false);
			playerData.save ();
		}

		//  If the player has not moved yet, show a message
		//MovementTip.SetActive(!playerData.tutFlagMovement);
		//BasicCameraTip.SetActive(!playerData.tutFlagSnap);
		//SeeControlsTip.SetActive(!playerData.tutFlagViewControls);
		//  End tutorial logic ------------------

		OpenBag (isOpen);
		setPause(isPaused);
	}

	public void OpenBag(bool bagState){
		player.m_MouseLook.SetCursorLock (!bagState);

		PanelBag.SetActive(bagState);
		if (bagState) {
			Time.timeScale = 0.0f;
		} else {
			Time.timeScale = 1.0f;
		}
	}

	public void setPause(bool pauseState) {
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
		ParkPrepUIManager.src.Play ();
		AudioManager.getInstance().setExitToMenu (true);
	}
}
