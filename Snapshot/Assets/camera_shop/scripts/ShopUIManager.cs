using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ShopUIManager : MonoBehaviour {
	private AudioSource shopSource;
	public Text moneyText;

	//private Dictionary<string,PurchaseButton> purchaseButtons; // Ugh I am not clever enough for this.
	public Button wideButton;
	float widePrice = 250f;
	string wideName = "wide1";

	public Button portButton;
	float portPrice = 100f;
	string portName = "port1";

	public Button teleButton;
	float telePrice = 400f;
	string teleName = "tele1";

	public Button filterB;
	float fBPrice = 100f;
	string fBName = "fb";

	public Button filterO;
	float fOPrice = 100f;
	string fOName = "fo";

	public Button filterBO;
	float fBOPrice = 100f;
	string fBOName = "fbo";

	public Button filterBC;
	float fBCPrice = 100f;
	string fBCName = "fbc";

	public Button filterOC;
	float fOCPrice = 100f;
	string fOCName = "foc";

	public Button filterR;
	float fRPrice = 100f;
	string fRName = "fr";

	public Button bagFive;
	float bFPrice = 100f;

	public Button bagTen;
	float bTPrice = 1000f;

	// Use this for initialization
	void Start () {
		PlayerProfile.profile.load ();
		shopSource = GetComponent<AudioSource> ();

		shopSource.ignoreListenerPause = true;
		shopSource.Play ();
		//Panel panel = GameObject.Find ("/CanvasShop/PanelShop");
		/*
		purchaseButtons = new Dictionary<string,PurchaseButton> ();
		purchaseButtons ["tele1"] = new PurchaseButton ();
		purchaseButtons ["tele1"].price = 399.99f;
		purchaseButtons ["tele1"].name = "tele1";
		purchaseButtons ["tele1"].btn = UnityEngine.Resources.Load<Button>("UI/Button");
		purchaseButtons ["tele1"].btn.onClick.AddListener(
			delegate() {
				buyLens(purchaseButtons ["tele1"].price, purchaseButtons ["tele1"].name);
			}
		);
		purchaseButtons ["tele1"].btn.transform.parent = panel.transform;

		purchaseButtons ["port1"] = new PurchaseButton ();
		purchaseButtons ["port1"].price = 99.99f;
		purchaseButtons ["port1"].name = "port1";
		purchaseButtons ["port1"].btn = UnityEngine.Resources.Load<Button>("UI/Button");
		purchaseButtons ["port1"].btn.onClick.AddListener(
			delegate() {
				buyLens(purchaseButtons ["port1"].price, purchaseButtons ["port1"].name);
			}
		);
		purchaseButtons ["port1"].btn.transform.parent = panel.transform;

		purchaseButtons ["wide1"] = new PurchaseButton ();
		purchaseButtons ["wide1"].price = 299.99f;
		purchaseButtons ["wide1"].name = "wide1";
		purchaseButtons ["wide1"].btn = UnityEngine.Resources.Load<Button>("UI/Button");
		purchaseButtons ["wide1"].btn.onClick.AddListener(
			delegate() {
				buyLens(purchaseButtons ["wide1"].price, purchaseButtons ["wide1"].name);
			}
		);
		purchaseButtons ["wide1"].btn.transform.parent = panel.transform;*/
	}
	
	// Update is called once per frame
	void Update () {
		if (PlayerProfile.profile.lenses.Contains (wideName)) {
			wideButton.GetComponentInChildren<Text> ().text = "Already Owned";
			wideButton.interactable = false;
		} else if (PlayerProfile.profile.money < widePrice) {
			wideButton.interactable = false;
		} else {
			wideButton.interactable = true;
		}

		if (PlayerProfile.profile.lenses.Contains (teleName)) {
			teleButton.GetComponentInChildren<Text> ().text = "Already Owned";
			teleButton.interactable = false;
		} else if (PlayerProfile.profile.money < telePrice) {
			teleButton.interactable = false;
		} else {
			teleButton.interactable = true;
		}

		if (PlayerProfile.profile.lenses.Contains (portName)) {
			portButton.GetComponentInChildren<Text> ().text = "Already Owned";
			portButton.interactable = false;
		} else if (PlayerProfile.profile.money < portPrice) {
			portButton.interactable = false;
		} else {
			portButton.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fBName)) {
			filterB.GetComponentInChildren<Text> ().text = "Already Owned";
			filterB.interactable = false;
		} else if (PlayerProfile.profile.money < fBPrice) {
			filterB.interactable = false;
		} else {
			filterB.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fOName)) {
			filterO.GetComponentInChildren<Text> ().text = "Already Owned";
			filterO.interactable = false;
		} else if (PlayerProfile.profile.money < fOPrice) {
			filterO.interactable = false;
		} else {
			filterO.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fBOName)) {
			filterBO.GetComponentInChildren<Text> ().text = "Already Owned";
			filterBO.interactable = false;
		} else if (PlayerProfile.profile.money < fBOPrice) {
			filterBO.interactable = false;
		} else {
			filterBO.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fBCName)) {
			filterBC.GetComponentInChildren<Text> ().text = "Already Owned";
			filterBC.interactable = false;
		} else if (PlayerProfile.profile.money < fBCPrice) {
			filterBC.interactable = false;
		} else {
			filterBC.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fOCName)) {
			filterOC.GetComponentInChildren<Text> ().text = "Already Owned";
			filterOC.interactable = false;
		} else if (PlayerProfile.profile.money < fOCPrice) {
			filterOC.interactable = false;
		} else {
			filterOC.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fRName)) {
			filterR.GetComponentInChildren<Text> ().text = "Already Owned";
			filterR.interactable = false;
		} else if (PlayerProfile.profile.money < fRPrice) {
			filterR.interactable = false;
		} else {
			filterR.interactable = true;
		}

		if (PlayerProfile.profile.bagSize == 5) {
			bagFive.GetComponentInChildren<Text> ().text = "Already Owned";
			bagFive.interactable = false;
		} else if (PlayerProfile.profile.money < bFPrice) {
			bagFive.interactable = false;
		} else {
			bagFive.interactable = true;
		}

		if (PlayerProfile.profile.bagSize == 10) {
			bagTen.GetComponentInChildren<Text> ().text = "Already Owned";
			bagTen.interactable = false;
		} else if (PlayerProfile.profile.money < bTPrice) {
			bagTen.interactable = false;
		} else {
			bagTen.interactable = true;
		}

		moneyText.text = "$" + PlayerProfile.profile.money;

		/*foreach (KeyValuePair<string, PurchaseButton> entry in purchaseButtons) {
			if (PlayerProfile.profile.lenses.Contains (entry.Key)
				|| PlayerProfile.profile.money < entry.Value.price) {
				purchaseButtons [entry.Key].btn.interactable = false;
			}
		}*/
	}

	void Awake() {}

	public void giveTenDollars() {
		PlayerProfile.profile.money += 10;
		PlayerProfile.profile.save ();
	}

	public void buyLens(float price, string name) {
        if (PlayerProfile.profile.money >= price
            && !PlayerProfile.profile.lenses.Contains(name)) {
            PlayerProfile.profile.lenses.Add(name);
            PlayerProfile.profile.money -= price;
            PlayerProfile.profile.save();
        }
	}
		
	public void buyFilter(float price, string name){
		if (PlayerProfile.profile.money >= price
			&& !PlayerProfile.profile.filters.Contains(name)) {
			PlayerProfile.profile.filters.Add(name);
			PlayerProfile.profile.money -= price;
			PlayerProfile.profile.save();
		}
	}

	public void buyBag(float size, float price){
		if (PlayerProfile.profile.money >= price 
			&& PlayerProfile.profile.bagSize < size) {
			PlayerProfile.profile.bagSize = size;
			PlayerProfile.profile.money -= price;
			PlayerProfile.profile.save();
		}
	}

	public void buyWideLens() {
		// Assume lens is not owned and player has enough money
		buyLens(widePrice, wideName);
	}

	public void buyPortLens() {
		// Assume lens is not owned and player has enough money
		buyLens(portPrice, portName);
	}

	public void buyTeleLens() {
		// Assume lens is not owned and player has enough money
		buyLens(telePrice, teleName);
	}

	public void buyFilterB(){
		buyFilter (fBPrice, fBName);
	}

	public void buyFilterO(){
		buyFilter (fOPrice, fOName);
	}

	public void buyFilterBO(){
		buyFilter (fBOPrice, fBOName);
	}

	public void buyFilterBC(){
		buyFilter (fBCPrice, fBCName);
	}

	public void buyFilterOC(){
		buyFilter (fOCPrice, fOCName);
	}

	public void buyFilterR(){
		buyFilter (fRPrice, fRName);
	}

	public void buyBagFive(){
		buyBag (5, bFPrice);
	}

	public void buyBagTen(){
		buyBag (10, bTPrice);
	}

	public void loadMainMenu() {
		shopSource.Stop ();
		SceneManager.LoadScene ("main_menu");
	}
}

/*class PurchaseButton {
	public float price;
	public string name;
	public Button btn;
}*/
