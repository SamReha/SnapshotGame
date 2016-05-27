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

    private string pathToPostedPhotos;
	private int numPhotos = 0;
	private int rectTransforms = 0; //debug

#if UNITY_EDITOR
    [MenuItem ("AssetDatabase/Snapshot")]
#endif

	// Use this for initialization
	void Start () {
        pathToPostedPhotos = Application.dataPath + "/Resources/PostedImages/";
        updatePhotos();
        getMetaData();
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
		for(int i = 0; i < info.Length; i++) {
			string filename = info[i].Name;
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

	public string getPhotoComment(Photo p) {
		//Photo p = new Photo ();
		p.load ();
		return p.comment;
	}

	public void viewComments() {
		RawImage riSelectedPhoto;
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("imagebtn");
		GameObject parent = GameObject.FindGameObjectWithTag("imagebtn");

		foreach (GameObject go in objs) {
			if (go.GetComponent<Button> ().enabled) {
				parent = go;
			}
		}
		riSelectedPhoto = BlogUIManager.photoPanel.GetComponentInChildren<RawImage> ();
		Texture2D pic = new Texture2D (2, 2);
		byte[] bytes = File.ReadAllBytes(pathToPostedPhotos + parent.GetComponent<RawImage>().name + ".png");
		pic.LoadImage (bytes);
		riSelectedPhoto.texture = pic;
		getMetaData ();
		BlogUIManager.photoPanel.SetActive (true);
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
		//	Debug.Log (photo.hasComment);


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

			if (photo.hasComment) {
				BlogUIManager.photoPanel.GetComponentInChildren<Text> ().text = getPhotoComment(photo);
			} else {
				photo.comment = gameObject.GetComponent<CommentGenerator> ().GenerateComment (markup);
				photo.hasComment = true;
				photo.save ();
				photo.pathname = pathToPostedPhotos +  filename;
				BlogUIManager.photoPanel.GetComponentInChildren<Text> ().text = getPhotoComment(photo);
			
			}
				//textData.text = gameObject.GetComponent<CommentGenerator> ().GenerateComment (markup);
			   //BlogUIManager.photoPanel.GetComponentInChildren<Text> ().text = photo.comment;

			//Debug.Log (textData.text);
			textData.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			metaData.GetComponent<RectTransform> ().position = new Vector3 (0f, -90f, 0f);
			//Debug.Log ("CHILD: " + child);

			metaData.transform.SetParent (child, false);
		}
	}
}
