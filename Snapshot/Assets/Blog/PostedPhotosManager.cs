using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PostedPhotosManager : MonoBehaviour {
	public GameObject newPicture;

    private string pathToPostedPhotos;

#if UNITY_EDITOR
    [MenuItem ("AssetDatabase/Snapshot")]
#endif

	// Use this for initialization
	void Start () {
        pathToPostedPhotos = Application.dataPath + "/Resources/PostedImages/";

        updatePhotos();
        getMetaData ();
	}

	void Update() {}

	public void updatePhotos() {
		foreach (Transform child in transform) {
			Destroy (child.gameObject);
		}

		#if UNITY_EDITOR
		//  Make sure pictures are loaded into resources
		AssetDatabase.Refresh();
		#endif

		DirectoryInfo dir = new DirectoryInfo(pathToPostedPhotos);
		FileInfo[] info = dir.GetFiles("*.png");
		foreach (FileInfo file in info) {
			string filename = file.Name;
			GameObject curPicture = (GameObject) Instantiate(newPicture);
			Texture2D pic = new Texture2D (2, 2);
			byte[] bytes = File.ReadAllBytes (pathToPostedPhotos + filename);
			pic.LoadImage (bytes);
			RawImage r = (RawImage) curPicture.GetComponent<RawImage> ();
			r.texture = pic;
			curPicture.GetComponent<RawImage> ().name = filename.Replace (".png", "");
			curPicture.GetComponentInChildren<Toggle> ().GetComponentInChildren<Image> ().enabled = false;
			curPicture.transform.SetParent(this.transform, false);
		}
	}

	public void getMetaData() {
		#if UNITY_EDITOR
		//  Make sure pictures are loaded into resources
		AssetDatabase.Refresh();
		#endif

		DirectoryInfo dir = new DirectoryInfo(pathToPostedPhotos);
		FileInfo[] info = dir.GetFiles("*.metaphoto");
		Photo photo = new Photo ();
		foreach (FileInfo file in info) {
			string filename = file.Name;
			photo.pathname = pathToPostedPhotos +  filename;
			photo.load ();

			Transform child = transform.Find (filename.Replace (".metaphoto", ""));
			GameObject metaData = new GameObject ();
			metaData.transform.position.Set(20f, 0f, 0f);
			Text textData = metaData.AddComponent<Text> ();

			// Configure comments for photo
			if (photo.comments.Count == 0) {
				string markup = "";
				float score = Math.Max(photo.balanceValue, Math.Max(photo.spacingValue, photo.interestingnessValue));
				//Debug.Log (score);
				if (score <= 20f) {
					markup = "bad";
				} else if (score <= 70f) {
					markup = "good";
				} else {
					markup = "perfect";
				}

				photo.comments.Add(gameObject.GetComponent<CommentGenerator>().GenerateComment (markup));
				photo.save ();
			}

			//Debug.Log (filename + " - " + photo.comments [0]);

			textData.text = photo.comments[0];
			textData.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			metaData.GetComponent<RectTransform> ().position = new Vector3 (0f, -90f, 0f);

			metaData.transform.SetParent (child, false);
		}
	}
}
