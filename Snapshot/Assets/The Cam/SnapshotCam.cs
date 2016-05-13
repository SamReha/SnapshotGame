﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

namespace UnityStandardAssets.ImageEffects {
	public class SnapshotCam : MonoBehaviour {

		public GameObject PortraitLens;
		public GameObject WideAngleLens;
		public GameObject TelephotoLens;
		public CameraMenuManager camMenuManager;
		public GameObject FilterPrefab;

		public string currentLens;

		// These two ints determine the resolution of the photos taken  
		public int width = 1024;
		public int height = 1024;

		public bool buttonDownWhilePaused = true;

		public UIManager uimanager;

		// These are the presets for the size of the aperture and the amount of light the aperture takes in and the shutter speed
		float[] apertureSize = {0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f};
		float[] apertureLight = {0.0f, 0.3f, 0.6f, 0.9f, 1.2f , 1.5f};
		float[] shutterSpeed = { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f};
		// Aperture and Shutter initialized at index 3
		int apertureInt = 3;
		int shutterInt = 3;

		// Color for the white balance
		Color whiteBalanceColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

		// This is the render texture for the cams digital screen. Used during camera switches
		public RenderTexture camView;		
		public AudioClip cam_click;
		public AudioClip cam_shutter;

		private AudioSource cameraAudio;
		private GameObject parent;
		private Vector3 cameraHeldUp;
		private Vector3 cameraHeldDown;

        private MemoryCardReader memCardReader;
		private GameObject curLens;

		int lensIter;
		int filterIter;

		void Start () {
			cameraAudio = GetComponent<AudioSource> ();

			parent = GameObject.Find("PlayerCam");
			cameraHeldUp = new Vector3(0.0f, 0.0f, -0.15f);
			cameraHeldDown = new Vector3(0.293f, -0.499f, 0.16f);

			// Set portrait lens
			/*curLens = GameObject.Find (currentLens);
			curLens.GetComponent<MeshRenderer> ().enabled = true;

			parent.GetComponentInParent<DepthOfField> ().focalSize = curLens.GetComponent<Lens> ().focalSize;
			parent.GetComponentInParent<DepthOfField> ().focalLength = curLens.GetComponent<Lens> ().focalDistance;
			parent.GetComponentInParent<Camera> ().fieldOfView = curLens.GetComponent<Lens> ().fieldOfView;
*/
			FilterPrefab.SetActive (false);

			lensIter = 0;
			filterIter = 0;

			memCardReader = GameObject.Find("/MemoryCardManager").GetComponent<MemoryCardReader>();

			PlayerProfile.profile.load ();
			currentLens = PlayerProfile.profile.lensesInBag[0];
			curLens = GameObject.Find (currentLens);
			curLens.GetComponent<MeshRenderer> ().enabled = true;

			parent.GetComponentInParent<DepthOfField> ().focalSize = curLens.GetComponent<Lens> ().focalSize;
			parent.GetComponentInParent<DepthOfField> ().focalLength = curLens.GetComponent<Lens> ().focalDistance;
			parent.GetComponentInParent<Camera> ().fieldOfView = curLens.GetComponent<Lens> ().fieldOfView;
			foreach (string s in PlayerProfile.profile.lensesInBag) {
				Debug.Log ("Lens " + s);
			}
			foreach (string s in PlayerProfile.profile.filtersInBag) {
				Debug.Log ("Filters " + s);
			}
		}

