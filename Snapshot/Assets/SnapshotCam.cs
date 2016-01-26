using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

namespace UnityStandardAssets.ImageEffects {
	public class SnapshotCam : MonoBehaviour {
		List<Texture2D> pics;
		float[] apertureSize = {0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f};
		int apertureInt = 3;
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			if (Input.GetMouseButtonDown (1)) {
				RenderTexture rt = new RenderTexture (1024, 1024, 24);
				Camera c = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
				c.targetTexture = rt;
				Texture2D t2d = new Texture2D (1024, 1024, TextureFormat.RGB24, false);
				c.Render ();
				RenderTexture.active = rt;
				t2d.ReadPixels (new Rect (0, 0, 1024, 1024), 0, 0);
				c.targetTexture = null;
				RenderTexture.active = null;
				Destroy(rt); 
				pics.Add (t2d);
			}
			if (Input.GetMouseButtonDown (0)) {
				GameObject.FindGameObjectWithTag ("MainCamera");
			}
			if (Input.GetKeyDown (KeyCode.E) && apertureInt < 5) {
				DepthOfField d = GameObject.Find ("PlayerCam").GetComponent<DepthOfField> ();
				apertureInt += 1;
				d.aperture = apertureSize [apertureInt];
				print (d.aperture);

			}
			if (Input.GetKeyDown (KeyCode.Q) && apertureInt > 0) {
				DepthOfField d = GameObject.Find ("PlayerCam").GetComponent<DepthOfField> ();
				apertureInt -= 1;
				d.aperture = apertureSize [apertureInt];
				print (d.aperture);
			}
		}
	}
}
