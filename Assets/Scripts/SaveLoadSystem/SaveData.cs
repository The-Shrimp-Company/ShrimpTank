using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadSystem
{
    [System.Serializable]
    public class SaveData
    {
        public float money;
        public float reputation;
        public float totalTime;
        public int day;
        public int year;
        public System.Numerics.Vector3 playerPosition;
        public float playerRotation;

        public string storeName;


        public Stats playerStats;
        public Settings gameSettings;

        public Item[] inventoryItems;

        public ShelfSaveData[] shelves;

        public GlobalGene[] globalGenes;

        public NPC.NPCData[] npcs;

        public Email[] emails;

        public Request[] requests;

        public List<string> tutorialFlags;

        public string versionNumber;
        public string fileIntegrityCheck;
    }





    [System.Serializable]
    public class ShelfSaveData
    {
        public TankSocketSaveData[] tanks;
    }

    [System.Serializable]
    public class TankSocketSaveData
    {
        public int socketNumber;
        public TankTypes type;
        public TankSaveData tank = null;
    }

    [System.Serializable]
    public class TankSaveData
    {
        public ShrimpStats[] shrimp;
        public TankDecorationSaveData[] decorations;
        public string tankName;
        public bool destinationTank;
        public bool openTank;
        public float openTankPrice;
        public string[] upgradeIDs;
    }

    [System.Serializable]
    public class TankDecorationSaveData
    {
        public string name;
        public System.Numerics.Vector3 position;
        public System.Numerics.Vector3 rotation;
        public bool floating;
    }
}