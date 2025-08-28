using SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Xml;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    public float autosaveTime = 15.0f;
    private float autosaveTimer = 0f;
    [SerializeField] bool loadPlayerPosition = true;

    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (SaveManager.startNewGame)
        {
            NewGame();
            return;
        }

        if (SaveManager.currentSaveFile != null && SaveManager.currentSaveFile != "")
        {
            LoadGame(SaveManager.currentSaveFile);
            return;
        }

        Debug.Log("Autosave");
        LoadGame("Autosave");
    }


    private void OnApplicationPause()  // Autosave when game is suspended
    {
        SaveGame("Autosave");
    }


    private void OnApplicationQuit()  // Autosave when game is quit
    {
        SaveGame("Autosave");
    }


    void Update()
    {
        autosaveTimer += Time.deltaTime;
        if ( autosaveTimer > autosaveTime )  // Autosave after a certain amount of time
        {
            autosaveTimer = 0;
            SaveGame("Autosave");
        }

    }

    public void SaveGame(string _fileName)
    {
        if (!SaveManager.gameInitialized) return;  // If the game hasn't loaded yet
        if (SaveManager.currentlySaving) return;   // If the game is already saving

        SaveManager.currentlySaving = true;
        CopyDataToSaveData();
        SaveManager.SaveGame(_fileName);
        SaveManager.currentlySaving = false;
    }

    public void LoadGame(string _fileName)
    {
        Debug.Log("Loading " +  _fileName + "...");

        SaveManager.LoadGame(_fileName);
        
        if (SaveManager.loadingGameFromFile)  // Loading was successful
            CopyDataFromSaveData(SaveManager.CurrentSaveData);

        SaveManager.gameInitialized = true;
        SaveManager.OnLoadGameFinish?.Invoke();
    }



    private void CopyDataToSaveData()  // Save
    {
        SaveData d = new();

        // Money
        d.money = Money.instance.money;

        // Reputation
        d.reputation = Reputation.GetReputation();

        // Time
        d.totalTime = TimeManager.instance.totalTime;
        d.day = TimeManager.instance.day;
        d.year = TimeManager.instance.year;

        // Store
        d.storeName = Store.StoreName;

        // Player
        Transform player = GameObject.Find("Player").transform;
        d.playerPosition = new System.Numerics.Vector3(player.position.x, player.position.y, player.position.z);
        d.playerRotation = player.GetComponent<CameraControls>().GetRotationX();

        // Stats & Settings
        d.playerStats = PlayerStats.stats;
        d.gameSettings = GameSettings.settings;
        d.versionNumber = Application.version;

        // Inventory
        d.inventoryItems = Inventory.GetInventory(false, false).ToArray();

        // Global Genes
        if (GeneManager.instance)
        {
            d.globalGenes = GeneManager.instance.GetGlobalGeneArray();
        }



        // Room Decorations
        int tankIndex = 0;
        List<RoomDecorationSaveData> decorationsInRoom = new List<RoomDecorationSaveData>();
        List<TankSaveData> tanks = new List<TankSaveData>();
        foreach (Decoration decoration in Store.decorateController.decorationsInStore)
        {
            RoomDecorationSaveData decorationSaveData = new RoomDecorationSaveData();
            decorationSaveData.name = decoration.decorationSO.itemName;
            decorationSaveData.position = new System.Numerics.Vector3(decoration.transform.position.x, decoration.transform.position.y, decoration.transform.position.z);
            decorationSaveData.rotation = new System.Numerics.Vector3(decoration.transform.rotation.eulerAngles.x, decoration.transform.rotation.eulerAngles.y, decoration.transform.rotation.eulerAngles.z);
            decorationSaveData.locked = decoration.locked;

            // Tank
            TankController t = null;
            decoration.TryGetComponent<TankController>(out t);
            if (t != null)
            {
                TankSaveData tankSave = new TankSaveData();

                tankSave.tankName = t.tankName;
                tankSave.tankId = t.tankId;
                tankSave.upgradeState = t.upgradeState;
                tankSave.destinationTank = t.destinationTank;
                tankSave.openTank = t.openTank;
                tankSave.openTankPrice = t.openTankPrice;
                tankSave.upgradeIDs = t.GetComponent<TankUpgradeController>().SaveUpgrades();
                tankSave.waterTemp = t.waterTemperature;
                tankSave.waterQuality = t.waterQuality;
                tankSave.waterSalt = t.waterSalt;
                tankSave.waterPH = t.waterPh;
                tankSave.waterHNC = t.waterAmmonium;
                tankSave.alarmIds = t.AlarmIds.ToArray();


                // Food
                tankSave.shrimpFood = new FoodSaveData[t.foodInTank.Count];
                for (int i = 0; i < t.foodInTank.Count; i++)
                {
                    tankSave.shrimpFood[i] = FoodSaveData.CreateFoodSaveData(t.foodInTank[i]);
                }


                // Shrimp
                List<ShrimpStats> shrimpInTank = new List<ShrimpStats>();
                foreach (Shrimp s in t.shrimpInTank)
                {
                    s.illnessCont.SaveIllnesses();
                    shrimpInTank.Add(s.stats);
                }
                tankSave.shrimp = shrimpInTank.ToArray();


                // Tank Decorations
                List<TankDecorationSaveData> decorationsInTank = new List<TankDecorationSaveData>();
                foreach (GameObject obj in t.decorationsInTank)
                {
                    Decoration tankDecoration = obj.GetComponent<Decoration>();
                    TankDecorationSaveData tankDecorationSaveData = new();
                    tankDecorationSaveData.name = tankDecoration.decorationSO.itemName;
                    tankDecorationSaveData.position = new System.Numerics.Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
                    tankDecorationSaveData.rotation = new System.Numerics.Vector3(obj.transform.rotation.eulerAngles.x, obj.transform.rotation.eulerAngles.y, obj.transform.rotation.eulerAngles.z);
                    tankDecorationSaveData.floating = tankDecoration.floating;
                    decorationsInTank.Add(tankDecorationSaveData);
                }
                tankSave.decorations = decorationsInTank.ToArray();


                decorationSaveData.tankSaveReference = tankIndex;
                tankIndex++;
                tanks.Add(tankSave);
            }
            decorationsInRoom.Add(decorationSaveData);
        }
        d.roomDecorations = decorationsInRoom.ToArray();
        d.tanks = tanks.ToArray();

        // NPCs
        List<NPC.NPCData> npcdata = new List<NPC.NPCData>();
        foreach(NPC npc in NPCManager.Instance.NPCs)
        {
            npcdata.Add(npc.Data);
        }
        d.npcs = npcdata.ToArray();

        // Emails
        d.emails = EmailManager.instance.emails.ToArray();

        // NPC Shops
        d.shops = ShopManager.instance.shops.ToArray();

        // Requests
        d.requests = CustomerManager.Instance.requests.ToArray();

        // Tutorial
        d.tutorialFlags = Tutorial.instance.flags;

        SaveManager.CurrentSaveData = d;
    }



    private void CopyDataFromSaveData(SaveData d)  // Load
    {
        // Money
        Money.instance.SetMoney(d.money);

        // Time
        TimeManager.instance.totalTime = d.totalTime;
        TimeManager.instance.prevDay = d.day - 1;
        TimeManager.instance.prevYear = d.year - 1;

        // Player
        Transform player = GameObject.Find("Player").transform;
        if (loadPlayerPosition) player.position = new Vector3(d.playerPosition.X, d.playerPosition.Y, d.playerPosition.Z);
        if (loadPlayerPosition) player.GetComponent<CameraControls>().SetRotationX(d.playerRotation);

        // Stats & Settings
        PlayerStats.stats = d.playerStats;
        GameSettings.settings = d.gameSettings;

        // Inventory
        Inventory.instance.Initialize(d.inventoryItems);

        // Room Decorations
        Store.decorateController.LoadDecorations(d.roomDecorations, d.tanks);

        // Requests
        CustomerManager.Instance.Initialize(d.requests);

        // Npcs
        NPCManager.Instance.Initialize();

        // Shops
        ShopManager.instance.Initialize(d.shops.ToList());

        // Emails
        EmailManager.instance.Initialize();

        // Reputation
        Reputation.SetReputation(d.reputation);

        // Tutorial
        Tutorial.instance.init();
    }


    private void NewGame()
    {
        PlayerStats.stats = new Stats();
        SaveManager.CurrentSaveData = new SaveData();
        Inventory.instance.Initialize();
        CustomerManager.Instance.Initialize();
        EmailManager.instance.Initialize();
        ShopManager.instance.Initialize();
        NPCManager.Instance.Initialize();
        Money.instance.SetStartingMoney();
        Reputation.SetReputation(0);
        SaveManager.NewGame();
    }
}
