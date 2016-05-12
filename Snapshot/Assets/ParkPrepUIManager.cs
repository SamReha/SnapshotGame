using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ParkPrepUIManager : MonoBehaviour {
	private int itemCount = 0;
	private float MAX_ITEMS; //for now
	private const int MIN_ITEMS = 0;

	private Text itemCountTxt;
	private Text bagEmpty;
	private Button parkBtn;
	private ToggleGroup toggles;
	private ToggleGroup toggles_b;
	public static AudioSource src;

	private Toggle[] toggles_inventory;
	private Toggle[] toggles_bag;

	private List<string> filters = new List<string>();
	private List<string> lenses = new List<string>();

	void Start () {
		PlayerProfile.profile.load();

		if (!src) {
			src = MainMenuUI.mainMenuSource;
		}

		itemCountTxt = GameObject.FindGameObjectWithTag("itemCountText").GetComponent<Text>();
		bagEmpty = GameObject.FindGameObjectWithTag ("empty text").GetComponent<Text>();
		itemCountTxt.text = "Total Items: " + itemCount + "/" + MAX_ITEMS;
		parkBtn = GameObject.FindGameObjectWithTag("parkbtn").GetComponent<Button>();
		toggles = GameObject.FindGameObjectWithTag ("toggles_inventory").GetComponent<ToggleGroup>();
		toggles_b = GameObject.FindGameObjectWithTag ("toggles_bag").GetComponent<ToggleGroup>();
		toggles_inventory = toggles.GetComponentsInChildren<Toggle>();
		toggles_bag = toggles_b.GetComponentsInChildren<Toggle>();
		MAX_ITEMS = PlayerProfile.profile.bagSize;

		int numItems = PlayerProfile.profile.lenses.Count + PlayerProfile.profile.filters.Count;

		for (int j = 0; j < toggles_bag.Length; j++) {
			toggles_bag[j].gameObject.SetActive (false);
			toggles_bag[j].GetComponentInChildren<Text>().fontSize = 12;
		}
			

		//make it easier to initalize everything
		for (int k = 0; k < toggles_inventory.Length; k++) {
			toggles_inventory [k].interactable = false;
			toggles_inventory [k].isOn = false;
			toggles_inventory [k].gameObject.SetActive (false);
		}

		for (int ii = 0; ii < numItems; ii++) {
			toggles_inventory [ii].gameObject.SetActive (true);
			toggles_inventory [ii].isOn = false;
			toggles_inventory [ii].interactable = true;
			toggles_inventory [ii].GetComponentInChildren<Text> ().fontSize = 12;
		}

		PlayerProfile.profile.lensesInBag.Clear ();
		PlayerProfile.profile.filtersInBag.Clear ();

		initItemNames ();
	}

	// Update is called once per frame
	void Update () {
		itemCountTxt.text = "Total Items: " + itemCount + "/" + MAX_ITEMS;

		if (itemCount > MAX_ITEMS) {
			itemCountTxt.color = Color.red; //signal the player they are over the bag space
			parkBtn.interactable = false; //disable the button
		} else {
			itemCountTxt.color = Color.black; // the player has at most 5 items 
			parkBtn.interactable = true; //reeable button
		}
		bagEmpty = bagEmpty;
	}

	//test function
	public void addToItemsCount() {
		itemCount++;
	}

	public void move_items() {
		for (int i = 0; i < toggles_inventory.Length; i++) {
			if (toggles_inventory [i].isOn) {
				itemCount++;

				toggles_inventory [i].interactable = false;
				toggles_inventory [i].isOn = false;
				toggles_inventory [i].gameObject.SetActive (false);

				toggles_bag [i].gameObject.SetActive (true);
				toggles_bag [i].isOn = false;
				toggles_bag [i].interactable = true;
			}
		}
		bagEmpty.gameObject.SetActive(false);


	}

	public void move_items_bag() {
		for (int i = 0; i < toggles_bag.Length; i++) {
			if (toggles_bag [i].isOn) {
				itemCount--;

				toggles_bag [i].interactable = false;
				toggles_bag [i].isOn = false;
				toggles_bag [i].gameObject.SetActive (false);

				toggles_inventory [i].gameObject.SetActive (true);
				toggles_inventory [i].isOn = false;
				toggles_inventory [i].interactable = true;
			}
		}
		if (itemCount == 0) {
			bagEmpty.gameObject.SetActive (true);
		}
	}

	public void toMainMenu() {
		MainMenuUI.prepMenu = false;
		SceneManager.LoadScene ("main_menu");
	}

	public void toPark() {
		foreach (string s in PlayerProfile.profile.filters) {
			Debug.Log ("Filters: " + s);
		}
		foreach (string s in PlayerProfile.profile.lenses) {
			Debug.Log ("Lenses: " + s);
		}

		foreach (Toggle t in toggles_bag) {
			if (t.IsActive ()) {
				string s = t.GetComponentInChildren<Text> ().text;
				Debug.Log (s);
				s = convertName (s);
				Debug.Log (s + " Lense: " + PlayerProfile.profile.lenses.Contains (s) + " Filter: " + PlayerProfile.profile.filters.Contains (s));
				if (PlayerProfile.profile.lenses.Contains (s)) {
					lenses.Add (s);
				} else if (PlayerProfile.profile.filters.Contains (s)) {
					filters.Add (s);
				}
			}
		}

		PlayerProfile.profile.filtersInBag = filters;
		PlayerProfile.profile.lensesInBag = lenses;
		foreach (string s in PlayerProfile.profile.filtersInBag) {
			Debug.Log ("Filters in bag: " + s);
		}
		foreach (string s in PlayerProfile.profile.lensesInBag) {
			Debug.Log ("Lenses in bag: " + s);
		}
		PlayerProfile.profile.save ();
		MainMenuUI.mainMenuSource.Stop ();
		src.Stop ();
		MainMenuUI.prepMenu = false;
		SceneManager.LoadScene ("SSV0.0");
	}

	public string convertName (string name){
		switch (name) {
		case "Basic Portrait Lens":
			Debug.Log (name + " Returning port1");
			return "port1";
		case "Basic Wide Angle Lens":
			Debug.Log (name + " Returning wide1");
			return "wide1";
		case "Basic Telephoto Lens":
			Debug.Log (name + " Returning tele1");
			return "tele1";
		case "Basic Clear Filter":
			Debug.Log (name + " Returning clear");
			return "clear";
		case "Blue Filter":
			Debug.Log (name + " Returning bluefilter");
			return "bluefilter";
		case "Orange Filter":
			Debug.Log (name + " Returning orangefilter");
			return "orangefilter";
		case "Blue Orange Filter":
			Debug.Log (name + " Returning blueorangefilter");
			return "blueorangefilter";
		case "Faded Blue Filter":
			Debug.Log (name + " Returning bluefadefilter");
			return "bluefadefilter";
		case "Faded Orange Filter":
			Debug.Log (name + " Returning orangefadefilter");
			return "orangefadefilter";
		case "Rainbow Filter":
			Debug.Log (name + " Returning rainbowfilter");
			return "rainbowfilter";
		default:
			Debug.Log (name + " Returning null");
			return "";
	}
	}

	public void initItemNames() {
		List<string> everything = new List<string> ();
		foreach (string s in PlayerProfile.profile.lenses) {
			everything.Add (s);
		}

		for (int i = 0; i < PlayerProfile.profile.filters.Count; i++) {
			everything.Add (PlayerProfile.profile.filters [i]);
		}
		for ( int i = 0; i < everything.Count; i++) {
			if (everything [i].Equals ("port1")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Basic Portrait Lens";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (everything [i].Equals ("wide1")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Basic Wide Angle Lens";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (everything [i].Equals ("tele1")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Basic Telephoto Lens";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (everything [i].Equals ("clear")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Basic Clear Filter";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (everything [i].Equals ("bluefilter")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Blue Filter";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (everything [i].Equals ("orangefilter")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Orange Filter";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (everything [i].Equals ("blueorangefilter")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Blue Orange Filter";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (everything [i].Equals ("bluefadefilter")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Faded Blue Filter";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (everything [i].Equals ("orangefadefilter")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Faded Orange Filter";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (everything [i].Equals ("rainbowfilter")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Rainbow Filter";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			}
		}

	//	foreach (string filter in PlayerProfile.profile.filters) {


	}
}
