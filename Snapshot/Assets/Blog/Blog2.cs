using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Blog2 : MonoBehaviour {

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
        int counter = 0;
        DirectoryInfo dir = new DirectoryInfo(Path.Combine("Assets", "Resources"));
        Debug.Log(dir);
        FileInfo[] info = dir.GetFiles("*.png");
        foreach (FileInfo f in info)
        {
            Debug.Log(f);
            string filename = info[counter].Name;
            Debug.Log(filename);
            GameObject curPicture = (GameObject) Instantiate(newPicture);
            curPicture.GetComponent<RawImage>().texture = Resources.Load(filename.Replace(".png", "")) as Texture;
            curPicture.transform.SetParent(this.transform, false);
            counter++;
        }
	}
}
