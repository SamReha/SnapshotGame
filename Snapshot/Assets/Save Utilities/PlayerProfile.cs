using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerProfile : MonoBehaviour {
	private string savePath;

	public static PlayerProfile profile;
	public float money;
	public List<string> lenses;
	public List<string> postedPhotos;


	//  Tutorial flags
	public bool tutFlagMovement;
	public bool tutFlagAim;
	public bool tutFlagSnap;
	public bool tutFlagLook;
	public bool tutFlagJump;
	public bool tutFlagRun;
	public bool tutFlagAperture;
	public bool tutFlagWhiteBalance;
	public bool tutFlagChangeLens;
	public bool tutFlagShutterSpeed;
	public bool tutFlagViewControls;

	// Use this for initialization
	void Start () {}

	/*
	 * Turn this shit into a singleton
	 */
	void Awake() {
		if (profile == null) {
			DontDestroyOnLoad (gameObject);
			profile = this;
			profile.savePath = Application.persistentDataPath + "/playerprofile.save";
		} else if (profile != this) {
			Destroy (gameObject);
		}
	}

	/*
	 * Tries to open a save file for the current player.
	 * If it can't, it creates a new save file.
	 * In the future, this may need to consider an player
	 * name so we can have multiple save files.
	 */
	public void load() {
		if (File.Exists(savePath)) {
			Debug.Log("Loading file at " + savePath);

			BinaryFormatter binForm = new BinaryFormatter ();
			FileStream saveFile = File.Open (savePath, FileMode.Open);

			InternalProfile saveData = (InternalProfile)binForm.Deserialize (saveFile);
			saveFile.Close ();
			//  Load in values
			money = saveData.money;
			lenses = saveData.lenses;
			postedPhotos = saveData.postedPhotos;

			tutFlagMovement = saveData.tutFlagMovement;
			tutFlagAim = saveData.tutFlagAim;
			tutFlagSnap = saveData.tutFlagSnap;
			tutFlagLook = saveData.tutFlagLook;
			tutFlagJump = saveData.tutFlagJump;
			tutFlagRun = saveData.tutFlagRun;
			tutFlagAperture = saveData.tutFlagAperture;
			tutFlagWhiteBalance = saveData.tutFlagWhiteBalance;
			tutFlagChangeLens = saveData.tutFlagChangeLens;
			tutFlagShutterSpeed = saveData.tutFlagShutterSpeed;
			tutFlagViewControls = saveData.tutFlagViewControls;

		} else {
			Debug.Log("Save file does not exist! Creating an empty one...");
			createProfile ();
		}
	}

	/*
	 * Tries to write to a save file for the current player.
	 * In the future, this may need to consider a player
	 * name so we can have multiple save files.
	 */
	public void save() {
		Debug.Log("Saving file to " + savePath);

		BinaryFormatter binForm = new BinaryFormatter ();

		FileStream saveFile;
		if (File.Exists (savePath)) {
			saveFile = File.Open (savePath, FileMode.Open);
		} else saveFile = File.Create (savePath);

		InternalProfile saveData = new InternalProfile ();
		saveData.money = money;
		saveData.lenses = lenses;
		saveData.postedPhotos = postedPhotos;
		saveData.tutFlagMovement = tutFlagMovement;
		saveData.tutFlagAim = tutFlagAim;
		saveData.tutFlagSnap = tutFlagSnap;
		saveData.tutFlagLook = tutFlagLook;
		saveData.tutFlagJump = tutFlagJump;
		saveData.tutFlagRun = tutFlagRun;
		saveData.tutFlagAperture = tutFlagAperture;
		saveData.tutFlagWhiteBalance = tutFlagWhiteBalance;
		saveData.tutFlagChangeLens = tutFlagChangeLens;
		saveData.tutFlagShutterSpeed = tutFlagShutterSpeed;
		saveData.tutFlagViewControls = tutFlagViewControls;

		binForm.Serialize (saveFile, saveData);
		saveFile.Close ();
	}

	/*
	 * Creates a default profile
	 */
	private void createProfile() {
		money = 0;
		lenses = new List<string> ();
		postedPhotos = new List<string> ();

		lenses.Add ("port1");

		//  Tutorial flags
		tutFlagMovement = false;
		tutFlagAim = false;
		tutFlagSnap = false;
		tutFlagLook = false;
		tutFlagJump = false;
		tutFlagRun = false;
		tutFlagAperture = false;
		tutFlagWhiteBalance = false;
		tutFlagChangeLens = false;
		tutFlagShutterSpeed = false;
		tutFlagViewControls = false;

		save ();
	}
}

/*
 * InternalProfile is a serializeable copy of the player's data.
 * 
 * Why not just have this be a public member? Well, it's easier to type
 * profile.money than profile.internalprofile.money. It also allows us
 * to preform binary serialization (that way save files aren't ever user-editable)
 */

[Serializable]
class InternalProfile {
	public float money;
	public List<string> lenses;
	public List<string> postedPhotos;
	//  Tutorial flags
	public bool tutFlagMovement;
	public bool tutFlagAim;
	public bool tutFlagSnap;
	public bool tutFlagLook;
	public bool tutFlagJump;
	public bool tutFlagRun;
	public bool tutFlagAperture;
	public bool tutFlagWhiteBalance;
	public bool tutFlagChangeLens;
	public bool tutFlagShutterSpeed;
	public bool tutFlagViewControls;
}