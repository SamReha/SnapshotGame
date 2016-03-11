using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class Photo {

	public Texture2D photo;
	public string pathname;
	public List<GameObject> visible = new List<GameObject> ();
	public float balanceValue = 0f;
	public float spacingValue = 0f;
	public float interestingnessValue = 0f;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	 * Tries to open a save file for the current player.
	 * If it can't, it creates a new save file.
	 * In the future, this may need to consider an player
	 * name so we can have multiple save files.
	 */
	public void load() {
		if (File.Exists(pathname)) {
			Debug.Log("Loading file at " + pathname);

			BinaryFormatter binForm = new BinaryFormatter ();
			FileStream saveFile = File.Open (pathname, FileMode.Open);

			MetaPhoto saveData = (MetaPhoto)binForm.Deserialize (saveFile);
			saveFile.Close ();

			balanceValue = saveData.balan;
			spacingValue = saveData.spaci;
			interestingnessValue = saveData.inter;
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
		Debug.Log("Saving file to " + pathname);

		BinaryFormatter binForm = new BinaryFormatter ();

		FileStream saveFile;
		if (File.Exists (pathname)) {
			saveFile = File.Open (pathname, FileMode.Open);
		} else saveFile = File.Create (pathname);

		MetaPhoto saveData = new MetaPhoto ();
		saveData.balan = balanceValue;
		saveData.spaci = spacingValue;
		saveData.inter = interestingnessValue;

		binForm.Serialize (saveFile, saveData);
		saveFile.Close ();
	}

	/*
	 * Creates an empty profile
	 */
	private void createProfile() {
		balanceValue = 0;
		spacingValue = 0;
		interestingnessValue = 0;
		save ();
	}
		/*
	 * InternalProfile is a serializeable copy of the player's data.
	 * 
	 * Why not just have this be a public member? Well, it's easier to type
	 * profile.money than profile.internalprofile.money. It also allows us
	 * to preform binary serialization (that way save files aren't ever user-editable)
	 */
	[Serializable]
	class MetaPhoto {
		public float balan;
		public float spaci;
		public float inter;
	}
}
