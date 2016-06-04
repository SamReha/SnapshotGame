using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;

public class MainMenuUI : MonoBehaviour {
	public static AudioSource mainMenuSource;
	public static bool prepMenu = false;
	public GameObject confirmDeleteSave;
	public GameObject deleteSaveSuccessful;

	// Use this for initialization
	void Start () {
		confirmDeleteSave.SetActive (false);
		deleteSaveSuccessful.SetActive (false);

		if (!prepMenu) {
			mainMenuSource = GetComponent<AudioSource> ();
			AudioSource.DontDestroyOnLoad (mainMenuSource);
			mainMenuSource.ignoreListenerPause = true;
			mainMenuSource.Play ();
		}

		if (prepMenu) {
			mainMenuSource.volume = 0f;
		}

		if (ParkPrepUIManager.src) {
			mainMenuSource.volume = 0f;
		}
	}
	
	// Update is called once per frame
	void Update () {}

	public void goToPark() {
		prepMenu = true;
		SceneManager.LoadScene ("park_prep");
	}

	public void goToShop() {
		prepMenu = false;
		mainMenuSource.Stop ();

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Stop ();
		}
		SceneManager.LoadScene ("camera_shop");
	}

	public void goToBlog() {
		prepMenu = false;
		mainMenuSource.Stop ();

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Stop();
		}
		SceneManager.LoadScene ("blog");
	}

    public void goToBadges() {
		prepMenu = false;
        mainMenuSource.Stop();

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Stop ();
		}
        SceneManager.LoadScene("badges");
    }

    public void goToCredits() {
		prepMenu = false;
        mainMenuSource.Stop();

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Stop();
		}
        SceneManager.LoadScene("credits");
    }

	public void deleteSave() {
		// First, wipe the data in the persistent datapath
		DirectoryInfo persistentDataDirectory = new DirectoryInfo(Application.persistentDataPath);

		foreach (FileInfo file in persistentDataDirectory.GetFiles()) {
			file.Delete(); 
		}
		foreach (DirectoryInfo dir in persistentDataDirectory.GetDirectories()) {
			dir.Delete(true); 
		}

		// Then, wipe the blog and memcard data from the resources directory.
		DirectoryInfo postedImages = new DirectoryInfo(Application.dataPath + "/Resources/PostedImages/");
		DirectoryInfo UploadQueue = new DirectoryInfo(Application.dataPath + "/Resources/UploadQueue/");

		foreach (FileInfo file in postedImages.GetFiles()) {
			file.Delete(); 
		}
		foreach (FileInfo file in UploadQueue.GetFiles()) {
			file.Delete(); 
		}

		confirmDeleteSave.SetActive (false);
		deleteSaveSuccessful.SetActive (true);
	}

	public void promptToDeleteSave() {
		confirmDeleteSave.SetActive (true);
	}

	public void cancelDeleteSave() {
		confirmDeleteSave.SetActive (false);
	}

	public void endSaveDialogue() {
		deleteSaveSuccessful.SetActive (false);
	}
}
