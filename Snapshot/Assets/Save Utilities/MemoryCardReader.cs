using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MemoryCardReader : MonoBehaviour {

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}

    public int getPhotoCount() {
        DirectoryInfo equipmentDir = new DirectoryInfo(Application.dataPath + "/Resources/UploadQueue/");
        FileInfo[] photoInfo = equipmentDir.GetFiles("*.png");
        return photoInfo.Length;
    }

    public List<Photo> getUploadQueueMetas() {
        DirectoryInfo equipmentDir = new DirectoryInfo(Application.dataPath + "/Resources/UploadQueue/");
        FileInfo[] photoInfos = equipmentDir.GetFiles("*.metaphoto");

        foreach (FileInfo photoInfo in photoInfos) {
            Debug.Log(photoInfo.Name);
        }

        // TODO: finish this function
        return new List<Photo>();
    }

    public List<Texture2D> getUploadQueueTextures() {
        DirectoryInfo equipmentDir = new DirectoryInfo(Application.dataPath + "/Resources/UploadQueue/");
        FileInfo[] photoInfos = equipmentDir.GetFiles("*.png");

        List<Texture2D> textureBuffer = new List<Texture2D>();

        foreach (FileInfo photoInfo in photoInfos) {
            Debug.Log(photoInfo.Name);

            Texture2D pic = new Texture2D(2, 2);
            byte[] bytes = File.ReadAllBytes(Application.dataPath + "/Resources/UploadQueue/" + photoInfo.Name);
            pic.LoadImage(bytes);

            textureBuffer.Add(pic);
        }

        return textureBuffer;
    }

    public void deletePhoto(Photo photo) {

    }
}
