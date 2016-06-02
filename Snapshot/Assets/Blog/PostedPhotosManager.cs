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
	public int currentPosition = 0;

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
			curPicture.GetComponentInChildren<Toggle> ().gameObject.SetActive (false);
			curPicture.AddComponent<Button> ();
			curPicture.GetComponentInChildren<Button> ().enabled = true;
			curPicture.GetComponentInChildren<Button> ().tag = "imagebtn";
			curPicture.GetComponentInChildren<Button> ().onClick.AddListener (() => {
				GameObject[] objs = GameObject.FindGameObjectsWithTag("imagebtn");
				List<Button> btns = new List<Button>();

				foreach(GameObject go in objs) {
					btns.Add(go.GetComponent<Button>());
				}

				foreach(Button b in btns) {
					if(!b.Equals(curPicture.GetComponentInChildren<Button>())) {
						b.enabled = false;
					}
				}
				viewComments ();
			});

			Texture2D pic = new Texture2D (2, 2);
			byte[] bytes = File.ReadAllBytes (pathToPostedPhotos + filename);
			pic.LoadImage (bytes);
			RawImage r = (RawImage) curPicture.GetComponent<RawImage> ();
			r.texture = pic;
			curPicture.GetComponent<RawImage> ().name = filename.Replace (".png", "");
			curPicture.transform.SetParent(this.transform, false);
		}
	}
	//a now deprecated method to get a single comment for a photo
	public string getPhotoComment(Photo p) { 
		//Photo p = new Photo ();
		p.load ();

		if (p.comments.Count == 0) {
			return "";
		} else return p.comments[0];
	}

	public void viewComments() {
		RawImage riSelectedPhoto;
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("imagebtn"); //get all of the photo buttons
		GameObject parent = GameObject.FindGameObjectWithTag("imagebtn"); //it's a temporary value

		foreach (GameObject go in objs) {
			if (go.GetComponent<Button> ().enabled) { //find the enabled photo button
				parent = go; //make it the parent
			}
		}
		riSelectedPhoto = BlogUIManager.photoPanel.GetComponentInChildren<RawImage> (); //grapb the photo panel raw image
		Texture2D pic = new Texture2D (2, 2); //set up a texture
		byte[] bytes = File.ReadAllBytes(pathToPostedPhotos + parent.GetComponent<RawImage>().name + ".png"); //read the photo's bytes
		pic.LoadImage (bytes); // load the image
		riSelectedPhoto.texture = pic; // texture the photo image
		riSelectedPhoto.rectTransform.sizeDelta = new Vector2(Screen.width - (Screen.width - Screen.height), Screen.height);
		//riSelectedPhoto.RecalculateClipping ();
		// Load appropriate photo metadata and get comment from that
		Photo photoData = new Photo();
		photoData.pathname = pathToPostedPhotos + parent.GetComponent<RawImage> ().name + ".metaphoto";
		BlogUIManager.photoPanel.GetComponentInChildren<Text> ().text = getPhotoComment (photoData);

		BlogUIManager.photoPanel.SetActive (true); //display the photo
	}

	public void getMetaData() {
		#if UNITY_EDITOR
		//  Make sure pictures are loaded into resources
		AssetDatabase.Refresh();
		#endif

		DirectoryInfo dir = new DirectoryInfo(pathToPostedPhotos);
		FileInfo[] info = dir.GetFiles("*.metaphoto");
		foreach (FileInfo file in info) {
			Photo photo = new Photo ();
			string filename = file.Name;
			photo.pathname = pathToPostedPhotos +  filename;
			photo.load ();

			Transform child = transform.Find (filename.Replace (".metaphoto", ""));
			GameObject metaData = new GameObject ();
			metaData.transform.position.Set(20f, 0f, 0f);
			Text textData = metaData.AddComponent<Text> ();

			// Configure comments for photo
			if (photo.comments.Count == 0) {
				Debug.Log ("It looks like there are no comments!");
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
			BlogUIManager.photoPanel.GetComponentInChildren<Text> ().text = getPhotoComment(photo); //spacing the comments

			textData.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			metaData.GetComponent<RectTransform> ().position = new Vector3 (0f, -90f, 0f);
			metaData.transform.SetParent (child, false);
		}
	}
}
