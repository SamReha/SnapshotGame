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
