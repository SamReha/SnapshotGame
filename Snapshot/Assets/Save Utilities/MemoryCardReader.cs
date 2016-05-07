using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MemoryCardReader : MonoBehaviour {
    public string pathToPostedPhotos;
    public string pathToUploadQueue;

    // Use this for initialization
    void Awake() {
        pathToPostedPhotos = Application.dataPath + "/Resources/PostedImages/";
        pathToUploadQueue = Application.dataPath + "/Resources/UploadQueue/";
    }

    void Start () {}
	
	// Update is called once per frame
	void Update () {}

    public int getPhotoCount() {
        DirectoryInfo equipmentDir = new DirectoryInfo(pathToUploadQueue);
        FileInfo[] photoInfo = equipmentDir.GetFiles("*.png");
        return photoInfo.Length;
    }

    public List<Photo> getUploadQueueMetas() {
        DirectoryInfo equipmentDir = new DirectoryInfo(pathToUploadQueue);
        FileInfo[] photoInfos = equipmentDir.GetFiles("*.metaphoto");

        foreach (FileInfo photoInfo in photoInfos) {
            Debug.Log(photoInfo.Name);
        }

        // TODO: finish this function
        return new List<Photo>();
    }

    public List<Texture2D> getUploadQueueTextures() {
        DirectoryInfo equipmentDir = new DirectoryInfo(pathToUploadQueue);
        FileInfo[] photoInfos = equipmentDir.GetFiles("*.png");

        List<Texture2D> textureBuffer = new List<Texture2D>();

        foreach (FileInfo photoInfo in photoInfos) {
            Texture2D pic = new Texture2D(2, 2);
            byte[] bytes = File.ReadAllBytes(pathToUploadQueue + photoInfo.Name);
            pic.LoadImage(bytes);

            textureBuffer.Add(pic);
        }

        return textureBuffer;
    }

    public void deleteFromUploadQueue(Photo photo) {
        string partialPath = pathToUploadQueue + photo.pathname;

        File.Delete(partialPath + ".png");
        File.Delete(partialPath + ".metaphoto");

        // Don't forget to clean up the meta files if they've already been generated
        if (File.Exists(partialPath + ".png.meta")) {
            File.Delete(partialPath + ".png.meta");
            File.Delete(partialPath + ".metaphoto.meta");
        }
    }
}
