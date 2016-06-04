using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

namespace UnityStandardAssets.ImageEffects {
	public class SnapshotCam : MonoBehaviour {

		public FirstPersonController player;
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
		float[] shutterSpeed = { 1.0f, 5.0f, 10.0f, 20.0f, 35.0f, 50.0f};
		float[] focus = { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f, 1.2f, 1.4f, 1.6f, 1.8f, 2.0f, };
		// Aperture and Shutter initialized at index 3
		int apertureInt = 3;
		int shutterInt = 3;
		int focusInt = 5;

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
		private bool photoReview;

		int lensIter;
		int filterIter;

		void Start () {
			cameraAudio = GetComponent<AudioSource> ();

			parent = GameObject.Find("PlayerCam");
			cameraHeldUp   = new Vector3( 0.009f, 0.030f,-0.100f);
            cameraHeldDown = new Vector3( 0.293f,-0.499f, 0.300f);

			photoReview = false;

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
			/*foreach (string s in PlayerProfile.profile.lensesInBag) {
				Debug.Log ("Lens " + s);
			}
			foreach (string s in PlayerProfile.profile.filtersInBag) {
				Debug.Log ("Filters " + s);
			}*/
		}

		void Update () {
			//Debug.Log (PlayerProfile.profile.lenses.Count);
			if(Input.GetButtonDown("Photo Review")){
				photoReview = !photoReview;
			}
			if (!photoReview) {
				if (Input.GetButtonDown ("Camera Switch")) {
					parent.transform.localPosition = cameraHeldUp;
				} 
				if (Input.GetButtonUp ("Camera Switch")) {
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
							RenderTexture renderTexture = new RenderTexture (width, height, 24);	// Creates a render texture to pull the pixels from
							Camera parentCamera = parent.GetComponent<Camera> ();	// Gets the camera to output to the render tuexture
							parentCamera.targetTexture = renderTexture;
							Texture2D photoTexture = new Texture2D (width, height, TextureFormat.RGB24, false); // Texture2D that wil be stored in the Photo object
							parentCamera.Render (); // Forces the camera to render
							RenderTexture.active = renderTexture;
							photoTexture.ReadPixels (new Rect (0, 0, width, height), 0, 0); // Reads the pixels
							Photo photoMetaData = new Photo (); // Creates a new Photo object and then stores t2d and list of visible objects
							photoMetaData.photo = photoTexture;

							PhotoEval photoEvaluator = GameObject.Find ("Camera Prefab").GetComponent<PhotoEval> ();
							photoEvaluator.evaluatePhoto ();
							photoMetaData.balanceValue = photoEvaluator.balance;
							photoMetaData.spacingValue = photoEvaluator.spacing;
							photoMetaData.interestingnessValue = photoEvaluator.interest;
							photoMetaData.containsDeer = photoEvaluator.containsDeer;
							photoMetaData.containsFox = photoEvaluator.containsFox;
							photoMetaData.takenWithTelephoto = photoEvaluator.takenWithTelephoto;
							photoMetaData.takenWithWide = photoEvaluator.takenWithWideAngle;
							photoMetaData.takenWithFilter = photoEvaluator.takenWithFilter;

							parentCamera.targetTexture = null;
							RenderTexture.active = null;
							Destroy (renderTexture); 
							byte[] bytes = photoTexture.EncodeToPNG ();

							// Note that pictures now get saved to the UploadQueue directory
							photoMetaData.pathname = Application.dataPath + "/Resources/UploadQueue/screen"
							+ System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss");
							//  Save image
							string filename = photoMetaData.pathname + ".png"; 
							System.IO.File.WriteAllBytes (filename, bytes);
							Debug.Log (string.Format ("Took screenshot to: {0}", filename));
							//  Save meta
							photoMetaData.save ();
							Camera mainCameraCam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
							parentCamera.targetTexture = camView;	// Sets the render texture
							mainCameraCam.Render ();	// Renders the Player view
							mainCameraCam.targetTexture = null;
							buttonDownWhilePaused = true;

							// Finally, tell the Camera Menu Manager to update its own info
							camMenuManager.updatePhotoCounter ();
							camMenuManager.updatePhotoReviewUI ();
						} else {
							camMenuManager.warnAboutFullCard ();
						}
					}
				}
				// Aperture Size
				// Increases Aperture Size
				if (Input.GetButtonDown ("Aperture Up") && apertureInt < 5) {
					// Access components Depth of Field and Tone Mapping
					DepthOfField dof = GameObject.Find ("PlayerCam").GetComponent<DepthOfField> ();
					Tonemapping tm = GameObject.Find ("PlayerCam").GetComponent<Tonemapping> ();
					apertureInt += 1;
					// Set Aperture Size and Light levels based on presets
					tm.exposureAdjustment = apertureLight [apertureInt];
					dof.aperture = apertureSize [apertureInt];

				}
				// Decreasses Aperture Size
				if (Input.GetButtonDown ("Aperture Down") && apertureInt > 0) {
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
				if (Input.GetButtonDown ("White Balance Down")) {
					ScreenOverlay so = GameObject.Find ("PlayerCam").GetComponent<ScreenOverlay> ();	// Access component Screen Overlay
					Texture2D t2d = so.texture;	// Copy Screen Overlay texture to a Texture2D
					if (whiteBalanceColor.b >= 1.0f && whiteBalanceColor.r >= 0.0f) {
						Debug.Log ("Test 1");
						whiteBalanceColor.r = whiteBalanceColor.r - 0.01f;
					} else if (whiteBalanceColor.r >= 1.0f && whiteBalanceColor.b < 1.0f) {
						Debug.Log ("Test 2");
						whiteBalanceColor.b = whiteBalanceColor.b + 0.01f;
					}
					Color[] newColor = new Color[256 * 16];
					// Sets each pixel color to new color
					for (int i = 0; i < newColor.Length; i++) {
						newColor [i] = whiteBalanceColor;
					}
					// Applies changes
					t2d.SetPixels (newColor);
					t2d.Apply ();
				}
				// Increase Red
				if (Input.GetButtonDown ("White Balance Up")) {
					ScreenOverlay so = GameObject.Find ("PlayerCam").GetComponent<ScreenOverlay> ();	// Access component Screen Overlay
					Texture2D t2d = so.texture;	// Copy Screen Overlay texture to a Texture2D
					if (whiteBalanceColor.r >= 1.0f && whiteBalanceColor.b >= 0.0f) {
						Debug.Log ("Test 3");
						whiteBalanceColor.b = whiteBalanceColor.b - 0.01f;
					} else if (whiteBalanceColor.b >= 1.0f && whiteBalanceColor.r < 1.0f) {
						Debug.Log ("Test 4");
						whiteBalanceColor.r = whiteBalanceColor.r + 0.01f;
					}
					Debug.Log (whiteBalanceColor.r + " , " + whiteBalanceColor.b);
					Color[] newColor = new Color[256 * 16];
					// Sets each pixel color to new color
					for (int i = 0; i < newColor.Length; i++) {
						newColor [i] = whiteBalanceColor;
					}
					// Applies changes
					t2d.SetPixels (newColor);
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
					parent.GetComponentInParent<DepthOfField> ().focalSize = GameObject.Find (currentLens).GetComponent<Lens> ().focalSize;
					parent.GetComponentInParent<DepthOfField> ().focalLength = GameObject.Find (currentLens).GetComponent<Lens> ().focalDistance;
					parent.GetComponentInParent<Camera> ().fieldOfView = GameObject.Find (currentLens).GetComponent<Lens> ().fieldOfView;

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


				if (Input.GetButtonDown ("Shutter Speed Up") && shutterInt < 5) {
					// Access component Camera Motion Blur
					CameraMotionBlur cmb = GameObject.Find ("PlayerCam").GetComponent<CameraMotionBlur> ();
					shutterInt += 1;
					// Set shutter speed based on presets
					cmb.velocityScale = shutterSpeed [shutterInt];

				}
				// Decreasses Aperture Size
				if (Input.GetButtonDown ("Shutter Speed Down") && shutterInt > 0) {
					// Access component Camera Motion Blur
					CameraMotionBlur cmb = GameObject.Find ("PlayerCam").GetComponent<CameraMotionBlur> ();
					shutterInt -= 1;
					// Set shutter speed based on presets
					cmb.velocityScale = shutterSpeed [shutterInt];
				}

				if (Input.GetButtonDown ("Focus In") && focusInt > 0) {
					DepthOfField dof = GameObject.Find ("PlayerCam").GetComponent<DepthOfField> ();
					focusInt -= 1;
					dof.focalSize = focus [focusInt];
					Debug.Log (dof.focalSize);
				}

				if (Input.GetButtonDown ("Focus Out") && focusInt < 10) {
					DepthOfField dof = GameObject.Find ("PlayerCam").GetComponent<DepthOfField> ();
					focusInt += 1;
					dof.focalSize = focus [focusInt];
					Debug.Log (dof.focalSize);
				}
			}
		}

		public string currentFilter() {
			if (PlayerProfile.profile.filtersInBag.Count == 0) return "";
			return PlayerProfile.profile.filtersInBag [filterIter];
		}
	}
}
