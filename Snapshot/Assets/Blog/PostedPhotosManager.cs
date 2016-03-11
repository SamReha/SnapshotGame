using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PostedPhotosManager : MonoBehaviour {
	public GameObject newPicture;

	#if UNITY_EDITOR
	[MenuItem ("AssetDatabase/Snapshot")]
	#endif

	// Use this for initialization
	void Start () {
		#if UNITY_EDITOR
		//  Make sure pictures are loaded into resources
		AssetDatabase.Refresh();
		#endif
		DirectoryInfo dir = new DirectoryInfo(Path.Combine("Assets", "Resources"));
		FileInfo[] info = dir.GetFiles("*.png");
		foreach (FileInfo file in info)
		{
			string filename = file.Name;

			if (PlayerProfile.profile.postedPhotos.Contains(filename.Replace(".png", ""))) {
				GameObject curPicture = (GameObject) Instantiate(newPicture);
				curPicture.GetComponent<RawImage>().texture = Resources.Load(filename.Replace(".png", "")) as Texture;
				curPicture.GetComponent<RawImage> ().name = filename.Replace (".png", "");
				curPicture.transform.SetParent(this.transform, false);
			}
		}

		getMetaData ();
	}

	void Update() {
		//updatePhotos ();
		//getMetaData ();
	}

	public void updatePhotos() {
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}

		#if UNITY_EDITOR
		//  Make sure pictures are loaded into resources
		AssetDatabase.Refresh();
		#endif
		DirectoryInfo dir = new DirectoryInfo(Path.Combine("Assets", "Resources"));
		FileInfo[] info = dir.GetFiles("*.png");
		foreach (FileInfo file in info)
		{
			string filename = file.Name;

			if (PlayerProfile.profile.postedPhotos.Contains(filename.Replace(".png", ""))) {
				GameObject curPicture = (GameObject) Instantiate(newPicture);
				curPicture.GetComponent<RawImage>().texture = Resources.Load(filename.Replace(".png", "")) as Texture;
				curPicture.GetComponent<RawImage> ().name = filename.Replace (".png", "");
				curPicture.transform.SetParent(this.transform, false);
			}
		}
	}

	public void getMetaData() {
		#if UNITY_EDITOR
		//  Make sure pictures are loaded into resources
		AssetDatabase.Refresh();
		#endif
		DirectoryInfo dir = new DirectoryInfo(Path.Combine("Assets", "Resources"));
		FileInfo[] info = dir.GetFiles("*.metaphoto");
		Photo photo = new Photo ();
		foreach (FileInfo file in info)
		{
			string filename = file.Name;

			if (PlayerProfile.profile.postedPhotos.Contains(filename.Replace(".metaphoto", ""))) {
				photo.pathname = Application.dataPath + "/Resources/" +  filename;
				photo.load ();

				//Debug.Log ("Meta data found");
				Transform child = transform.Find (filename.Replace (".metaphoto", ""));
				GameObject metaData = new GameObject ();
				metaData.transform.position.Set(20f, 0f, 0f);
				Text textData = metaData.AddComponent<Text> ();
				textData.text = "Balance: " + photo.balanceValue + ", " + "Spacing: " + photo.spacingValue + ", " + "Interesting: " + photo.interestingnessValue;

				Debug.Log ("CHILD: " + child);

				metaData.transform.SetParent (child, false);
			}
		}
	}
}
