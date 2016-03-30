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

#if UNITY_EDITOR
    [MenuItem ("AssetDatabase/Snapshot")]
#endif

    // Use this for initialization
    void Start () {
#if UNITY_EDITOR
		//  Make sure pictures are loaded into resources
        AssetDatabase.Refresh();
#endif
        int counter = 0;
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/");
        //Debug.Log(dir);
        FileInfo[] info = dir.GetFiles("*.png");
        foreach (FileInfo f in info)
        {
            //Debug.Log(f);
            string filename = f.Name;
            //Debug.Log(filename);

			if (!PlayerProfile.profile.postedPhotos.Contains (filename.Replace (".png", ""))) {
				GameObject curPicture = (GameObject)Instantiate (newPicture);
				Texture2D pic = new Texture2D (2, 2);
				byte[] bytes = File.ReadAllBytes (Application.dataPath + "/Resources/" + filename);
				pic.LoadImage (bytes);
				RawImage r = (RawImage) curPicture.GetComponent<RawImage> ();
				r.texture = pic;
				curPicture.GetComponent<RawImage> ().name = filename.Replace (".png", "");
				curPicture.transform.SetParent (this.transform, false);
				curNames.Add (curPicture);
			}
            counter++;
        }

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

	public void updatePostableImages() {
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}

		#if UNITY_EDITOR
		//  Make sure pictures are loaded into resources
		AssetDatabase.Refresh();
		#endif
		int counter = 0;
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/");
		//Debug.Log(dir);
		FileInfo[] info = dir.GetFiles("*.png");
		foreach (FileInfo f in info)
		{
			//Debug.Log(f);
			string filename = f.Name;
			//Debug.Log(filename);

			if (!PlayerProfile.profile.postedPhotos.Contains (filename.Replace (".png", ""))) {
				GameObject curPicture = (GameObject)Instantiate (newPicture);
				Texture2D pic = new Texture2D (2, 2);
				byte[] bytes = File.ReadAllBytes (Application.dataPath + "/Resources/" + filename);
				pic.LoadImage (bytes);
				RawImage r = (RawImage) curPicture.GetComponent<RawImage> ();
				r.texture = pic;
				curPicture.GetComponent<RawImage> ().name = filename.Replace (".png", "");
				curPicture.transform.SetParent (this.transform, false);
				curNames.Add (curPicture);
			}
			counter++;
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
