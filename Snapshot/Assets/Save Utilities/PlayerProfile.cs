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
    public uint memoryCardCapacity;
	public float bagSize;
    public List<string> lenses;
	public List<string> filters;
	public List<string> postedPhotos;
	public float maxInterestTotal;
	public float maxInterestIndividual;
	public List<string> lensesInBag;
	public List<string> filtersInBag;

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

			money = saveData.money;
            memoryCardCapacity = saveData.memoryCardCapacity;
			bagSize = saveData.bagSize;
			lenses = saveData.lenses;
			filters = saveData.filters;
			postedPhotos = saveData.postedPhotos;
			maxInterestTotal = saveData.maxInterestTotal;
			maxInterestIndividual = saveData.maxInterestIndividual;
			lensesInBag = saveData.lensesInBag;
			filtersInBag = saveData.filtersInBag;
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
        saveData.memoryCardCapacity = memoryCardCapacity;
		saveData.bagSize = bagSize;
		saveData.lenses = lenses;
		saveData.filters = filters;
		saveData.postedPhotos = postedPhotos;
		saveData.maxInterestTotal = maxInterestTotal;
		saveData.maxInterestIndividual = maxInterestIndividual;
		saveData.lensesInBag = lensesInBag;
		saveData.filtersInBag = filtersInBag;

		binForm.Serialize (saveFile, saveData);
		saveFile.Close ();
	}

	/*
	 * Creates a default profile
	 */
	private void createProfile() {
		money = 0;
        memoryCardCapacity = 8;
		bagSize = 5;
		lenses = new List<string> ();
		filters = new List<string> ();
		postedPhotos = new List<string> ();
		maxInterestTotal = 20f;
		maxInterestIndividual = 5f;
		lensesInBag = new List<string> ();
		filtersInBag = new List<string> ();

		filters.Add ("clear");
		lenses.Add ("port1");
		filtersInBag.Add ("clear");
		lensesInBag.Add ("port1");

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
    public uint memoryCardCapacity;
	public float bagSize;
	public List<string> lenses;
	public List<string> filters;
	public List<string> postedPhotos;
	public float maxInterestTotal;
	public float maxInterestIndividual;
	public List<string> lensesInBag;
	public List<string> filtersInBag;
}