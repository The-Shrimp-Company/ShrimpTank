using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    public float autosaveTime = 15.0f;
    private float autosaveTimer = 0f;
    [SerializeField] bool loadPlayerPosition = true;

    private ShelfSpawn shelfSpawn;

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

        //Debug.Log(GameSettings.settings.cameraSensitivity);
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
        SaveData d = new SaveData();

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


        // Shelves, Tanks and Shrimp
        if (shelfSpawn == null) shelfSpawn = (ShelfSpawn)FindObjectOfType(typeof(ShelfSpawn));
        if (shelfSpawn == null) Debug.LogWarning("Save Controller could not find Shelf Spawn");
        else
        {
            List<ShelfSaveData> shelfList = new List<ShelfSaveData>();
            foreach (Shelf shelf in shelfSpawn._shelves)
            {
                ShelfSaveData shelfSave = new ShelfSaveData();

                if (shelf != null && shelf.gameObject.activeSelf)
                {
                    int index = 0;
                    List<TankSocketSaveData> socketList = new List<TankSocketSaveData>();
                    foreach (TankSocket socket in shelf._tanks)
                    {
                        if (socket.tank != null && socket.tank.gameObject.activeInHierarchy)
                        {
                            TankSocketSaveData socketSave = new TankSocketSaveData();
                            TankSaveData tankSave = new TankSaveData();
                            socketSave.tank = tankSave;
                            socketSave.socketNumber = index;

                            List<ShrimpStats> shrimpInTank = new List<ShrimpStats>();
                            foreach (Shrimp s in socket.tank.shrimpInTank)
                            {
                                s.illnessCont.SaveIllnesses();
                                shrimpInTank.Add(s.stats);
                            }
                            tankSave.shrimp = shrimpInTank.ToArray();

                            List<TankDecorationSaveData> decorationsInTank = new List<TankDecorationSaveData>();
                            foreach (GameObject obj in socket.tank.decorationsInTank)
                            {
                                Decoration decoration = obj.GetComponent<Decoration>();
                                TankDecorationSaveData decorationSaveData = new TankDecorationSaveData();
                                decorationSaveData.name = decoration.decorationSO.itemName;
                                decorationSaveData.position = new System.Numerics.Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
                                decorationSaveData.rotation = new System.Numerics.Vector3(obj.transform.rotation.eulerAngles.x, obj.transform.rotation.eulerAngles.y, obj.transform.rotation.eulerAngles.z);
                                decorationSaveData.floating = decoration.floating;
                                decorationsInTank.Add(decorationSaveData);
                            }
                            tankSave.decorations = decorationsInTank.ToArray();

                            tankSave.tankName = socket.tank.tankName;
                            tankSave.destinationTank = socket.tank.destinationTank;
                            tankSave.openTank = socket.tank.openTank;
                            tankSave.openTankPrice = socket.tank.openTankPrice;
                            tankSave.upgradeIDs = socket.tank.GetComponent<TankUpgradeController>().SaveUpgrades();

                            socketList.Add(socketSave);
                        }

                        index++;
                    }

                    shelfSave.tanks = socketList.ToArray();
                }

                shelfList.Add(shelfSave);
            }
            d.shelves = shelfList.ToArray();
        }

        // NPCs
        List<NPC.NPCData> npcdata = new List<NPC.NPCData>();
        foreach(NPC npc in NPCManager.Instance.NPCs)
        {
            npcdata.Add(npc.Data);
        }
        d.npcs = npcdata.ToArray();

        // Emails
        d.emails = EmailManager.instance.emails.ToArray();

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

        // Requests
        CustomerManager.Instance.Initialize(d.requests);

        // Npcs
        NPCManager.Instance.Initialize();

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
        Inventory.instance.Initialize();
        CustomerManager.Instance.Initialize();
        EmailManager.instance.Initialize();
        NPCManager.Instance.Initialize();
        Money.instance.SetStartingMoney();
        Reputation.SetReputation(0);
        SaveManager.NewGame();
    }
}
