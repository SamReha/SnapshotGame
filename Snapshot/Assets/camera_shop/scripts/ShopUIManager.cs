using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ShopUIManager : MonoBehaviour {
	private AudioSource shopSource;
	public Text moneyText;
    public EquipmentManager equipManager;

	public Button wideButtonL;
	public Button wideButtonH;
	float widePrice = 250f;
	string wideName = "wide1";

	public Button portButtonL;
	public Button portButtonH;
	float portPrice = 100f;
	string portName = "port1";

	public Button teleButtonL;
	public Button teleButtonH;
	float telePrice = 400f;
	string teleName = "tele1";

	public Button filterBL;
	public Button filterBH;
	float fBPrice = 100f;
	string fBName = "bluefilter";

	public Button filterOL;
	public Button filterOH;
	float fOPrice = 100f;
	string fOName = "orangefilter";

	public Button filterBOL;
	public Button filterBOH;
	float fBOPrice = 100f;
	string fBOName = "blueorangefilter";

	public Button filterBCL;
	public Button filterBCH;
	float fBCPrice = 100f;
	string fBCName = "bluefadefilter";

	public Button filterOCL;
	public Button filterOCH;
	float fOCPrice = 100f;
	string fOCName = "orangefadefilter";

	public Button filterRL;
	public Button filterRH;
	float fRPrice = 100f;
	string fRName = "rainbowfilter";

	public Button bagFiveL;
	public Button bagFiveH;
	float bFPrice = 100f;

	public Button bagTenL;
	public Button bagTenH;
	float bTPrice = 1000f;

	public List<Button> memoryCardButtonsSortH = new List<Button> ();
	public List<Button> memoryCardButtonsSortL = new List<Button> ();
	public List<Button> filterButtons = new List<Button> ();
	public List<Button> lensButtons = new List<Button> ();
	public List<Button> bagButtons = new List<Button> ();

	public List<Button> specialButtons = new List<Button> ();

	// Use this for initialization
	void Start () {
		PlayerProfile.profile.load ();
        equipManager.loadEquipment();

        configureMemoryCardButtons();

		shopSource = GetComponent<AudioSource> ();

		shopSource.ignoreListenerPause = true;
		shopSource.Play ();

		foreach (Button b in specialButtons) {
			int x = Random.Range (0, 4);
			int y;
			float price;
			string name;
			switch(x)
			{
			case 0:
				y = Random.Range (0, memoryCardButtonsSortL.Count);
				b.GetComponentInChildren<Text> ().text = memoryCardButtonsSortL [y].GetComponentInChildren<Text> ().text;
				if (b.GetComponentInChildren<Text> ().text.Contains ("Already Owned")) {
					b.interactable = false;
				} else if (PlayerProfile.profile.money < checkPrice (b.GetComponentInChildren<Text> ().text)) {
					b.interactable = false;
				} else {
					uint i = checkMemory (b.GetComponentInChildren<Text> ().text);
					switch (i) {
					case 8:
						i = 0;
						break;
					case 16:
						i = 1;
						break;
					case 32:
						i = 2;
						break;
					}
					b.onClick.AddListener (() => {
						buyMemoryCard (equipManager.memCards [(int)i]);
					});
				}
				break;
			case 1:
				y = Random.Range (0, filterButtons.Count);
				b.GetComponentInChildren<Text> ().text = filterButtons [y].GetComponentInChildren<Text> ().text;
				price = checkPrice (b.GetComponentInChildren<Text> ().text);
				name = convertName (b.GetComponentInChildren<Text> ().text);
				Debug.Log (b.GetComponentInChildren<Text> ().text + " , " + name + " , " + price);
				if (b.GetComponentInChildren<Text> ().text.Contains ("Already Owned") || PlayerProfile.profile.filters.Contains (name)) {
					b.GetComponentInChildren<Text> ().text = b.GetComponentInChildren<Text> ().text.Substring (0, b.GetComponentInChildren<Text> ().text.IndexOf ("$")) + "Already Owned"; 

					b.interactable = false;
				} else if (PlayerProfile.profile.money < price) {
					b.interactable = false;
				} else {
					b.onClick.AddListener (() => {
						buyFilter (price, name);
					});
				}

				break;
			case 2:
				y = Random.Range (0, lensButtons.Count);
				b.GetComponentInChildren<Text> ().text = lensButtons [y].GetComponentInChildren<Text> ().text;
				price = checkPrice (b.GetComponentInChildren<Text> ().text);
				name = convertName (b.GetComponentInChildren<Text> ().text);
				Debug.Log(name +  " , " + price);
				if (b.GetComponentInChildren<Text> ().text.Contains ("Already Owned") || PlayerProfile.profile.lenses.Contains (name)) {
					b.GetComponentInChildren<Text> ().text = b.GetComponentInChildren<Text> ().text.Substring (0, b.GetComponentInChildren<Text> ().text.IndexOf ("$")) + "Already Owned"; 
					b.interactable = false;
				} else if (PlayerProfile.profile.money < price) {
					b.interactable = false;
				}
				b.onClick.AddListener(() => {buyLens(price , name);});
				break;
			case 3:
				y = Random.Range (0, bagButtons.Count);
				b.GetComponentInChildren<Text> ().text = bagButtons [y].GetComponentInChildren<Text> ().text;
				price = checkPrice (b.GetComponentInChildren<Text> ().text);
				name = convertName (b.GetComponentInChildren<Text> ().text);
				float xyz = float.Parse (name);
				if (PlayerProfile.profile.bagSize == xyz) {
					b.GetComponentInChildren<Text> ().text = b.GetComponentInChildren<Text> ().text.Substring (0, b.GetComponentInChildren<Text> ().text.IndexOf ("$")) + "Already Owned"; 
					b.interactable = false;
				} else if (PlayerProfile.profile.money < price) {
					b.interactable = false;
				} else {
					b.onClick.AddListener (() => {
						buyBag (xyz, price);
					});
				}
				break;
			}

		}
	}
	
	// Update is called once per frame
	void Update () {
		if (PlayerProfile.profile.lenses.Contains (wideName)) {
			wideButtonL.GetComponentInChildren<Text> ().text = "Wide Angle Lens Already Owned";
			wideButtonL.interactable = false;
			wideButtonH.GetComponentInChildren<Text> ().text = "Wide Angle Lens Already Owned";
			wideButtonH.interactable = false;
		} else if (PlayerProfile.profile.money < widePrice) {
			wideButtonL.interactable = false;
			wideButtonH.interactable = false;
		} else {
			wideButtonL.interactable = true;
			wideButtonH.interactable = true;
		}

		if (PlayerProfile.profile.lenses.Contains (teleName)) {
			teleButtonL.GetComponentInChildren<Text> ().text = "telephoto Lens Already Owned";
			teleButtonL.interactable = false;
			teleButtonH.GetComponentInChildren<Text> ().text = "telephoto Lens Already Owned";
			teleButtonH.interactable = false;
		} else if (PlayerProfile.profile.money < telePrice) {
			teleButtonL.interactable = false;
			teleButtonH.interactable = false;
		} else {
			teleButtonL.interactable = true;
			teleButtonH.interactable = true;
		}

		if (PlayerProfile.profile.lenses.Contains (portName)) {
			portButtonL.GetComponentInChildren<Text> ().text = "Portrait Lens Already Owned";
			portButtonL.interactable = false;
			portButtonH.GetComponentInChildren<Text> ().text = "Portrait Lens Already Owned";
			portButtonH.interactable = false;
		} else if (PlayerProfile.profile.money < portPrice) {
			portButtonL.interactable = false;
			portButtonH.interactable = false;
		} else {
			portButtonL.interactable = true;
			portButtonH.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fBName)) {
			filterBL.GetComponentInChildren<Text> ().text = "Blue Filter Already Owned";
			filterBL.interactable = false;
			filterBH.GetComponentInChildren<Text> ().text = "Blue Filter Already Owned";
			filterBH.interactable = false;
		} else if (PlayerProfile.profile.money < fBPrice) {
			filterBL.interactable = false;
			filterBH.interactable = false;
		} else {
			filterBL.interactable = true;
			filterBH.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fOName)) {
			filterOL.GetComponentInChildren<Text> ().text = "Orange Filter Already Owned";
			filterOL.interactable = false;
			filterOH.GetComponentInChildren<Text> ().text = "Orange Filter Already Owned";
			filterOH.interactable = false;
		} else if (PlayerProfile.profile.money < fOPrice) {
			filterOL.interactable = false;
			filterOH.interactable = false;
		} else {
			filterOL.interactable = true;
			filterOH.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fBOName)) {
			filterBOL.GetComponentInChildren<Text> ().text = "Blue Orange Filter Already Owned";
			filterBOL.interactable = false;
			filterBOH.GetComponentInChildren<Text> ().text = "Blue Orange Filter Already Owned";
			filterBOH.interactable = false;
		} else if (PlayerProfile.profile.money < fBOPrice) {
			filterBOL.interactable = false;
			filterBOH.interactable = false;
		} else {
			filterBOL.interactable = true;
			filterBOH.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fBCName)) {
			filterBCL.GetComponentInChildren<Text> ().text = "Blue Clear Filter Already Owned";
			filterBCL.interactable = false;
			filterBCH.GetComponentInChildren<Text> ().text = "Blue Clear Filter Already Owned";
			filterBCH.interactable = false;
		} else if (PlayerProfile.profile.money < fBCPrice) {
			filterBCL.interactable = false;
			filterBCH.interactable = false;
		} else {
			filterBCL.interactable = true;
			filterBCH.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fOCName)) {
			filterOCL.GetComponentInChildren<Text> ().text = "Orange Clear Filter Already Owned";
			filterOCL.interactable = false;
			filterOCH.GetComponentInChildren<Text> ().text = "Orange Clear Filter Already Owned";
			filterOCH.interactable = false;
		} else if (PlayerProfile.profile.money < fOCPrice) {
			filterOCL.interactable = false;
			filterOCH.interactable = false;
		} else {
			filterOCL.interactable = true;
			filterOCH.interactable = true;
		}

		if (PlayerProfile.profile.filters.Contains (fRName)) {
			filterRL.GetComponentInChildren<Text> ().text = "Rainbow Filter Already Owned";
			filterRL.interactable = false;
			filterRH.GetComponentInChildren<Text> ().text = "Rainbow Filter Already Owned";
			filterRH.interactable = false;
		} else if (PlayerProfile.profile.money < fRPrice) {
			filterRL.interactable = false;
			filterRH.interactable = false;
		} else {
			filterRL.interactable = true;
			filterRH.interactable = true;
		}

		if (PlayerProfile.profile.bagSize == 5) {
			bagFiveL.GetComponentInChildren<Text> ().text = "Small Bag Already Owned";
			bagFiveL.interactable = false;
			bagFiveH.GetComponentInChildren<Text> ().text = "Small Bag Already Owned";
			bagFiveH.interactable = false;
		} else if (PlayerProfile.profile.money < bFPrice) {
			bagFiveL.interactable = false;
			bagFiveH.interactable = false;
		} else {
			bagFiveL.interactable = true;
			bagFiveH.interactable = true;
		}

		if (PlayerProfile.profile.bagSize == 10) {
			bagTenL.GetComponentInChildren<Text> ().text = "Large Bag Already Owned";
			bagTenL.interactable = false;
			bagTenH.GetComponentInChildren<Text> ().text = "Large Bag Already Owned";
			bagTenH.interactable = false;
		} else if (PlayerProfile.profile.money < bTPrice) {
			bagTenL.interactable = false;
			bagTenH.interactable = false;
		} else {
			bagTenL.interactable = true;
			bagTenH.interactable = true;
		}

		moneyText.text = "$" + PlayerProfile.profile.money;
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

    private void configureMemoryCardButtons() {
		for (int i = 0; i < memoryCardButtonsSortL.Count; i++) {
			Button button = memoryCardButtonsSortL [i];
			EquipmentManager.Equipment<EquipmentManager.MemCardData> memoryCard = equipManager.memCards [i];

			if (memoryCard.data.owned) {
				button.GetComponentInChildren<Text> ().text = memoryCard.data.name + "\nAlready Owned";
				button.interactable = false;
			} else {
				float cost = memoryCard.data.cost;
				button.GetComponentInChildren<Text> ().text = memoryCard.data.name + "\n$" + cost;

				if (cost > PlayerProfile.profile.money) {
					button.interactable = false;
				}

				button.onClick.AddListener (() => {buyMemoryCard (memoryCard);});
			}

		}
		for (int i = 0; i < memoryCardButtonsSortH.Count; i++) {
			Button button = memoryCardButtonsSortH [i];
			EquipmentManager.Equipment<EquipmentManager.MemCardData> memoryCard = equipManager.memCards [i];

			if (memoryCard.data.owned) {
				button.GetComponentInChildren<Text> ().text = memoryCard.data.name + "\nAlready Owned";
				button.interactable = false;
			} else {
				float cost = memoryCard.data.cost;
				button.GetComponentInChildren<Text> ().text = memoryCard.data.name + "\n$" + cost;

				if (cost > PlayerProfile.profile.money) {
					button.interactable = false;
				}

				button.onClick.AddListener (() => {buyMemoryCard (memoryCard);});
			}

		}
    }
		
	private void buyMemoryCard(EquipmentManager.Equipment<EquipmentManager.MemCardData> memCard) {
		// If it's not already owned and the player has enough money
		if (memCard.data.owned == false && PlayerProfile.profile.money >= memCard.data.cost) {
			PlayerProfile.profile.money -= memCard.data.cost;
			memCard.data.owned = true;

			if (PlayerProfile.profile.memoryCardCapacity < memCard.data.capacity) {
				PlayerProfile.profile.memoryCardCapacity = memCard.data.capacity;
			}

			PlayerProfile.profile.save ();
			equipManager.saveEquipment ();

			// Refresh the buttons to ensure they look correct after purchase
			configureMemoryCardButtons ();
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

		if (ParkPrepUIManager.src != null) {
			ParkPrepUIManager.src.Play();
		}
	}

	public float checkPrice(string item){
		int x = item.IndexOf ("$");
		float price = float.Parse (item.Substring (x+1));
		return price;
	}

	public uint checkMemory (string mem){
		uint x = uint.Parse (mem.Substring (0, 1));
		switch (x) {
		case 8:
			return x;
		case 3:
			return x = 32;
		case 1:
			return x = 16;
		default:
			return x = 0;
		}
	}

	public string convertName (string name){
		switch (name) {
		case "Portrait Lens\n$100":
			return "port1";
		case "Wide Angle Lens\n$250":
			return "wide1";
		case "Telephoto Lens\n$450":
			return "tele1";
		case "Blue\n$100":
			return "bluefilter";
		case "Orange\n$100":
			return "orangefilter";
		case "Blue Orange Gradient\n$100":
			return "blueorangefilter";
		case "Blue Clear Gradient\n$100":
			return "bluefadefilter";
		case "Orange Clear Gradient\n$100":
			return "orangefadefilter";
		case "Rainbow\n$100":
			return "rainbowfilter";
		case "Small Bag\n$100":
			return "5";
		case "Large Bag\n$1000":
			return "10";
		case "Portrait Lens\nAlready Owned":
			return "port1";
		case "Wide Angle Lens\nAlready Owned":
			return "wide1";
		case "Telephoto Lens\nAlready Owned":
			return "tele1";
		case "Blue\nAlready Owned":
			return "bluefilter";
		case "Orange\nAlready Owned":
			return "orangefilter";
		case "Blue Orange Gradient\nAlready Owned":
			return "blueorangefilter";
		case "Blue Clear Gradient\nAlready Owned":
			return "bluefadefilter";
		case "Orange Clear Gradient\nAlready Owned":
			return "orangefadefilter";
		case "Rainbow\nAlready Owned":
			return "rainbowfilter";
		case "Small Bag\nAlready Owned":
			return "5";
		case "Large Bag\nAlready Owned":
			return "10";
		default:
			return "";
		}
	}

	public void specialButton(Button b){
		b.GetComponentInChildren<Text>().text = b.GetComponentInChildren<Text> ().text.Substring (0, b.GetComponentInChildren<Text> ().text.IndexOf ("$")) + "Already Owned";
		b.interactable = false;
	}
}


/*class PurchaseButton {
	public float price;
	public string name;
	public Button btn;
}*/
