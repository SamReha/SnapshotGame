using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraMenuManager : MonoBehaviour {
    public Canvas cameraCanvas;
    public Text photoCounter;
    public Button memoryCardFullWarning;
    public MemoryCardReader memCardReader;

    private float warningTime = 0f;

	// Use this for initialization
	void Start () {
        photoCounter.text = updatePhotoCounter();
    }
	
	// Update is called once per frame
	void Update () {
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

    public string updatePhotoCounter() {
        return memCardReader.getPhotoCount() + " / " + PlayerProfile.profile.memoryCardCapacity;
    }
}
