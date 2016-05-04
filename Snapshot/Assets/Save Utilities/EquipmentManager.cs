using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class EquipmentManager : MonoBehaviour {
    // If I was REALLY clever, I'd implement a class hierarchy to make adding new equipment types easier...
    [Serializable]
    public class MemCardData {
        public string name;
        public string description;
        public float cost;
        public uint capacity;
        public bool owned;
    }

    // Why bother with clunky wrappers? It allows us to have our Equipment objects hold non-serializeable data (like how Achievements do)
    public class Equipment<TSerializeableType> {
        public TSerializeableType data;
    }

    public List<Equipment<MemCardData>> memCards;
    public bool equipmentLoaded;

    private bool memCardsLoaded;
    private string savePath;
    private string memCardPath;

    void Awake() {
        savePath = Application.persistentDataPath + "/equipment/";
        memCardPath = "memCards/";

        memCards = new List<Equipment<MemCardData>>();
    }

    void Start () {
        loadEquipment();
        ValidateEquipment();
    }

    // Loads equipment data from disk (Also ensures that equipment data exists on disk to begin with)
    public void loadEquipment() {
        if (!memCardsLoaded) {
            try {
                memCards = loadEquipmentType<MemCardData>(memCardPath, ".memcard");
            }
            catch (InvalidOperationException e) {
                // If we got an exception, it means that memCards were not intialized
                Debug.Log(e.Message);
                initializeMemCards();
            }
            memCardsLoaded = true;
        }
    }

    /*
     * TSerializeableType: The type of the serializable class associated with the type of Equipment you want to load (ie, MemCardData for Memory Cards)
     * dirName: The (partial) directory path to where the equip files should be located (ie, "memcards/" if the files are in Application.PersisitentDataPath/equipment/memcards/)
     * extension: The file extension associated with that equipment type (ie, ".memcard")
     * 
     * Returns a populated generic list of the appropriate type containing all the items of the given equipment type found on the disk.
     * If the given dirName does not exist in Application.PersisitentDataPath/equipment/, a directory not found exception will be thrown
     */
    private List<Equipment<TSerializeableType>> loadEquipmentType<TSerializeableType>(string dirName, string extension) {
        List<Equipment<TSerializeableType>> equipmentList = new List<Equipment<TSerializeableType>>();
        DirectoryInfo equipmentDir = new DirectoryInfo(savePath + dirName);

        if (!equipmentDir.Exists) {
            throw new InvalidOperationException("Directory at " + savePath + dirName + " could not be found, equipment objects could not be loaded!");
        }

        // info is a collection of all files in a given directory that have the specified extension
        FileInfo[] info = equipmentDir.GetFiles("*" + extension);
        BinaryFormatter binForm = new BinaryFormatter();
        string fullEquipmentPath;

        foreach (FileInfo file in info) {
            fullEquipmentPath = savePath + dirName + file.Name;
            FileStream equipmentFile = File.Open(fullEquipmentPath, FileMode.Open);

            TSerializeableType equipmentData = (TSerializeableType)binForm.Deserialize(equipmentFile);
            Equipment<TSerializeableType> equipment = new Equipment<TSerializeableType>();
            equipment.data = equipmentData;
            equipmentList.Add(equipment);

            equipmentFile.Close();
        }

        return equipmentList;
    }

    /*
     * TSerializeableType: The type of the serializable class associated with the type of Equipment you want to save (ie, MemCardData for Memory Cards)
     * equipList: The generic list holding the current state of all equipment objects of that type (ie, memCards for Memory Cards)
     * dirName: The (partial) directory path to where the equip files should be located (ie, "memcards/" if the files are in Application.PersisitentDataPath/equipment/memcards/)
     * extension: The file extension associated with that equipment type (ie, ".memcard")
     * 
     * For each equipment object in equipList, a data file is written with the most up-to-date info in the appropriate place with the appropriate extension
     */
    private void saveEquipmentType<TSerializeableType>(List<Equipment<TSerializeableType>> equipList, string dirName, string extension) {
        Debug.Log("Saving equipment data to " + savePath + dirName);
        BinaryFormatter binForm = new BinaryFormatter();
        string fullEquipmentPath;

		// First, check to see if we even have a directory to save into. If not, then be a dear and create one
		DirectoryInfo equipmentDir = new DirectoryInfo(savePath + dirName);
		if (!equipmentDir.Exists) {
			equipmentDir.Create ();
		}

        foreach (Equipment<TSerializeableType> equipment in equipList) {
            // We need to pull info out of equipment.data without actually knowing it's type, which requires a bit of fuckery.
            // If these next few lines are confusing, feel free to read up on C# generics and typing! :P
            Type serializableType = equipment.data.GetType();
			FieldInfo nameField = serializableType.GetField("name");
			string equipmentName = (string)nameField.GetValue(equipment.data);

            fullEquipmentPath = savePath + dirName + equipmentName + extension;
            FileStream equipmentFile;
            if (File.Exists(fullEquipmentPath)) {
                equipmentFile = File.Open(fullEquipmentPath, FileMode.Open);
            } else equipmentFile = File.Create(fullEquipmentPath);

            binForm.Serialize(equipmentFile, equipment.data);
            equipmentFile.Close();
        }
    }

    // Initializes all memory card objects and saves them to disk
    private void initializeMemCards() {
        memCards.Clear();

        Equipment<MemCardData> eightCard = new Equipment<MemCardData>();
        MemCardData eightCardData = new MemCardData();
        eightCardData.name = "8-Slot Card";
        eightCardData.description = "A basic memory card that only holds 8 photos. How boring.";
        eightCardData.cost = 0f;
        eightCardData.capacity = 8;
        eightCard.data = eightCardData;
        memCards.Add(eightCard);

        Equipment<MemCardData> sixteenCard = new Equipment<MemCardData>();
        MemCardData sixteenCardData = new MemCardData();
        sixteenCardData.name = "16-Slot Card";
        sixteenCardData.description = "A decent memory card that holds 16 photos. Not bad!";
        sixteenCardData.cost = 100f;
        sixteenCardData.capacity = 16;
        sixteenCard.data = sixteenCardData;
        memCards.Add(sixteenCard);

        Equipment<MemCardData> thirtyTwoCard = new Equipment<MemCardData>();
        MemCardData thirtyTwoCardData = new MemCardData();
        thirtyTwoCardData.name = "32-Slot Card";
        thirtyTwoCardData.description = "A high-end memory card that holds up to 32 photos. Nice!";
        thirtyTwoCardData.cost = 500f;
        thirtyTwoCardData.capacity = 32;
        thirtyTwoCard.data = thirtyTwoCardData;
        memCards.Add(thirtyTwoCard);

        saveEquipmentType<MemCardData>(memCards, memCardPath, ".memcard");
    }

    // Make sure some assumptions about equipment data setup are followed.
    private void ValidateEquipment() {
        ArrayList usedNames = new ArrayList();
        foreach (Equipment<MemCardData> memCard in memCards) {
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
