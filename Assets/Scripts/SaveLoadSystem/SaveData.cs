using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        public RoomDecorationSaveData[] roomDecorations;

        public TankSaveData[] tanks;

        public GlobalGene[] globalGenes;

        public NPC.NPCData[] npcs;

        public Shop[] shops;

        public Email[] emails;

        public Request[] requests;

        public List<string> tutorialFlags;

        public string versionNumber;
        public string fileIntegrityCheck;
    }







    [System.Serializable]
    public class TankSaveData
    {
        public ShrimpStats[] shrimp;
        public TankDecorationSaveData[] decorations;
        public UpgradeState upgradeState;
        public string tankName;
        public string tankId;
        public bool destinationTank;
        public bool openTank;
        public float openTankPrice;
        public string[] upgradeIDs;
        public float waterTemp;
        public float waterQuality;
        public float waterSalt;
        public float waterHNC;
        public float waterPH;
        public FoodSaveData[] shrimpFood;
    }

    public class FoodSaveData
    {
        public float x, y, z;
        [JsonIgnore]
        public Vector3 position { get { return new Vector3(x, y, z); } set { x = value.x; y = value.y; z = value.z; } }
        public bool settled;
        public float despawnTimer;
        public Item thisItem;

        public static FoodSaveData CreateFoodSaveData(ShrimpFood food)
        {
            FoodSaveData data = new FoodSaveData()
            {
                position = food.transform.position,
                settled = food.settled,
                despawnTimer = food.despawnTimer,
                thisItem = food.thisItem,
            };
            return data;
        }
    }

    [System.Serializable]
    public class TankDecorationSaveData
    {
        public string name;
        public System.Numerics.Vector3 position;
        public System.Numerics.Vector3 rotation;
        public bool floating;
    }

    [System.Serializable]
    public class RoomDecorationSaveData
    {
        public string name;
        public System.Numerics.Vector3 position;
        public System.Numerics.Vector3 rotation;
        public bool locked;
        public int tankSaveReference;
    }
}