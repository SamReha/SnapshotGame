using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

namespace UnityStandardAssets.ImageEffects {
	public class SnapshotCam : MonoBehaviour {
		List<Texture2D> pics = new List<Texture2D>();
		float[] apertureSize = {0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f};
		float[] apertureLight = {0.0f, 0.3f, 0.6f, 0.9f, 1.2f , 1.5f};
		int apertureInt = 3;
		Color whiteBalanceColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {
			if (Input.GetMouseButtonDown (1)) {
				RenderTexture rt = new RenderTexture (1024, 1024, 24);
				Camera c = GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<Camera>();
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
			if (Input.GetKeyDown (KeyCode.E) && apertureInt < 5) {
				DepthOfField dof = GameObject.Find ("PlayerCam").GetComponent<DepthOfField> ();
				Tonemapping tm = GameObject.Find ("PlayerCam").GetComponent<Tonemapping> ();
				apertureInt += 1;
				tm.exposureAdjustment = apertureLight [apertureInt];
				dof.aperture = apertureSize [apertureInt];
				print (dof.aperture);

			}
			if (Input.GetKeyDown (KeyCode.Q) && apertureInt > 0) {
				DepthOfField dof = GameObject.Find ("PlayerCam").GetComponent<DepthOfField> ();
				Tonemapping tm = GameObject.Find ("PlayerCam").GetComponent<Tonemapping> ();
				apertureInt -= 1;
				dof.aperture = apertureSize [apertureInt];
				tm.exposureAdjustment = apertureLight [apertureInt];
				print (dof.aperture);
			}
			if (Input.GetKeyDown (KeyCode.R) && whiteBalanceColor.r > 0.0f) {
				ScreenOverlay so = GameObject.Find ("PlayerCam").GetComponent<ScreenOverlay> ();
				Texture2D t2d = so.texture;
				whiteBalanceColor.r = whiteBalanceColor.r - 0.01f;
				Color[] newColor = new Color[256 * 16];
				for (int i = 0; i < newColor.Length; i++) {
					newColor [i] = whiteBalanceColor;
				}
				t2d.SetPixels(newColor);
				t2d.Apply ();
				print (whiteBalanceColor);
			}
			if (Input.GetKeyDown (KeyCode.F) && whiteBalanceColor.r < 1.0f) {
				ScreenOverlay so = GameObject.Find ("PlayerCam").GetComponent<ScreenOverlay> ();
				Texture2D t2d = so.texture;
				whiteBalanceColor.r = whiteBalanceColor.r + 0.01f;
				Color[] newColor = new Color[256 * 16];
				for (int i = 0; i < newColor.Length; i++) {
					newColor [i] = whiteBalanceColor;
				}
				t2d.SetPixels(newColor);
				t2d.Apply ();
				print (whiteBalanceColor);

			}
		}
	}
}
