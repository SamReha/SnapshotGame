using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class EquipmentManager : MonoBehaviour {
    [Serializable]
    public class SerializeableMemCardData {
        public string name;
        public string description;
        public float cost;
        public uint capacity;
        public bool owned;
    }

    public class MemCard {
        public SerializeableMemCardData data;

        public MemCard(string name, string description, float cost, uint capacity) {
            data = new SerializeableMemCardData();
            data.name = name;
            data.description = description;
            data.cost = cost;
            data.capacity = capacity;
            data.owned = false;
        }

        public MemCard(SerializeableMemCardData memCardData)  {
            data = memCardData;
        }
    }

    public List<MemCard> memCards;
    public bool equipmentLoaded;

    private bool memCardsLoaded;
    private string savePath;
    private string memCardPath;

    void Awake() {
        savePath = Application.persistentDataPath + "/equipment/";
        memCardPath = "memCards/";

        memCards = new List<MemCard>();
    }

    void Start () {
        loadEquipment();
        ValidateEquipment();
    }

    // Loads equipment data from disk (Also ensures that equipment data exists on disk to begin with)
    public void loadEquipment() {
        DirectoryInfo equipmentDir = new DirectoryInfo(savePath);

        if (!equipmentDir.Exists) {
            equipmentDir.Create();
        }

        if (!equipmentLoaded) {
            loadMemCards();
        }
    }

    private void loadMemCards() {
        DirectoryInfo memCardDir = new DirectoryInfo(savePath + memCardPath);

        if (!memCardDir.Exists) {
            memCardDir.Create();
        }

        FileInfo[] info = memCardDir.GetFiles("*.memcard");
        BinaryFormatter binForm = new BinaryFormatter();
        string fullMemCardPath;

        // If info.Length is 0, then there is no equipment data saved on disk, so we should set some up!
        if (info.Length == 0) {
            initializeMemCards();
            saveEquipment();
        }
        else if (!memCardsLoaded) {   // Make sure we don't attempt to load the memCards twice
            foreach (FileInfo file in info) {
                fullMemCardPath = savePath + file.Name;
                FileStream memCardFile = File.Open(fullMemCardPath, FileMode.Open);

                SerializeableMemCardData memCardData = (SerializeableMemCardData)binForm.Deserialize(memCardFile);
                MemCard memCard = new MemCard(memCardData);

                memCardFile.Close();
                memCards.Add(memCard);
            }
            memCardsLoaded = true;
        }
    }

    private void initializeMemCards() {
        memCards.Add(new MemCard(
            "8-Slot Card",
            "A basic memory card that only holds 8 photos. How boring.",
            0f,
            8
        ));

        memCards.Add(new MemCard(
            "16-Slot Card",
            "A decent memory card that holds 16 photos. Not bad!",
            100f,
            16
        ));

        memCards.Add(new MemCard(
            "32-Slot Card",
            "A high-end memory card that holds up to 32 photos. Nice!",
            500f,
            32
        ));
    }

    // Make sure some assumptions about achievement data setup are followed.
    private void ValidateEquipment() {
        ArrayList usedNames = new ArrayList();
        foreach (MemCard memCard in memCards) {
            if (usedNames.Contains(memCard.data.name)) {
                Debug.LogError("EquipmentManager::ValidateEquipment() - Duplicate MemoryCard names! " + memCard.data.name + " found more than once!");
            }
            usedNames.Add(memCard.data.name);
        }
    }


    // Update is called once per frame
    void Update () {
	
	}
}
