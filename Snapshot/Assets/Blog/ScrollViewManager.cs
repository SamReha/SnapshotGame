using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScrollViewManager : MonoBehaviour {
	public GameObject newPicture;
	public List<string> imagesToPost;
    public Toggle toggle;
    public List<GameObject> curNames = new List<GameObject>();

	private string pathToUploadQueue;

#if UNITY_EDITOR
    [MenuItem ("AssetDatabase/Snapshot")]
#endif

    // Use this for initialization
    void Start () {
		pathToUploadQueue = Application.dataPath + "/Resources/UploadQueue/";

		updatePostableImages ();

		imagesToPost = new List<string> ();
	}

	void Update() {
		List<string> imageBuffer = new List<string>();
		var images = GetComponentsInChildren<RawImage> ();

		foreach (RawImage image in images) {
			Toggle tog = image.GetComponentInChildren<Toggle> ();

			if (tog.isOn) {
				imageBuffer.Add (image.name);
			}
		}

		imagesToPost = imageBuffer;
	}

	public void selectAll() {
		var images = GetComponentsInChildren<RawImage> ();

		foreach(RawImage img in images) {
			Toggle t = img.GetComponentInChildren<Toggle> ();
			t.isOn = true;
		}
	}
	public void updatePostableImages() {
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}


		#if UNITY_EDITOR
		//  Make sure pictures are loaded into resources
		AssetDatabase.Refresh();
		#endif

		DirectoryInfo dir = new DirectoryInfo(pathToUploadQueue);
		FileInfo[] info = dir.GetFiles("*.png");
		foreach (FileInfo photoFile in info) {
			string filename = photoFile.Name;

			GameObject curPicture = (GameObject)Instantiate (newPicture);
			Texture2D pic = new Texture2D (2, 2);
			byte[] bytes = File.ReadAllBytes (pathToUploadQueue + filename);
			pic.LoadImage (bytes);
			RawImage rawImage = (RawImage) curPicture.GetComponent<RawImage> ();
			rawImage.texture = pic;
			curPicture.GetComponent<RawImage> ().name = filename.Replace (".png", "");
			curPicture.transform.SetParent (this.transform, false);
			curNames.Add (curPicture);
		}
	}

    public void eventValueChanged ()
    {
        if (toggle.isOn)
        {
            Debug.Log("It's on.");
        }
    }
}
