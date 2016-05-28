using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class MainMenuUI : MonoBehaviour {
	public static AudioSource mainMenuSource;
	public static bool prepMenu = false;
	// Use this for initialization
	void Start () {
		if (!prepMenu) {
			mainMenuSource = GetComponent<AudioSource> ();
			AudioSource.DontDestroyOnLoad (mainMenuSource);
			mainMenuSource.ignoreListenerPause = true;
			mainMenuSource.Play ();
		}

		if (prepMenu) {
			mainMenuSource.volume = 0f;
		}

		if (ParkPrepUIManager.src) {
			mainMenuSource.volume = 0f;
		}
	}
	
	// Update is called once per frame
	void Update () {Debug.Log(mainMenuSource.isPlaying);}

	public void goToPark() {
		prepMenu = true;
		SceneManager.LoadScene ("park_prep");
	}

	public void goToShop() {
		prepMenu = false;
		mainMenuSource.Stop ();

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Stop ();
		}
		SceneManager.LoadScene ("camera_shop");
	}

	public void goToBlog() {
		prepMenu = false;
		mainMenuSource.Stop ();

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Stop();
		}
		SceneManager.LoadScene ("blog");
	}

    public void goToBadges() {
		prepMenu = false;
        mainMenuSource.Stop();

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Stop ();
		}
        SceneManager.LoadScene("badges");
    }

    public void goToCredits() {
		prepMenu = false;
        mainMenuSource.Stop();

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Stop();
		}
        SceneManager.LoadScene("credits");
    }
}
