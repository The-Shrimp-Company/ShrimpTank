using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class DebugController : MonoBehaviour
{
    bool canShow;
    bool showConsole;
    bool showHelp;
    static bool showStats;
    string input;
    Vector2 helpScroll;
    Vector2 statsScroll;

    float scrollSpeed = 5;
    float autosaveTime = 1;

    PlayerInput playerInput;
    string oldActionMap;
    float oldAutosaveTime;


    public static DebugCommand<float> GIVE_MONEY;
    public static DebugCommand<float> SET_MONEY;
    public static DebugCommand<float> GIVE_REPUTATION;
    public static DebugCommand<float> SET_REPUTATION;

    public static DebugCommand<int> SPAWN_RANDOM;
    public static DebugCommand<int> SPAWN_CHERRY;
    public static DebugCommand<int> SPAWN_CARIDID;
    public static DebugCommand<int> SPAWN_NYLON;
    public static DebugCommand<int> SPAWN_ANOMALIS;

    public static DebugCommand HELP;

    public List<object> commandList;



    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (Debug.isDebugBuild) canShow = true;

        #if UNITY_EDITOR
        canShow = true;
        #endif



        GIVE_MONEY = new DebugCommand<float>("give_money", "Gives or removes money", "give_money", (x) =>
        {
            if (Money.instance != null) Money.instance.AddMoney(x);
            else Debug.LogError("SET_MONEY Command failed");
        });

        SET_MONEY = new DebugCommand<float>("set_money", "Sets your money", "set_money", (x) =>
        {
            if (Money.instance != null) Money.instance.SetMoney(x);
            else Debug.LogError("SET_MONEY Command failed");
        });

        GIVE_REPUTATION = new DebugCommand<float>("give_reputation", "Gives or removes store reputation", "give_reputation", (x) =>
        {
            Reputation.AddReputation(x);
        });

        SET_REPUTATION = new DebugCommand<float>("set_reputation", "Sets your store reputation", "set_reputation", (x) =>
        {
            Reputation.SetReputation(x);
        });





        SPAWN_RANDOM = new DebugCommand<int>("spawn_random", "Spawns random shrimp in the destination tank", "spawn_random", (x) =>
        {
            ShelfSpawn shelf = GameObject.Find("Shelving").GetComponent<ShelfSpawn>();
            if (shelf)
                for (int i = 0; i < x; i++)
                    shelf.GetDestinationTank().SpawnShrimp(TraitSet.None);
            else Debug.LogError("SPAWN_RANDOM Command failed");
        });

        SPAWN_CHERRY = new DebugCommand<int>("spawn_cherry", "Spawns cherry shrimp in the destination tank", "spawn_cherry", (x) =>
        {
            ShelfSpawn shelf = GameObject.Find("Shelving").GetComponent<ShelfSpawn>();
            if (shelf)
                for (int i = 0; i < x; i++)
                    shelf.GetDestinationTank().SpawnShrimp(TraitSet.Cherry);
            else Debug.LogError("SPAWN_CHERRY Command failed");
        });

        SPAWN_NYLON = new DebugCommand<int>("spawn_nylon", "Spawns nylon shrimp in the destination tank", "spawn_nylon", (x) =>
        {
            ShelfSpawn shelf = GameObject.Find("Shelving").GetComponent<ShelfSpawn>();
            if (shelf)
                for (int i = 0; i < x; i++)
                    shelf.GetDestinationTank().SpawnShrimp(TraitSet.Nylon);
            else Debug.LogError("SPAWN_NYLON Command failed");
        });

        SPAWN_ANOMALIS = new DebugCommand<int>("spawn_anomalis", "Spawns anomalis shrimp in the destination tank", "spawn_anomalis", (x) =>
        {
            ShelfSpawn shelf = GameObject.Find("Shelving").GetComponent<ShelfSpawn>();
            if (shelf)
                for (int i = 0; i < x; i++)
                    shelf.GetDestinationTank().SpawnShrimp(TraitSet.Anomalis);
            else Debug.LogError("SPAWN_ANOMALIS Command failed");
        });

        SPAWN_CARIDID = new DebugCommand<int>("spawn_caridid", "Spawns caridid shrimp in the destination tank", "spawn_caridid", (x) =>
        {
            ShelfSpawn shelf = GameObject.Find("Shelving").GetComponent<ShelfSpawn>();
            if (shelf)
                for (int i = 0; i < x; i++)
                    shelf.GetDestinationTank().SpawnShrimp(TraitSet.Caridid);
            else Debug.LogError("SPAWN_CARIDID Command failed");
        });





        HELP = new DebugCommand("help", "Shows a list of commands", "help", () =>
        {
            showHelp = !showHelp;
        });




        commandList = new List<object>
        {
            GIVE_MONEY,
            SET_MONEY,
            GIVE_REPUTATION,
            SET_REPUTATION,

            SPAWN_RANDOM,
            SPAWN_CHERRY,
            SPAWN_CARIDID,
            SPAWN_NYLON,
            SPAWN_ANOMALIS,

            HELP,
        };
    }


    public void OnToggleDebug(InputValue value)
    {
        if (canShow)
        {
            showConsole = !showConsole;
            showStats = !showStats;


            SaveController saveController = GameObject.Find("Save Controller").GetComponent<SaveController>();

            if (showConsole)  // Opening menu
            {
                oldActionMap = playerInput.currentActionMap.name;
                playerInput.SwitchCurrentActionMap("Debug");

                if (saveController) oldAutosaveTime = saveController.autosaveTime;
                if (saveController) saveController.autosaveTime = autosaveTime;
            }
            else  // Closing menu
            {
                playerInput.SwitchCurrentActionMap(oldActionMap);
                if (saveController) saveController.autosaveTime = oldAutosaveTime;
            }
        }
    }


    public void OnReturnDebug(InputValue value)
    {
        if (showConsole)
        {
            HandleInput();
            input = "";
        }
    }


    public void OnScrollDebug(InputValue value)
    {
        if (showHelp)
        {
            helpScroll.y += value.Get<Vector2>().y * scrollSpeed;
        }
        if (showStats)
        {
            statsScroll.y += value.Get<Vector2>().y * scrollSpeed;
        }
    }


    private void OnGUI()
    {
        float y = 0f;
        if (showConsole)
        {
            // Help Box
            if (showHelp)
            {
                GUI.Box(new Rect(0, y, Screen.width, 100), "");

                Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);
                helpScroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), helpScroll, viewport);

                for (int i = 0; i < commandList.Count; i++)
                {
                    DebugCommandBase command = commandList[i] as DebugCommandBase;

                    string labelParameter = "";
                    if (command as DebugCommand<int> != null) labelParameter = " <int>";
                    if (command as DebugCommand<float> != null) labelParameter = " <float>";

                    string label = $"{command.commandFormat}{labelParameter}  -  {command.commandDescription}";

                    Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                    GUI.Label(labelRect, label);
                }

                GUI.EndScrollView();
                y += 100;
            }


            //Command Bar
            GUI.Box(new Rect(0, y, Screen.width, 30), "");

            // Input Field
            GUI.SetNextControlName("Console");
            input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
            if (input != null) input = input.Replace("/", "");
            GUI.FocusControl("Console");

            y += 30;
        }

        if (showStats)
        {
            string saveData = JsonUtility.ToJson(SaveManager.CurrentSaveData);
            if (saveData != null)
            {
                saveData = saveData.Replace(",", "\n");
                saveData = saveData.Replace(":", " - ");
                saveData = saveData.Replace("{", "");
                saveData = saveData.Replace("}", "");
                saveData = saveData.Replace("\"", "");
                saveData = saveData.Replace("playerPosition - ", "");
                saveData = saveData.Replace("playerStats - ", "");
                saveData = saveData.Replace("gameSettings - ", "");
                saveData = saveData.Replace("inventoryItems - ", "");
                saveData = saveData.Replace("inventoryQuantities - ", "");
            }

            GUI.backgroundColor = new Color(1, 1, 1, 0.5f);
            GUI.Box(new Rect(0, y, 250, Screen.height - y), "");
            Rect viewport = new Rect(0, y, 250, Screen.height * 2);
            statsScroll = GUI.BeginScrollView(new Rect(0, y, 250, Screen.height - y), statsScroll, viewport);

            Rect labelRect = new Rect(5, y + 5, viewport.width - 10, viewport.height);
            GUI.Label(labelRect, saveData);

            GUI.EndScrollView();
        }


    }


    private void HandleInput()
    {
        string[] properties = input.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;
            if (input.Contains(commandBase.commandID))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<int> != null)
                {
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                }
                else if (commandList[i] as DebugCommand<float> != null)
                {
                    (commandList[i] as DebugCommand<float>).Invoke(float.Parse(properties[1]));
                }
                //else if (commandList[i] as DebugCommand<string> != null)
                //{
                //    (commandList[i] as DebugCommand<string>).Invoke(string.Parse(properties[1]));
                //}
            }
        }
    }
}