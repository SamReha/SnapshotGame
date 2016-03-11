using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BlogUIManager : MonoBehaviour {
	public GameObject scrollManager;

	// Use this for initialization
	void Start () {
		PlayerProfile.profile.load ();
	}
	
	// Update is called once per frame
	void Update () {}

	public void loadMainMenu() {
		SceneManager.LoadScene ("main_menu");
	}

	public void postPhotos() {
		// Get list of photos to post
		List<string> newPhotos = scrollManager.GetComponent<ScrollViewManager>().imagesToPost;
		Debug.Log (newPhotos.Count);
		// Append to player profile and save
		foreach (string imageName in newPhotos) {
			PlayerProfile.profile.postedPhotos.Add(imageName);
		}
		PlayerProfile.profile.save ();
	}
}
