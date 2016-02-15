using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BagCamera : MonoBehaviour {

	public int bagSize;
	public List<Item> bagInventory;
	public int[][] inventoryPosistions;

	void Start () {
		bagSize = 8;
		bagInventory = new List<Item> ();
		inventoryPosistions = new int[4][2];
		for (int x = 0; x < inventoryPosistions.Length; x++) {
			for (int y = 0; y < inventoryPosistions [x].Length; y++) {
				inventoryPosistions [x] [y] = -1;
			}
		}
	}
}

public class Item {
	
	int itemSize;
	int itemID;
	GameObject item;

}