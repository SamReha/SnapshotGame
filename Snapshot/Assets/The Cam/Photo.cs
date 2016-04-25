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
    public bool containsFox = false;
    public bool containsOwl = false;
    public bool containsDeer = false;
    public bool containsPosingAnimal = false;
    public bool takenWithTelephoto = false;
    public bool takenWithWide = false;


	// Use this for initialization
	void Start () {
		createProfile ();
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
			string fullPath = pathname;

			BinaryFormatter binForm = new BinaryFormatter ();
			FileStream saveFile = File.Open (fullPath, FileMode.Open);

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
		string fullPath = pathname + ".metaphoto";

		BinaryFormatter binForm = new BinaryFormatter ();

		FileStream saveFile;
		if (File.Exists (fullPath)) {
			saveFile = File.Open (fullPath, FileMode.Open);
		} else saveFile = File.Create (fullPath);

		MetaPhoto saveData = new MetaPhoto ();
		saveData.balan = balanceValue;
		saveData.spaci = spacingValue;
		saveData.inter = interestingnessValue;
        saveData.containsFox = containsFox;
        saveData.containsOwl = containsOwl;
        saveData.containsDeer = containsDeer;
        saveData.containsPosingAnimal = containsPosingAnimal;
        saveData.takenWithTelephoto = takenWithTelephoto;
        saveData.takenWithWide = takenWithWide;

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
        containsFox = false;
        containsOwl = false;
        containsDeer = false;
        containsPosingAnimal = false;
        takenWithTelephoto = false;
        takenWithWide = false;
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
        public bool containsFox;
        public bool containsOwl;
        public bool containsDeer;
        public bool containsPosingAnimal;
        public bool takenWithTelephoto;
        public bool takenWithWide;
    }
}
