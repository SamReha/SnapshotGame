using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : MonoBehaviour {
	private AudioSource mainMenuSource;
	// Use this for initialization
	void Start () {
		mainMenuSource = GetComponent<AudioSource> ();

		mainMenuSource.ignoreListenerPause = true;
		mainMenuSource.Play ();
	}
	
	// Update is called once per frame
	void Update () {}

	public void goToPark() {
		mainMenuSource.Stop ();
		SceneManager.LoadScene ("SSV0.0");
	}

	public void goToShop() {
		mainMenuSource.Stop ();
		SceneManager.LoadScene ("camera_shop");
	}

	public void goToBlog() {
		mainMenuSource.Stop ();
		SceneManager.LoadScene ("blog");
	}

    public void goToBadges() {
        mainMenuSource.Stop();
        SceneManager.LoadScene("badges");
    }
}
