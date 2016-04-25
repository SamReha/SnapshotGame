using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour {
    public Button goBack;
    public AudioSource muzak;
    public GameObject rump;

    private GameObject credits;
	private float scrollSpeed = 20.0f;
    private Vector3 fallBackScrollSpeed = new Vector3(0.0f, 0.4f, 0.0f);
    private bool showButton = false;
    private bool scrolling = true;

	// Use this for initialization
	void Start () {
		credits = GameObject.Find ("HolderOfTheText");
        goBack.gameObject.SetActive(false);

        muzak.ignoreListenerPause = true;
        muzak.PlayDelayed(0.5f);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Cancel")) {
            showButton = true;
        }
        goBack.gameObject.SetActive(showButton);

        if (scrolling) {
            // For some reason, this does not work in the compiled version of the game (go figure!)
            // We'll use a fallback, but it sacrifices fine control over scroll speed.
            //credits.transform.Translate(Vector3.up * Time.deltaTime * scrollSpeed);
            credits.transform.Translate(fallBackScrollSpeed);
        }

        if (rump.transform.position.y >= 0) {
            scrolling = false;
            showButton = true;
        }
    }

    public void goBackOnClick() {
        muzak.Stop();
        SceneManager.LoadScene("main_menu");
    }
}
