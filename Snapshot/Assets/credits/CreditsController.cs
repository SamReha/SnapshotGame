using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsController : MonoBehaviour {
    public Button goBack;
    public AudioSource muzak;

    private GameObject credits;
	private Vector3 scrollSpeed = new Vector3 (0.0f, 0.4f, 0.0f);
    private bool showButton = false;

	// Use this for initialization
	void Start () {
		credits = GameObject.Find ("HolderOfTheText");
        goBack.gameObject.SetActive(false);

        muzak.ignoreListenerPause = true;
        muzak.PlayDelayed(0.5f);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Cancel")) {
            showButton = true;
        }

        if (credits.transform.position.y < 2325) {
            credits.transform.Translate(scrollSpeed);
        } else showButton = true;

        goBack.gameObject.SetActive(showButton);
    }

    public void goBackOnClick() {
        muzak.Stop();
        SceneManager.LoadScene("main_menu");
    }
}
