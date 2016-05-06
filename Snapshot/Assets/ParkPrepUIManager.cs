using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ParkPrepUIManager : MonoBehaviour {
	private int itemCount = 0;
	private const int MAX_ITEMS = 5; //for now
	private const int MIN_ITEMS = 0;

	private Text itemCountTxt;
	private Text bagEmpty;
	private Button parkBtn;
	private ToggleGroup toggles;
	private ToggleGroup toggles_b;
	public static AudioSource src;

	private Toggle[] toggles_inventory;
	private Toggle[] toggles_bag;
	string blah = "blah";

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

		for (int ii = 0; ii < PlayerProfile.profile.lenses.Count; ii++) {
			toggles_inventory [ii].gameObject.SetActive (true);
			toggles_inventory [ii].isOn = false;
			toggles_inventory [ii].interactable = true;
			toggles_inventory [ii].GetComponentInChildren<Text> ().fontSize = 12;
		}
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
		MainMenuUI.mainMenuSource.Stop ();
		src.Stop ();
		MainMenuUI.prepMenu = false;
		SceneManager.LoadScene ("SSV0.0");
	}

	public void initItemNames() {
		for (int i = 0; i < PlayerProfile.profile.lenses.Count; i++) {
			if (PlayerProfile.profile.lenses [i].Equals ("port1")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Basic Portrait Lens";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (PlayerProfile.profile.lenses [i].Equals ("wide1")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Basic Wide Angle Lens";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			} else if (PlayerProfile.profile.lenses [i].Equals ("tele1")) {
				toggles_inventory [i].GetComponentInChildren<Text> ().text = "Basic Telephoto Lens";
				toggles_bag [i].GetComponentInChildren<Text> ().text = toggles_inventory [i].GetComponentInChildren<Text> ().text;
			}
		}
	}
}
