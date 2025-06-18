using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class DebugController : MonoBehaviour
{
    bool canShow;
    bool showConsole;
    bool showHelp = true;
    static bool showStats = false;
    string input;
    Vector2 helpScroll;
    Vector2 statsScroll;

    float scrollSpeed = 5;
    float autosaveTime = 1;

    PlayerInput playerInput;
    string oldActionMap;
    float oldAutosaveTime;

    TankViewScript focussedTank;
    ShrimpView focussedShrimp;


    public static DebugCommand HELP;
    public static DebugCommand STATS;

    public static DebugCommand<float> GIVE_MONEY;
    public static DebugCommand<float> SET_MONEY;
    public static DebugCommand<float> GIVE_REPUTATION;
    public static DebugCommand<float> SET_REPUTATION;

    public static DebugCommand<int> SPAWN_RANDOM;
    public static DebugCommand<int> SPAWN_CHERRY;
    public static DebugCommand<int> SPAWN_CARIDID;
    public static DebugCommand<int> SPAWN_NYLON;
    public static DebugCommand<int> SPAWN_ANOMALIS;

    public static DebugCommand<float> WALK_SPEED;
    public static DebugCommand<float> GAME_SPEED;

    public static DebugCommand NEW_GAME;
    public static DebugCommand<string> SAVE_GAME;
    public static DebugCommand<string> LOAD_GAME;
    public static DebugCommand OPEN_SAVE_FOLDER;
    public static DebugCommand<float> SET_AUTOSAVE_DELAY;

    public static DebugCommand RELOAD;
    public static DebugCommand FREEZE;
    public static DebugCommand MAIN_MENU;
    public static DebugCommand FORCE_CLOSE;



    public static DebugCommand<string> NAME_TANK;




    public static DebugCommand<string> NAME_SHRIMP;

    public static DebugCommand<string> SET_GENDER;
    public static DebugCommand<int> SET_AGE;
    public static DebugCommand<int> SET_HUNGER;
    public static DebugCommand<int> SET_SIZE;

    public static DebugCommand<int> SET_TEMPERAMENT;
    public static DebugCommand<int> SET_IMMUNITY;
    public static DebugCommand<int> SET_METABOLISM;
    public static DebugCommand<int> SET_FILTRATION;
    public static DebugCommand<int> SET_TEMPERATURE;

    public static DebugCommand<int> GIVE_BIG_HEAD;
    public static DebugCommand<int> GIVE_DISCOLOUR;
    public static DebugCommand<int> GIVE_BUBBLES;
    public static DebugCommand CURE_SHRIMP;
    public static DebugCommand KILL_SHRIMP;




    public static DebugCommand Spacer;

    private List<object> fullCommandList;
    public List<object> generalCommandList;
    public List<object> tankCommandList;
    public List<object> shrimpCommandList;



    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (Debug.isDebugBuild) canShow = true;

#if UNITY_EDITOR
        canShow = true;
#endif

        if (!canShow) return;



        HELP = new DebugCommand("help", "Shows a list of commands", "help", () =>
        {
            showHelp = true;
            showStats = false;
        });

        STATS = new DebugCommand("stats", "Shows a selection of game stats", "stats", () =>
        {
            showStats = true;
            showHelp = false;
        });



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





        ShelfSpawn shelf = null;
        if (GameObject.Find("Shelving")) shelf = GameObject.Find("Shelving").GetComponent<ShelfSpawn>();
        if (shelf)
        {

            SPAWN_RANDOM = new DebugCommand<int>("spawn_random", "Spawns random shrimp in the destination tank", "spawn_random", (x) =>
            {
                TankController targetTank = shelf.GetDestinationTank();
                if (focussedTank != null) targetTank = focussedTank.GetTank();
                for (int i = 0; i < x; i++)
                    targetTank.SpawnShrimp(TraitSet.None);
            });

            SPAWN_CHERRY = new DebugCommand<int>("spawn_cherry", "Spawns cherry shrimp in the destination tank", "spawn_cherry", (x) =>
            {
                TankController targetTank = shelf.GetDestinationTank();
                if (focussedTank != null) targetTank = focussedTank.GetTank();
                for (int i = 0; i < x; i++)
                    targetTank.SpawnShrimp(TraitSet.Cherry);
            });

            SPAWN_NYLON = new DebugCommand<int>("spawn_nylon", "Spawns nylon shrimp in the destination tank", "spawn_nylon", (x) =>
            {
                TankController targetTank = shelf.GetDestinationTank();
                if (focussedTank != null) targetTank = focussedTank.GetTank();
                for (int i = 0; i < x; i++)
                    targetTank.SpawnShrimp(TraitSet.Nylon);
            });

            SPAWN_ANOMALIS = new DebugCommand<int>("spawn_anomalis", "Spawns anomalis shrimp in the destination tank", "spawn_anomalis", (x) =>
            {
                TankController targetTank = shelf.GetDestinationTank();
                if (focussedTank != null) targetTank = focussedTank.GetTank();
                for (int i = 0; i < x; i++)
                    targetTank.SpawnShrimp(TraitSet.Anomalis);
            });

            SPAWN_CARIDID = new DebugCommand<int>("spawn_caridid", "Spawns caridid shrimp in the destination tank", "spawn_caridid", (x) =>
            {
                TankController targetTank = shelf.GetDestinationTank();
                if (focussedTank != null) targetTank = focussedTank.GetTank();
                for (int i = 0; i < x; i++)
                    targetTank.SpawnShrimp(TraitSet.Caridid);
            });
        }





        PlayerMovement movement = null;
        if (GameObject.Find("Player")) movement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        if (movement)
        {
            WALK_SPEED = new DebugCommand<float>("walk_speed", "Change the player's walk speed (Default is " + movement.Speed + ")", "walk_speed", (x) =>
            {
                movement.Speed = x;
            });
        }

        GAME_SPEED = new DebugCommand<float>("game_speed", "Change the speed at which the game runs", "game_speed", (x) =>
        {
            Time.timeScale = x;
        });

        FREEZE = new DebugCommand("freeze", "Set the gamespeed to 0 and freeze the game", "freeze", () =>
        {
            Time.timeScale = 0;
        });





        SaveController saveController = null;
        if (GameObject.Find("Save Controller")) saveController = GameObject.Find("Save Controller").GetComponent<SaveController>();
        if (saveController)
        {
            NEW_GAME = new DebugCommand("new_game", "Resets everything and starts a new game", "new_game", () =>
            {
                saveController.SaveGame("Autosave");
                SaveManager.currentSaveFile = null;
                SaveManager.gameInitialized = false;
                SaveManager.startNewGame = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });

            RELOAD = new DebugCommand("reload", "Loads the last autosave, can be used if there is a bug preventing gameplay", "reload", () =>
            {
                saveController.SaveGame("Autosave");
                SaveManager.currentSaveFile = "Autosave";
                SaveManager.gameInitialized = false;
                SaveManager.startNewGame = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });

            SAVE_GAME = new DebugCommand<string>("save_game", "Saves the game to this file, any name can be entered so you can have more files", "save_game", (x) =>
            {
                saveController.SaveGame(x);
            });

            LOAD_GAME = new DebugCommand<string>("load_game", "Loads the game from this file (Default files are named like 'SaveFile1')", "load_game", (x) =>
            {
                saveController.SaveGame("Autosave");
                SaveManager.currentSaveFile = x;
                SaveManager.gameInitialized = false;
                SaveManager.startNewGame = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }

        OPEN_SAVE_FOLDER = new DebugCommand("open_save_folder", "Opens the folder containing your game saves in the file explorer", "open_save_folder", () =>
        {
            SaveManager.OpenSaveFolder();
        });

        SET_AUTOSAVE_DELAY = new DebugCommand<float>("set_autosave_delay", "Changes the rate at which the game is autosaved", "set_autosave_delay", (x) =>
        {
            oldAutosaveTime = x;
        });





        if (saveController)
        {
            MAIN_MENU = new DebugCommand("main_menu", "Return to the main menu", "main_menu", () =>
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                saveController.SaveGame("Autosave");
                SaveManager.currentSaveFile = null;
                SaveManager.gameInitialized = false;
                SaveManager.startNewGame = false;
                SceneManager.LoadScene(0);
            });
        }

        FORCE_CLOSE = new DebugCommand("force_close", "Close the game", "force_close", () =>
        {
            Application.Quit();
        });




        generalCommandList = new List<object>
        {
            HELP,
            STATS,

            Spacer,

            GIVE_MONEY,
            SET_MONEY,
            GIVE_REPUTATION,
            SET_REPUTATION,

            Spacer,

            SPAWN_RANDOM,
            SPAWN_CHERRY,
            SPAWN_CARIDID,
            SPAWN_NYLON,
            SPAWN_ANOMALIS,

            Spacer,

            WALK_SPEED,
            GAME_SPEED,

            Spacer,

            NEW_GAME,
            SAVE_GAME,
            LOAD_GAME,
            OPEN_SAVE_FOLDER,
            SET_AUTOSAVE_DELAY,

            Spacer,

            RELOAD,
            FREEZE,
            MAIN_MENU,
            FORCE_CLOSE,

            Spacer,
        };





        NAME_TANK = new DebugCommand<string>("name_tank", "Change the name of the tank", "name_tank", (x) =>
        {
            focussedTank.GetTank().tankName = x;
        });




        tankCommandList = new List<object>
        {
            NAME_TANK,

            Spacer,
        };





        NAME_SHRIMP = new DebugCommand<string>("name_shrimp", "Change the name of the shrimp", "name_shrimp", (x) =>
        {
            focussedShrimp.GetShrimp().name = x;
            focussedShrimp.GetShrimp().stats.name = x;
        });

        SET_GENDER = new DebugCommand<string>("set_gender", "Change the gender of the shrimp (male / female)", "set_gender", (x) =>
        {
            if (x == "male")
                focussedShrimp.GetShrimp().stats.gender = true;
            else if (x == "female")
                focussedShrimp.GetShrimp().stats.gender = false;
        });

        SET_AGE = new DebugCommand<int>("set_age", "Set the age of the shrimp in days (Adult ~ 40, Death ~ 380)", "set_age", (x) =>
        {
            focussedShrimp.GetShrimp().stats.birthTime = TimeManager.instance.CalculateBirthTimeFromAge(x);
        });

        SET_HUNGER = new DebugCommand<int>("set_hunger", "Set the hunger of the shrimp (0-100)", "set_hunger", (x) =>
        {
            focussedShrimp.GetShrimp().stats.hunger = x;
        });

        SET_SIZE = new DebugCommand<int>("set_size", "Set the genetic size of the shrimp", "set_size", (x) =>
        {
            focussedShrimp.GetShrimp().stats.geneticSize = x;
            focussedShrimp.GetShrimp().agent.shrimpModel.localScale = ShrimpManager.instance.GetShrimpSize(
                TimeManager.instance.GetShrimpAge(focussedShrimp.GetShrimp().stats.birthTime),
                focussedShrimp.GetShrimp().stats.geneticSize);
        });




        SET_TEMPERAMENT = new DebugCommand<int>("set_temperament", "Set the temperament of the shrimp (0-100)", "set_temperament", (x) =>
        {
            focussedShrimp.GetShrimp().stats.temperament = x;
        });

        SET_IMMUNITY = new DebugCommand<int>("set_immunity", "Set the illness immunity of the shrimp (0-100)", "set_immunity", (x) =>
        {
            focussedShrimp.GetShrimp().stats.immunity = x;
        });

        SET_METABOLISM = new DebugCommand<int>("set_metabolism", "Set the metabolism of the shrimp (0-100)", "set_metabolism", (x) =>
        {
            focussedShrimp.GetShrimp().stats.metabolism = x;
        });

        SET_FILTRATION = new DebugCommand<int>("set_filtration", "Set the filtration impact of the shrimp (0-100)", "set_filtration", (x) =>
        {
            focussedShrimp.GetShrimp().stats.filtration = x;
        });

        SET_TEMPERATURE = new DebugCommand<int>("set_temperature", "Set the temperature preference of the shrimp (0-100)", "set_temperature", (x) =>
        {
            focussedShrimp.GetShrimp().stats.temperaturePreference = x;
        });




        GIVE_BIG_HEAD = new DebugCommand<int>("give_big_head", "Give the shrimp the big head illness, value is the severity of it (0-100)", "give_big_head", (x) =>
        {
            Symptom symptom = new SymptomBodySize();
            focussedShrimp.GetShrimp().illnessCont.currentSymptoms.Add(symptom);
            symptom.shrimp = focussedShrimp.GetShrimp();
            symptom.severity = x;
            symptom.StartSymptom();


            foreach (IllnessSO so in focussedShrimp.GetShrimp().illnessCont.possibleIllness)
            {
                if (so.symptoms[0] == symptom.symptom)
                {
                    focussedShrimp.GetShrimp().illnessCont.currentIllness.Add(so);
                    focussedShrimp.GetShrimp().illnessCont.AddIllnessToTank(focussedShrimp.GetShrimp().tank, so);
                }
            }

            if (focussedShrimp.GetShrimp().illnessCont.gainIllnessParticles != null)
                GameObject.Instantiate(focussedShrimp.GetShrimp().illnessCont.gainIllnessParticles, 
                    focussedShrimp.GetShrimp().transform.position, focussedShrimp.GetShrimp().transform.rotation, 
                    focussedShrimp.GetShrimp().particleParent);
        });

        GIVE_BUBBLES = new DebugCommand<int>("give_bubbles", "Give the shrimp the bubble illness, value is the severity of it (0-100)", "give_bubbles", (x) =>
        {
            Symptom symptom = new SymptomBubbles();
            focussedShrimp.GetShrimp().illnessCont.currentSymptoms.Add(symptom);
            symptom.shrimp = focussedShrimp.GetShrimp();
            symptom.severity = x;
            symptom.StartSymptom();

            foreach (IllnessSO so in focussedShrimp.GetShrimp().illnessCont.possibleIllness)
            {
                if (so.symptoms[0] == symptom.symptom)
                {
                    focussedShrimp.GetShrimp().illnessCont.currentIllness.Add(so);
                    focussedShrimp.GetShrimp().illnessCont.AddIllnessToTank(focussedShrimp.GetShrimp().tank, so);
                }
            }

            if (focussedShrimp.GetShrimp().illnessCont.gainIllnessParticles != null)
                GameObject.Instantiate(focussedShrimp.GetShrimp().illnessCont.gainIllnessParticles, 
                    focussedShrimp.GetShrimp().transform.position, focussedShrimp.GetShrimp().transform.rotation, 
                    focussedShrimp.GetShrimp().particleParent);
        });

        GIVE_DISCOLOUR = new DebugCommand<int>("give_discolour", "Give the shrimp the discolour illness, value is the severity of it (0-100)", "give_discolour", (x) =>
        {
            Symptom symptom = new SymptomDiscolouration();
            focussedShrimp.GetShrimp().illnessCont.currentSymptoms.Add(symptom);
            symptom.shrimp = focussedShrimp.GetShrimp();
            symptom.severity = x;
            symptom.StartSymptom();

            foreach (IllnessSO so in focussedShrimp.GetShrimp().illnessCont.possibleIllness)
            {
                if (so.symptoms[0] == symptom.symptom)
                {
                    focussedShrimp.GetShrimp().illnessCont.currentIllness.Add(so);
                    focussedShrimp.GetShrimp().illnessCont.AddIllnessToTank(focussedShrimp.GetShrimp().tank, so);
                }
            }

            if (focussedShrimp.GetShrimp().illnessCont.gainIllnessParticles != null)
                GameObject.Instantiate(focussedShrimp.GetShrimp().illnessCont.gainIllnessParticles, 
                    focussedShrimp.GetShrimp().transform.position, focussedShrimp.GetShrimp().transform.rotation, 
                    focussedShrimp.GetShrimp().particleParent);
        });

        CURE_SHRIMP = new DebugCommand("cure_shrimp", "Cure all illnesses on the shrimp", "cure_shrimp", () =>
        {
            focussedShrimp.GetShrimp().illnessCont.CureAllIllnesses();
        });

        KILL_SHRIMP = new DebugCommand("kill_shrimp", "Kill the shrimp", "kill_shrimp", () =>
        {
            focussedShrimp.GetShrimp().KillShrimp();
        });




        shrimpCommandList = new List<object>
        {
            NAME_SHRIMP,
            SET_GENDER,
            SET_AGE,
            SET_HUNGER,
            SET_SIZE,

            Spacer,

            SET_TEMPERAMENT,
            SET_IMMUNITY,
            SET_METABOLISM,
            SET_FILTRATION,
            SET_TEMPERATURE,

            Spacer,

            GIVE_BIG_HEAD,
            GIVE_DISCOLOUR,
            GIVE_BUBBLES,
            CURE_SHRIMP,
            KILL_SHRIMP,

            Spacer,
        };
    }


    public void OnToggleDebug(InputValue value)
    {
        if (canShow)
        {
            showConsole = !showConsole;

            SaveController saveController = GameObject.Find("Save Controller").GetComponent<SaveController>();


            if (showConsole)  // Opening menu
            {
                oldActionMap = playerInput.currentActionMap.name;
                playerInput.SwitchCurrentActionMap("Debug");


                if (saveController) oldAutosaveTime = saveController.autosaveTime;
                if (saveController) saveController.autosaveTime = autosaveTime;


                if (UIManager.instance != null)
                {
                    TankViewScript tv = UIManager.instance.GetScreen() as TankViewScript;
                    if (tv != null)
                    {
                        focussedTank = tv;
                    }
                    ShrimpView sv = UIManager.instance.GetScreen() as ShrimpView;
                    if (sv != null)
                    {
                        focussedShrimp = sv;
                    }
                }


                fullCommandList = new List<object>();
                if (focussedTank) fullCommandList.AddRange(tankCommandList);
                if (focussedShrimp) fullCommandList.AddRange(shrimpCommandList);
                fullCommandList.AddRange(generalCommandList);
            }

            else  // Closing menu
            {
                playerInput.SwitchCurrentActionMap(oldActionMap);
                if (saveController) saveController.autosaveTime = oldAutosaveTime;
                focussedTank = null;
                focussedShrimp = null;
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

            //Command Bar
            GUI.Box(new Rect(0, y, Screen.width, 30), "");


            // Input Field
            GUI.SetNextControlName("Console");
            input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
            if (input != null) input = input.Replace("/", "");
            GUI.FocusControl("Console");

            y += 30;


            // Help Box
            if (showHelp)
            {
                GUI.Box(new Rect(0, y, 600, Screen.height - y), "");

                Rect viewport = new Rect(0, 0, 600 - 30, 20 * fullCommandList.Count);
                helpScroll = GUI.BeginScrollView(new Rect(0, y + 5f, 600, Screen.height - y - 10), helpScroll, viewport);

                for (int i = 0; i < fullCommandList.Count; i++)
                {
                    if (fullCommandList[i] != null)
                    {
                        DebugCommandBase command = fullCommandList[i] as DebugCommandBase;

                        string labelParameter = "";
                        if (command as DebugCommand<int> != null) labelParameter = " <int>";
                        if (command as DebugCommand<float> != null) labelParameter = " <float>";

                        string label = $"{command.commandFormat}{labelParameter}  -  {command.commandDescription}";

                        Rect labelRect = new Rect(5, 20 * i, viewport.width - 30, 20);
                        GUI.Label(labelRect, label);
                    }
                }

                GUI.EndScrollView();
                y += 100;
            }


            // Stats Box
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

                //GUI.backgroundColor = new Color(1, 1, 1, 0.5f);
                GUI.Box(new Rect(0, y, 300, Screen.height - y), "");
                Rect viewport = new Rect(0, y, 270, Screen.height * 2);
                statsScroll = GUI.BeginScrollView(new Rect(0, y + 5, 300, Screen.height - y - 10), statsScroll, viewport);

                Rect labelRect = new Rect(5, y + 5, viewport.width - 10, viewport.height);
                GUI.Label(labelRect, saveData);

                GUI.EndScrollView();
            }
        }
    }


    private void HandleInput()
    {
        string[] properties = input.Split(' ');

        for (int i = 0; i < fullCommandList.Count; i++)
        {
            if (fullCommandList[i] != null)
            {
                DebugCommandBase commandBase = fullCommandList[i] as DebugCommandBase;
                if (input.Contains(commandBase.commandID))
                {
                    if (fullCommandList[i] as DebugCommand != null)
                    {
                        (fullCommandList[i] as DebugCommand).Invoke();
                    }
                    else if (fullCommandList[i] as DebugCommand<int> != null)
                    {
                        (fullCommandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                    }
                    else if (fullCommandList[i] as DebugCommand<float> != null)
                    {
                        (fullCommandList[i] as DebugCommand<float>).Invoke(float.Parse(properties[1]));
                    }
                    else if (fullCommandList[i] as DebugCommand<string> != null)
                    {
                        (fullCommandList[i] as DebugCommand<string>).Invoke(properties[1]);
                    }
                }
            }
        }
    }
}