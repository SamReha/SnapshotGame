using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraMenuManager : MonoBehaviour {
	public FirstPersonController player;
    public Canvas cameraCanvas;
    public GameObject cameraMenuViewPortContents;
    public Text photoCounter;
    public Button memoryCardFullWarning;
    public MemoryCardReader memCardReader;
	public GameObject photoReview;
    public GameObject newPicture;
	public Button deleteBttn;

    private bool reviewingPhotos = false;
    private float warningTime = 0f;

	private GameObject parent;
	private Vector3 cameraHeldUp;
	private Vector3 cameraHeldDown;

	// Use this for initialization
	void Start () {
		PlayerProfile.profile.load ();
        updatePhotoCounter();
        updatePhotoReviewUI();

		deleteBttn.interactable = false;

		parent = GameObject.Find("PlayerCam");
		cameraHeldUp   = new Vector3( 0.009f, 0.030f,-0.100f);
		cameraHeldDown = new Vector3( 0.293f,-0.499f, 0.300f);
    }
	
	// Update is called once per frame
	void Update () {
		// Toggle active Photo Review Thingy
		photoReview.SetActive(reviewingPhotos);

		// Check to see if the user wants to review their photos
		if (Input.GetButtonUp ("Photo Review")) {
			reviewingPhotos = !reviewingPhotos;
			if (reviewingPhotos) {
				player.m_MouseLook.SetCursorLock (!reviewingPhotos);
				player.m_MouseLook.XSensitivity = 0f;
				player.m_MouseLook.YSensitivity = 0f;
				parent.transform.localPosition = cameraHeldUp;
			} else {
				player.m_MouseLook.SetCursorLock (reviewingPhotos);
				player.m_MouseLook.XSensitivity = 2f;
				player.m_MouseLook.YSensitivity = 2f;
				parent.transform.localPosition = cameraHeldDown;
			}
		}

        // Handle warning timer
		if (warningTime <= 0f) {
			memoryCardFullWarning.gameObject.SetActive (false);
		} else {
			warningTime -= Time.deltaTime;
		}

		foreach (Transform child in cameraMenuViewPortContents.transform) {
			if (child.gameObject.GetComponentInChildren<Toggle> ().isOn) {
				deleteBttn.interactable = true;
				break;
			} else {
				deleteBttn.interactable = false;
			}
		}

	}

	public void deletePhotos(){
		foreach (Transform child in cameraMenuViewPortContents.transform) {
			if (child.gameObject.GetComponentInChildren<Toggle> ().isOn) {
				string name = child.name;
				Destroy (child.gameObject);
				File.Delete (Application.dataPath + "/Resources/UploadQueue/" + name + ".png");
				File.Delete (Application.dataPath + "/Resources/UploadQueue/" + name + ".metaphoto");
			}
		}
		//updatePhotoReviewUI ();
		updatePhotoCounter ();
	}

    public void updatePhotoReviewUI() {
        foreach (Transform child in cameraMenuViewPortContents.transform) {
			Destroy (child.gameObject);
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
