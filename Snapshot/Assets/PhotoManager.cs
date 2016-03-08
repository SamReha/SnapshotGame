using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhotoManager : MonoBehaviour {

	List<Photo> uploadPending;
	List<Photo> uploaded;

	public void Start () {
		uploadPending = GameObject.Find ("PersistentGlobal").GetComponent<PersistentGlobals>().pics;
		//uploaded = GameObject.Find ("Camera Prefab").GetComponent<SnapshotCam> ().pics;
	}

	public void Update () {
		if (uploadPending.Count > 0) {
			//  UPLOAD MODE
		} else {
			//  Regular mode.
		}
	}

	/*
	public void PNGAnimation () {
		
		string filename = imageNameBase + ZeroPad(index,imageNameZeroPadding);
		Resources.UnloadAsset(texture);
		texture = Resources.Load(fileName);
		renderer.material.mainTexture = texture;

	}

	public static T[] GetAtPath<T> (string path) {

		ArrayList al = new ArrayList();
		string [] fileEntries = Directory.GetFiles(Application.dataPath+"/"+path);
		foreach(string fileName in fileEntries)
		{
			int index = fileName.LastIndexOf("/");
			string localPath = "Assets/" + path;

			if (index > 0)
				localPath += fileName.Substring(index);

			Object t = Resources.LoadAssetAtPath(localPath, typeof(T));

			if(t != null)
				al.Add(t);
		}
		T[] result = new T[al.Count];
		for(int i=0;i<al.Count;i++)
			result[i] = (T)al[i];

		return result;
	} */
}
