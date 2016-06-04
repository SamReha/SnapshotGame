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
	public GameObject mainCamera;
    public AchievementManager manager;
	public bool isPaused;
	public bool isOpen;
	public bool cameraUP;

	private float timeAfterTip = 0;

	private float tutTimer;

	// Use this for initialization
	void Start () {
        manager.loadAchievements();
		isPaused = false;
		isOpen = false;

		PlayerProfile.profile.load ();

		pauseSource = GetComponent<AudioSource> ();
		pauseSource.ignoreListenerPause = true;
		pauseSource.Play ();
		pauseSource.Pause ();
		PanelControls.SetActive(false);

		MovementTip.SetActive(false);
		BasicCameraTip.SetActive(false);
		SeeControlsTip.SetActive(false);

		tutTimer = PlayerProfile.profile.timeElapsedInPark;

	}

	// Update is called once per frame
	void Update () {
		tutTimer += Time.deltaTime;
		PlayerProfile.profile.timeElapsedInPark = tutTimer;

		if (Input.GetButtonDown("Cancel")) {
			isPaused = !isPaused;
			setPause(isPaused);
			Cursor.visible = isPaused;

		}

		//  Tutorial logic ---------------------
		if (tutTimer > movementTipTime &&
		    !PlayerProfile.profile.tutFlagRun) {
			Debug.Log ("Movement flag active");
			MovementTip.SetActive (true);
		} else if (PlayerProfile.profile.tutFlagRun && tutTimer - timeAfterTip > 5f && !PlayerProfile.profile.tutFlagAim) {
			Debug.Log ("BasicCam flag active");
			BasicCameraTip.SetActive (true);
		} else if (PlayerProfile.profile.tutFlagAim && tutTimer - timeAfterTip > 5f && !PlayerProfile.profile.tutFlagViewControls) {
			Debug.Log ("ADVANCED flag active");
			SeeControlsTip.SetActive (true);
		}

		//Debug.Log ("Movement flag: " + PlayerProfile.profile.tutFlagMovement);
		//Debug.Log ("Camera flag: " + PlayerProfile.profile.tutFlagSnap);
		//Debug.Log ("Advanced flag: " + PlayerProfile.profile.tutFlagViewControls);
		//Debug.Log ("elapsed time: " + tutTimer ); Don't you dare leave a log like this in! It floods the console. >:(

		if (Input.GetButtonDown ("Horizontal") ||
			Input.GetButtonDown ("Vertical") && tutTimer > movementTipTime) {
			PlayerProfile.profile.tutFlagRun = true;
			timeAfterTip = tutTimer;
			MovementTip.SetActive(false);
			PlayerProfile.profile.save ();
		}

		if (Input.GetButtonDown ("Camera Switch")) {
			PlayerProfile.profile.tutFlagAim = true;
			timeAfterTip = tutTimer;
			BasicCameraTip.SetActive(false);
			PlayerProfile.profile.save ();

		}
		if (Input.GetButtonUp ("View Controls")) {
			PlayerProfile.profile.tutFlagViewControls = true;
			//  Shows controls the first time the button is pressed (fix)
			PanelControls.SetActive(!PanelControls.activeSelf);
			SeeControlsTip.SetActive(false);
			PlayerProfile.profile.save ();
		}

		//  If the player has not moved yet, show a message
		//MovementTip.SetActive(!PlayerProfile.profile.tutFlagMovement);
		//BasicCameraTip.SetActive(!PlayerProfile.profile.tutFlagSnap);
		//SeeControlsTip.SetActive(!PlayerProfile.profile.tutFlagViewControls);
		//  End tutorial logic ------------------
	}

	public void setPause(bool pauseState) {


		PanelPause.SetActive(pauseState);
		if (pauseState) {
			player.m_MouseLook.SetCursorLock (!pauseState);
			player.m_MouseLook.XSensitivity = 0f;
			player.m_MouseLook.YSensitivity = 0f;
			Time.timeScale = 0.0f;
			AudioListener.pause = true;

			pauseSource.UnPause ();
		} else {
			Time.timeScale = 1.0f;
			player.m_MouseLook.XSensitivity = 2f;
			player.m_MouseLook.YSensitivity = 2f;
			AudioListener.pause = false;

			pauseSource.Pause ();
		}
	}

	// A handy method for when passing args is difficult
	public void unPause() {
		isPaused = false;
		setPause(isPaused);
		Cursor.visible = false;
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
