using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : MonoBehaviour {

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}

	public void goToPark() {
		SceneManager.LoadScene ("SSV0.0");
	}

	public void goToShop() {
		SceneManager.LoadScene ("camera_shop");
	}

	public void goToBlog() {
		SceneManager.LoadScene ("blog");
	}

    public void goToCredits()
    {
        SceneManager.LoadScene("credits");
    }
}
