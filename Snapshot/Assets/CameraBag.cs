using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BagCamera : MonoBehaviour {

	public int bagSize;
	public List<Item> bagInventory;
	public int[,] inventoryPosistions;

	void Start () {
		bagSize = 8;
		int rows = bagSize / 4;
		int columns = bagSize / rows;
		bagInventory = new List<Item> ();
		inventoryPosistions = new int[rows, columns];
		for (int x = 0; x < rows; x++) {
			for (int y = 0; y < columns; y++) {
				inventoryPosistions [x, y] = -1;
			}
		}
	}
}

public class Item {
	
	int itemSize;
	int itemID;
	GameObject item;

}