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
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/");
		FileInfo[] info = dir.GetFiles("*.png");
		foreach (FileInfo file in info)
		{
			
			string filename = file.Name;


			if (PlayerProfile.profile.postedPhotos.Contains(filename.Replace(".png", ""))) {
				GameObject curPicture = (GameObject) Instantiate(newPicture);
				Texture2D pic = new Texture2D (2, 2);
				byte[] bytes = File.ReadAllBytes (Application.dataPath + "/Resources/" + filename);
				pic.LoadImage (bytes);
				RawImage r = (RawImage) curPicture.GetComponent<RawImage> ();
				r.texture = pic;
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
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/");
		FileInfo[] info = dir.GetFiles("*.png");
		foreach (FileInfo file in info)
		{
			string filename = file.Name;

			if (PlayerProfile.profile.postedPhotos.Contains(filename.Replace(".png", ""))) {
				GameObject curPicture = (GameObject) Instantiate(newPicture);
				Texture2D pic = new Texture2D (2, 2);
				byte[] bytes = File.ReadAllBytes (Application.dataPath + "/Resources/" + filename);
				pic.LoadImage (bytes);
				RawImage r = (RawImage) curPicture.GetComponent<RawImage> ();
				r.texture = pic;
				curPicture.GetComponent<RawImage> ().name = filename.Replace (".png", "");
				curPicture.GetComponentInChildren<Toggle> ().GetComponentInChildren<Image> ().enabled = false;
				curPicture.transform.SetParent(this.transform, false);
			}
		}
	}

	public void getMetaData() {
		#if UNITY_EDITOR
		//  Make sure pictures are loaded into resources
		AssetDatabase.Refresh();
		#endif
		DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/");
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
				string markup = "";
				float score = photo.balanceValue + photo.spacingValue + photo.interestingnessValue;
				Debug.Log (score);
				if (score <= 10f) {
					markup = "bad";
				} else if (score <= 20f) {
					markup = "good";
				} else {
					markup = "perfect";
				}
				textData.text = gameObject.GetComponent<CommentGenerator>().GenerateComment (markup);
				//Debug.Log (textData.text);
				textData.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
				metaData.GetComponent<RectTransform> ().position = new Vector3 (0f, -90f, 0f);
				//Debug.Log ("CHILD: " + child);

				metaData.transform.SetParent (child, false);
			}
		}
	}
}
