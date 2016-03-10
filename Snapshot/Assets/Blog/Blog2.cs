using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Blog2 : MonoBehaviour {

    public GameObject newPicture;
    [MenuItem ("AssetDatabase/Snapshot")]

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        int counter = 0;
        DirectoryInfo dir = new DirectoryInfo("Assets");
        FileInfo[] info = dir.GetFiles("*.png");
        foreach (FileInfo f in info)
        {
            print(f);
            string path = dir.ToString();
            string filename = info[counter].Name;
            print(filename);
#if UNITY_EDITOR
            AssetDatabase.MoveAsset(Path.Combine(path, filename), Path.Combine(Path.Combine(path, "Resources"), filename)); // move from [E....\Assets\png] name to [E....\Assets\Resources\png name]
#endif
            GameObject curPicture = (GameObject) Instantiate(newPicture);
            curPicture.GetComponent<RawImage>().texture = Resources.Load(filename.Replace(".png", "")) as Texture;
            curPicture.transform.SetParent(this.transform, false);
            counter++;
        }
	}
}
