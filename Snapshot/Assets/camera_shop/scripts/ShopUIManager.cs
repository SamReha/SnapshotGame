using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour {
	public Text moneyText;
	private PlayerProfile playerProfile;

	// Use this for initialization
	void Start () {
		playerProfile = GetComponentInParent<PlayerProfile> ();
		playerProfile.loadProfile ();
		moneyText.text = "$" + playerProfile.profile.money;
	}
	
	// Update is called once per frame
	void Update () {}

	public void giveTenDollars() {
		playerProfile.profile.money += 10;
		playerProfile.saveProfile ();

		moneyText.text = "$" + playerProfile.profile.money;
	}
}
