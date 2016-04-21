using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BadgeUIManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void goToMain() {
        SceneManager.LoadScene("main_menu");
    }
}