		void Update () {
			//Debug.Log (PlayerProfile.profile.lenses.Count);

			if (Input.GetButton("Camera Switch")) {
				parent.transform.localPosition = cameraHeldUp;
			} else {
				parent.transform.localPosition = cameraHeldDown;
			}

			// When player presses down, a beep is heard
			if (!uimanager.isPaused) {
				if (Input.GetButtonDown ("Take Photo")) {
					cameraAudio.PlayOneShot (cam_click, 0.7f);  //  beep beep
					buttonDownWhilePaused = false;
					//  Then upon release the photo is taken
				} else if (Input.GetButtonUp ("Take Photo") && !buttonDownWhilePaused) {
					if (PlayerProfile.profile.memoryCardCapacity > memCardReader.getPhotoCount ()) {
						cameraAudio.PlayOneShot (cam_shutter, 0.7f);  //  snap
						//GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ().PhotoValues ();
						RenderTexture rt = new RenderTexture (width, height, 24);	// Creates a render texture to pull the pixels from
						Camera c = parent.GetComponent<Camera> ();	// Gets the camera to output to the render tuexture
						c.targetTexture = rt;
						Texture2D t2d = new Texture2D (width, height, TextureFormat.RGB24, false); // Texture2D that wil be stored in the Photo object
						c.Render (); // Forces the camera to render
						RenderTexture.active = rt;
						t2d.ReadPixels (new Rect (0, 0, width, height), 0, 0); // Reads the pixels
						Photo p = new Photo (); // Creates a new Photo object and then stores t2d and list of visible objects
						p.photo = t2d;

						PhotoEval photoEvaluator = GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ();
						photoEvaluator.evaluatePhoto ();
						p.visible = photoEvaluator.visibleObjs;
						p.balanceValue = photoEvaluator.balance;
						p.spacingValue = photoEvaluator.spacing;
						p.interestingnessValue = photoEvaluator.interest;
						p.containsDeer = photoEvaluator.containsDeer;
						p.containsFox = photoEvaluator.containsFox;
						p.containsOwl = photoEvaluator.containsOwl;
						p.takenWithTelephoto = photoEvaluator.takenWithTelephoto;
						p.takenWithWide = photoEvaluator.takenWithWideAngle;

						c.targetTexture = null;
						RenderTexture.active = null;
						Destroy (rt); 
						byte[] bytes = t2d.EncodeToPNG ();

						// Note that pictures now get saved to the UploadQueue directory
						p.pathname = Application.dataPath + "/Resources/UploadQueue/screen"
						+ System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss");
						//  Save image
						string filename = p.pathname + ".png"; 
						System.IO.File.WriteAllBytes (filename, bytes);
						Debug.Log (string.Format ("Took screenshot to: {0}", filename));
						//  Save meta
						p.save ();
						Camera c2 = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
						c.targetTexture = camView;	// Sets the render texture
						c2.Render ();	// Renders the Player view
						c2.targetTexture = null;
						buttonDownWhilePaused = true;

						// Finally, tell the Camera Menu Manager to update its own info
						camMenuManager.updatePhotoCounter();
                        camMenuManager.updatePhotoReviewUI();
                    } else {
						camMenuManager.warnAboutFullCard ();
					}
				}
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
			// Change between camera lenses
			//Portrait
			/*if (Input.GetButtonDown ("Portrait") && currentLens != "port1" && PlayerProfile.profile.lenses.Contains ("port1")) {
				//GameObject parent = GameObject.Find ("PlayerCam");
				GameObject.Find (currentLens).GetComponent<MeshRenderer> ().enabled = false;
				currentLens = "Portrait Lens";
				GameObject.Find ("Portrait Lens").GetComponent<MeshRenderer> ().enabled = true;
				parent.GetComponentInParent<DepthOfField> ().focalSize = PortraitLens.GetComponent<Lens> ().focalSize;
				parent.GetComponentInParent<DepthOfField> ().focalLength = PortraitLens.GetComponent<Lens> ().focalDistance;
				parent.GetComponentInParent<Camera> ().fieldOfView = PortraitLens.GetComponent<Lens> ().fieldOfView;

			}
			// Wide Angle
			if (Input.GetButtonDown ("Wide Angle") && currentLens != "wide1" && PlayerProfile.profile.lenses.Contains ("wide1")) {
				//GameObject parent = GameObject.Find ("PlayerCam");
				GameObject.Find (currentLens).GetComponent<MeshRenderer> ().enabled = false;
				currentLens = "Wide Angle Lens";
				GameObject.Find ("Wide Angle Lens").GetComponent<MeshRenderer> ().enabled = true;
				parent.GetComponentInParent<DepthOfField> ().focalSize = WideAngleLens.GetComponent<Lens> ().focalSize;
				parent.GetComponentInParent<DepthOfField> ().focalLength = WideAngleLens.GetComponent<Lens> ().focalDistance;
				parent.GetComponentInParent<Camera> ().fieldOfView = WideAngleLens.GetComponent<Lens> ().fieldOfView;
			}
			// Telephoto
			if (Input.GetButtonDown ("Telephoto") && currentLens != "tele1" && PlayerProfile.profile.lenses.Contains ("tele1")) {
				//GameObject parent = GameObject.Find ("PlayerCam");
				GameObject.Find (currentLens).GetComponent<MeshRenderer> ().enabled = false;
				currentLens = "telephoto_lens";
				GameObject.Find ("telephoto_lens").GetComponent<MeshRenderer> ().enabled = true;
				parent.GetComponentInParent<DepthOfField> ().focalSize = TelephotoLens.GetComponent<Lens> ().focalSize;
				parent.GetComponentInParent<DepthOfField> ().focalLength = TelephotoLens.GetComponent<Lens> ().focalDistance;
				parent.GetComponentInParent<Camera> ().fieldOfView = TelephotoLens.GetComponent<Lens> ().fieldOfView;
			}*/

			if (Input.GetButtonDown ("Portrait")) {

				lensIter++;
				Debug.Log ("Lens Iter" + lensIter);
				if (lensIter == PlayerProfile.profile.lensesInBag.Count) {
					lensIter = 0;
				}
				GameObject.Find (currentLens).GetComponent<MeshRenderer> ().enabled = false;
				currentLens = PlayerProfile.profile.lensesInBag [lensIter];
				Debug.Log ("Current: " + currentLens);
				GameObject.Find (currentLens).GetComponent<MeshRenderer> ().enabled = true;
				parent.GetComponentInParent<DepthOfField> ().focalSize = GameObject.Find(currentLens).GetComponent<Lens> ().focalSize;
				parent.GetComponentInParent<DepthOfField> ().focalLength = GameObject.Find(currentLens).GetComponent<Lens> ().focalDistance;
				parent.GetComponentInParent<Camera> ().fieldOfView = GameObject.Find(currentLens).GetComponent<Lens> ().fieldOfView;

			}

			if (Input.GetButtonDown ("Wide Angle")) {
				filterIter++;
				Debug.Log (PlayerProfile.profile.filtersInBag.Count);
				if (filterIter == PlayerProfile.profile.filtersInBag.Count) {
					filterIter = 0;
					Debug.Log (PlayerProfile.profile.filtersInBag [filterIter]);
					FilterPrefab.SetActive (false);
				} else {
					Debug.Log (PlayerProfile.profile.filtersInBag [filterIter]);
					Texture newFilter = Resources.Load (PlayerProfile.profile.filtersInBag [filterIter], typeof(Texture)) as Texture;
					FilterPrefab.SetActive (true);
					FilterPrefab.GetComponent<MeshRenderer> ().material.SetTexture ("_MainTex", newFilter);
				}
			}


			if (Input.GetButtonDown("Shutter Speed Up") && shutterInt < 5) {
				// Access component Camera Motion Blur
				CameraMotionBlur cmb = GameObject.Find ("PlayerCam").GetComponent<CameraMotionBlur> ();
				shutterInt += 1;
				// Set shutter speed based on presets
				cmb.velocityScale = shutterSpeed[shutterInt];

			}
			// Decreasses Aperture Size
			if (Input.GetButtonDown("Shutter Speed Down") && shutterInt > 0) {
				// Access component Camera Motion Blur
				CameraMotionBlur cmb = GameObject.Find ("PlayerCam").GetComponent<CameraMotionBlur> ();
				shutterInt -= 1;
				// Set shutter speed based on presets
				cmb.velocityScale = shutterSpeed[shutterInt];
			}
		}
	}
}
