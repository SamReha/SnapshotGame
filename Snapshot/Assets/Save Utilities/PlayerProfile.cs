using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PlayerProfile : MonoBehaviour {
	public class Profile {
		public float money;
	}
	public static Profile profile;

	private static string saveFileName;

	// Use this for initialization
	void Start () {
		saveFileName = "/playerprofile.txt";
		profile = new Profile ();
	}

	/*
	 * Tries to open a save file for the current player.
	 * In the future, this may need to consider an player
	 * name so we can have multiple save files.
	 */
	public void loadProfile() {
		if (File.Exists(saveFileName)){
			string stringifiedProfile = File.ReadAllText (saveFileName);
			profile = JsonUtility.FromJson<Profile>(stringifiedProfile);
		} else {
			Debug.Log("Save file does not exist! Creating an empty one...");
			createProfile ();
			saveProfile ();
		}
	}

	/*
	 * Tries to write to a save file for the current player.
	 * In the future, this may need to consider a player
	 * name so we can have multiple save files.
	 */
	public void saveProfile() {
		string stringifiedProfile = JsonUtility.ToJson(profile);
		File.WriteAllText (saveFileName, stringifiedProfile);
	}

	/*
	 * Creates an empty profile
	 */
	private void createProfile() {
		profile.money = 0;
	}
}
