using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

namespace UnityStandardAssets.ImageEffects {
	public class SnapshotCam : MonoBehaviour {

		// List that will contain all of the photos that the player takes
		List<Photo> pics = new List<Photo>();

		// These two ints determine the resolution of the photos taken  
		public int width = 1024;
		public int height = 1024;

		// These are the presets for the size of the aperture and the amount of light the aperture takes in
		float[] apertureSize = {0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f};
		float[] apertureLight = {0.0f, 0.3f, 0.6f, 0.9f, 1.2f , 1.5f};
		// Aperture initialized at index 3
		int apertureInt = 3;

		// Color for the white balance
		Color whiteBalanceColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

		// Indicates which camera is active as main. False = Player View. True = Camera view
		bool cam = false;
		// This is the render texture for the cams digital screen. Used during camera switches
		public RenderTexture camView;

		private AudioSource cameraAudio;
		public AudioClip cam_click;
		public AudioClip cam_shutter;

		void Start () {
			cameraAudio = GetComponent<AudioSource> ();
		}

		void Update () {

			// Switches camera view from Player to Cam
			//if (Input.GetButtonDown("Camera Switch")) {
			cam  = !Input.GetButton("Camera Switch");
			// Player view
			if (cam) {
				// Gets the two cameras one to set the view the other to set the render texture
				Camera c = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
				Camera c2 = GameObject.FindGameObjectWithTag ("PlayerCam").GetComponent<Camera>();
				c2.targetTexture = camView;	// Sets the render texture
				c.Render ();	// Renders the Player view
				c.targetTexture = null;
				cam = !cam;
			}
			// Cam view
			else {
				Camera c = GameObject.FindGameObjectWithTag ("PlayerCam").GetComponent<Camera> ();	// Gets the Cam camera to set the view
				c.Render ();	// Renders the Cam view
				c.targetTexture = null;
				cam = !cam;
			}
			//}

			// When player presses down, a beep is heard
			if (Input.GetButtonDown("Take Photo")){
				cameraAudio.PlayOneShot (cam_click, 0.7f);  //  beep beep
			//  Then upon release the photo is taken
			} else if (Input.GetButtonUp("Take Photo")) {
				cameraAudio.PlayOneShot (cam_shutter, 0.7f);  //  snap
				//GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ().PhotoValues ();
				RenderTexture rt = new RenderTexture (width, height, 24);	// Creates a render texture to pull the pixels from
				Camera c = GameObject.FindGameObjectWithTag ("PlayerCam").GetComponent<Camera>();	// Gets the camera to output to the render tuexture
				c.targetTexture = rt; 
				Texture2D t2d = new Texture2D (width, height, TextureFormat.RGB24, false); // Texture2D that wil be stored in the Photo object
				c.Render (); // Forces the camera to render
				RenderTexture.active = rt;
				t2d.ReadPixels (new Rect (0, 0, width, height), 0, 0); // Reads the pixels
				Photo p = new Photo (); // Creates a new Photo object and then stores t2d and list of visible objects
				p.photo = t2d;
				p.visible = GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ().visibleObjs;
				p.balanceValue = GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ().balance;
				p.spacingValue = GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ().spacing;
				p.InterestingnessValue = GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ().interest;
				c.targetTexture = null;
				RenderTexture.active = null;
				Destroy(rt); 
				pics.Add (p);
				byte[] bytes = t2d.EncodeToPNG(); 
				string filename = Application.dataPath + "/screen" 
					+ System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png"; 
				System.IO.File.WriteAllBytes(filename, bytes);
				Debug.Log(string.Format("Took screenshot to: {0}", filename)); 
			}
			// Aperture Size
			// Increases Aperture Size
			if (Input.GetButtonDown("Aperture Up") && apertureInt < 5) {
				// Access components Depth of Field and Tone Mapping
				DepthOfField dof = GameObject.Find ("PlayerCam").GetComponent<DepthOfField> ();
				Tonemapping tm = GameObject.Find ("PlayerCam").GetComponent<Tonemapping> ();
				apertureInt += 1;
				// Set Aperture Size and Light levels based on presets
				tm.exposureAdjustment = apertureLight [apertureInt];
				dof.aperture = apertureSize [apertureInt];

			}
			// Decreasses Aperture Size
			if (Input.GetButtonDown("Aperture Down") && apertureInt > 0) {
				// Access components Depth of Field and Tone Mapping
				DepthOfField dof = GameObject.Find ("PlayerCam").GetComponent<DepthOfField> ();
				Tonemapping tm = GameObject.Find ("PlayerCam").GetComponent<Tonemapping> ();
				apertureInt -= 1;
				// Set Aperture Size and Light levels based on presets
				dof.aperture = apertureSize [apertureInt];
				tm.exposureAdjustment = apertureLight [apertureInt];
			}
			// White Balance
			// Currently only effects r value of the base white texture
			// Plan to implement color wheel selector for more control/options
			// Decrease Red
			if (Input.GetButtonDown("White Balance Down") && whiteBalanceColor.r > 0.0f) {
				ScreenOverlay so = GameObject.Find ("PlayerCam").GetComponent<ScreenOverlay> ();	// Access component Screen Overlay
				Texture2D t2d = so.texture;	// Copy Screen Overlay texture to a Texture2D
				whiteBalanceColor.r = whiteBalanceColor.r - 0.01f;
				Color[] newColor = new Color[256 * 16];
				// Sets each pixel color to new color
				for (int i = 0; i < newColor.Length; i++) {
					newColor [i] = whiteBalanceColor;
				}
				// Applies changes
				t2d.SetPixels(newColor);
				t2d.Apply ();
			}
			// Increase Red
			if (Input.GetButtonDown("White Balance Up") && whiteBalanceColor.r < 1.0f) {
				ScreenOverlay so = GameObject.Find ("PlayerCam").GetComponent<ScreenOverlay> ();	// Access component Screen Overlay
				Texture2D t2d = so.texture;	// Copy Screen Overlay texture to a Texture2D
				whiteBalanceColor.r = whiteBalanceColor.r + 0.01f;
				Color[] newColor = new Color[256 * 16];
				// Sets each pixel color to new color
				for (int i = 0; i < newColor.Length; i++) {
					newColor [i] = whiteBalanceColor;
				}
				// Applies changes
				t2d.SetPixels(newColor);
				t2d.Apply ();
			}
		}
	}
}
