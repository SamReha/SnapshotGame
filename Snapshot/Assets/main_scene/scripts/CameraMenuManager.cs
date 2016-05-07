using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEditor;

public class CameraMenuManager : MonoBehaviour {
    public Canvas cameraCanvas;
    public GameObject cameraMenuViewPortContents;
    public Text photoCounter;
    public Button memoryCardFullWarning;
    public MemoryCardReader memCardReader;
	public GameObject photoReview;
    public GameObject newPicture;

    private bool reviewingPhotos = false;
    private float warningTime = 0f;

	// Use this for initialization
	void Start () {
		PlayerProfile.profile.load ();
        updatePhotoCounter();
        updatePhotoReviewUI();
    }
	
	// Update is called once per frame
	void Update () {
		// Toggle active Photo Review Thingy
		photoReview.SetActive(reviewingPhotos);

		// Check to see if the user wants to review their photos
		if (Input.GetButtonUp ("Photo Review")) {
			reviewingPhotos = !reviewingPhotos;
		}

        // Handle warning timer
        if (warningTime <= 0f) {
            memoryCardFullWarning.gameObject.SetActive(false);
        } else {
            warningTime -= Time.deltaTime;
        }
	}

    public void updatePhotoReviewUI() {
        foreach (Transform child in cameraMenuViewPortContents.transform) {
            Destroy(child.gameObject);
        }

        #if UNITY_EDITOR
        //  Make sure pictures are loaded into resources
        AssetDatabase.Refresh();
        #endif

        DirectoryInfo dir = new DirectoryInfo(memCardReader.pathToUploadQueue);
        FileInfo[] info = dir.GetFiles("*.png");
        foreach (FileInfo photoFile in info) {
            string filename = photoFile.Name;

            GameObject curPicture = Instantiate(newPicture);
            Texture2D pic = new Texture2D(2, 2);
            byte[] bytes = File.ReadAllBytes(memCardReader.pathToUploadQueue + filename);
            pic.LoadImage(bytes);
            RawImage rawImage = curPicture.GetComponent<RawImage>();
            rawImage.texture = pic;
            curPicture.GetComponent<RawImage>().name = filename.Replace(".png", "");
            curPicture.transform.SetParent(cameraMenuViewPortContents.transform, false);
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
