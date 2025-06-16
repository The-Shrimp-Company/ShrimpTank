using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class DebugController : MonoBehaviour
{
    bool showConsole;
    bool showHelp;
    string input;
    Vector2 helpScroll;

    PlayerInput playerInput;
    string oldActionMap;


    public static DebugCommand<float> SET_MONEY;

    public static DebugCommand SPAWN_SHRIMP;

    public static DebugCommand HELP;

    public List<object> commandList;



    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        SET_MONEY = new DebugCommand<float>("set_money", "Sets your money", "set_money", (x) =>
        {
            if (Money.instance != null) Money.instance.SetMoney(x);
            else Debug.LogError("SET_MONEY Command failed");
        });


        SPAWN_SHRIMP = new DebugCommand("spawn_shrimp", "Spawns a random shrimp in the destination tank", "spawn_shrimp", () =>
        {
            ShelfSpawn shelf = GameObject.Find("Shelving").GetComponent<ShelfSpawn>();
            if (shelf)
            {
                shelf.GetDestinationTank().SpawnShrimp();
            }
            else Debug.LogError("SPAWN_SHRIMP Command failed");
        });


        HELP = new DebugCommand("help", "Shows a list of commands", "help", () =>
        {
            showHelp = !showHelp;
        });




        commandList = new List<object>
        {
            SET_MONEY,

            SPAWN_SHRIMP,

            HELP,
        };
    }


    public void OnToggleDebug(InputValue value)
    {
        showConsole = !showConsole;


        if (showConsole)  // Opening menu
        {
            oldActionMap = playerInput.currentActionMap.name;
            playerInput.SwitchCurrentActionMap("Debug");
        }
        else  // Closing menu
        {
            playerInput.SwitchCurrentActionMap(oldActionMap);
            oldActionMap = null;
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


    private void OnGUI()
    {
        if (!showConsole) { return; }

        float y = 0f;

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

                string label = $"{command.commandFormat}{labelParameter} - {command.commandDescription}";

                Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                GUI.Label(labelRect, label);
            }

            GUI.EndScrollView();
            y += 100;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);

        GUI.SetNextControlName("Console");
        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
        GUI.FocusControl("Console");
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