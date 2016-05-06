using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraMenuManager : MonoBehaviour {
    public Canvas cameraCanvas;
    public Text photoCounter;
    public Button memoryCardFullWarning;
    public MemoryCardReader memCardReader;
	public GameObject photoReview;

	private bool reviewingPhotos = false;
    private float warningTime = 0f;

	// Use this for initialization
	void Start () {
		PlayerProfile.profile.load ();
        updatePhotoCounter();
    }
	
	// Update is called once per frame
	void Update () {
		// Toggle active Photo Review Thingy
		photoReview.SetActive(reviewingPhotos);

		// Check to see if the user wants to review their photos
		if (Input.GetButton ("Photo Review")) {
			reviewingPhotos = !reviewingPhotos;
		}

        // Handle warning timer
        if (warningTime <= 0f) {
            memoryCardFullWarning.gameObject.SetActive(false);
        } else {
            warningTime -= Time.deltaTime;
        }
	}

    public void warnAboutFullCard() {
        memoryCardFullWarning.gameObject.SetActive(true);
        warningTime = 2f;
    }

    public void updatePhotoCounter() {
		photoCounter.text = memCardReader.getPhotoCount() + " / " + PlayerProfile.profile.memoryCardCapacity;
    }
}
