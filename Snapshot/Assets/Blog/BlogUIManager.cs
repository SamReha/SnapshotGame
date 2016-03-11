using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class BlogUIManager : MonoBehaviour {
	public GameObject scrollManager;
	public GameObject postedPhotosManager;
	public Text moneyText;

	// Use this for initialization
	void Start () {
		PlayerProfile.profile.load ();
	}
	
	// Update is called once per frame
	void Update () {
		moneyText.text = "$" + PlayerProfile.profile.money;
		//postedPhotosManager.GetComponent<PostedPhotosManager> ().updatePhotos();
	}

	public void loadMainMenu() {
		SceneManager.LoadScene ("main_menu");
	}

	public void postPhotos() {
		// Get list of photos to post
		List<string> newPhotos = scrollManager.GetComponent<ScrollViewManager>().imagesToPost;

		// Append to player profile and save
		foreach (string imageName in newPhotos) {
			PlayerProfile.profile.postedPhotos.Add(imageName);
			Photo photo = new Photo ();

			photo.pathname = Application.dataPath + "/Resources/" +  imageName + ".metaphoto";
			photo.load ();
			PlayerProfile.profile.money += getMoneyFromScore(photo.balanceValue, photo.interestingnessValue, photo.spacingValue);
			Debug.Log (photo.balanceValue + ", " + photo.interestingnessValue + ", " + photo.spacingValue);
		}
		PlayerProfile.profile.save ();

		scrollManager.GetComponent<ScrollViewManager> ().updatePostableImages();
		postedPhotosManager.GetComponent<PostedPhotosManager> ().updatePhotos();
		postedPhotosManager.GetComponent<PostedPhotosManager> ().getMetaData();
	}

	float getMoneyFromScore(float scoreOne, float scoreTwo, float scoreThree) {
		if (scoreOne >= scoreTwo && scoreOne >= scoreThree) {
			return scoreOne * 10f;
		} else if (scoreTwo >= scoreThree) {
			return scoreTwo * 10f;
		} else
			return scoreThree * 10f;
	}
}
