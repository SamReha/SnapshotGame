using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class BlogUIManager : MonoBehaviour {
	private AudioSource blogSource;
	public GameObject scrollManager;
	public GameObject postedPhotosManager;
	public Text moneyText;
    public AchievementManager achievementManager;

	private string pathToPostedPhotos;
	private string pathToUploadQueue;

	// Use this for initialization
	void Start () {
		PlayerProfile.profile.load ();
		blogSource = GetComponent<AudioSource> ();

		blogSource.ignoreListenerPause = true;
		blogSource.Play ();

		pathToPostedPhotos = Application.dataPath + "/Resources/PostedImages/";
		pathToUploadQueue = Application.dataPath + "/Resources/UploadQueue/";
	}
	
	// Update is called once per frame
	void Update () {
		moneyText.text = "$" + PlayerProfile.profile.money;
		//postedPhotosManager.GetComponent<PostedPhotosManager> ().updatePhotos();
	}

	public void loadMainMenu() {
        achievementManager.saveAchievements();
        blogSource.Stop ();
		SceneManager.LoadScene ("main_menu");

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Play ();
		}
	}

	public void postPhotos() {
		// Get list of photos to post
		List<string> newPhotos = scrollManager.GetComponent<ScrollViewManager>().imagesToPost;

		// Append to player profile and save
		foreach (string imageName in newPhotos) {
			PlayerProfile.profile.postedPhotos.Add(imageName);
			Photo photo = new Photo ();

			photo.pathname = pathToUploadQueue + imageName + ".metaphoto";
			photo.load ();
			PlayerProfile.profile.money += getMoneyFromScore(photo);
			//Debug.Log (photo.balanceValue + ", " + photo.interestingnessValue + ", " + photo.spacingValue);

            // Update Achievements (score based)
            if (photo.balanceValue >= 95.0f) {
                achievementManager.SetProgressToAchievement("Balanced Breakfast", 1.0f);
            }
            if (photo.interestingnessValue >= 95.0f) {
                achievementManager.SetProgressToAchievement("The Most Interesting Photo in The World", 1.0f);
            }
            if (photo.spacingValue >= 95.0f) {
                achievementManager.SetProgressToAchievement("The Final Frontier", 1.0f);
            }
            if (photo.balanceValue >= 95.0f && photo.interestingnessValue >= 95.0f && photo.spacingValue >= 95.0f) {
                achievementManager.SetProgressToAchievement("Perfection Incarnate", 1.0f);
            }

            // Update Achievements (subject or accessory based)
            if (photo.containsOwl && bestScore(photo) >= 50.0f) {
                achievementManager.SetProgressToAchievement("Owl Have What She's Having", 1.0f);
            }

            if (photo.containsFox && bestScore(photo) >= 50.0f) {
                achievementManager.SetProgressToAchievement("What the Fox", 1.0f);
            }

            if (photo.containsDeer && bestScore(photo) >= 50.0f) {
                achievementManager.SetProgressToAchievement("Doe, a Deer, a Female Deer", 1.0f);
            }

            if (photo.takenWithTelephoto && bestScore(photo) >= 50.0f) {
                achievementManager.SetProgressToAchievement("Telemachus", 1.0f);
            }

            if (photo.takenWithWide && bestScore(photo) >= 50.0f) {
                achievementManager.SetProgressToAchievement("Wide Awake", 1.0f);
            }

			Debug.Log (imageName);
			// Finally, move the photo from .../Resources/UploadQueue/ to .../Resources/PostedImages/
			File.Move(pathToUploadQueue + imageName + ".metaphoto", pathToPostedPhotos + imageName + ".metaphoto");
			File.Move(pathToUploadQueue + imageName + ".png", pathToPostedPhotos + imageName + ".png");
			File.Move(pathToUploadQueue + imageName + ".metaphoto.meta", pathToPostedPhotos + imageName + ".metaphoto.meta");
            File.Move(pathToUploadQueue + imageName + ".png.meta", pathToPostedPhotos + imageName + ".png.meta");
        }
		PlayerProfile.profile.save ();

        // Update achievements
        achievementManager.AddProgressToAchievement("Journeyman Photographer", newPhotos.Count);
        achievementManager.AddProgressToAchievement("Experienced Photographer", newPhotos.Count);
        achievementManager.AddProgressToAchievement("Expert Photographer", newPhotos.Count);

        scrollManager.GetComponent<ScrollViewManager> ().updatePostableImages();
		postedPhotosManager.GetComponent<PostedPhotosManager> ().updatePhotos();
		postedPhotosManager.GetComponent<PostedPhotosManager> ().getMetaData();
    }

	float getMoneyFromScore(Photo photo) {
        Debug.Log("This photo has a score of: Balance: " + photo.balanceValue
                  + " Spacing: " + photo.spacingValue + " Interest: " + photo.interestingnessValue);

        return Mathf.Max(0, bestScore(photo) * 10.0f);
	}

    float bestScore(Photo photo) {
        if (photo.balanceValue >= photo.interestingnessValue && photo.balanceValue >= photo.spacingValue) {
            return photo.balanceValue;
        } else if (photo.interestingnessValue >= photo.spacingValue) {
            return photo.interestingnessValue;
        } else
            return photo.spacingValue;
    }
}
