using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using SaveLoadSystem;
using UnityEngine.InputSystem.Composites;

public class UIController : MonoBehaviour
{
    private Label loadingText;
    private VisualElement mainScreen;
    private VisualElement loadingScreen;
    private VisualElement saveScreen;

    private Button[] saveFiles = new Button[3];


    private bool loading = false;
    private int count;

    private AsyncOperation operation;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Assigning functions
        root.Q<Button>("MainMenuButton").clicked += NewGame;
        root.Q<Button>("ContinueButton").clicked += ContinueGame;
        root.Q<Button>("SaveButton").clicked += OpenSaveScreen;
        root.Q<Button>("BackMainMenu").clicked += OpenMainMenu;
        root.Q<Button>("QuitButton").clicked += QuitGame;

        // Storing Values for later
        loadingText = root.Q<Label>("LoadingText");

        saveFiles[0] = root.Q<Button>("SaveFile1");
        saveFiles[1] = root.Q<Button>("SaveFile2");
        saveFiles[2] = root.Q<Button>("SaveFile3");

        mainScreen = root.Q<VisualElement>("MainScreen");
        loadingScreen = root.Q<VisualElement>("LoadingScreen");
        saveScreen = root.Q<VisualElement>("SaveMenu");

        // Ensuring correct order
        mainScreen.BringToFront();
    }

    private void Update()
    {
        if (loading)
        {
            if(count >= 1)
            {
                loadingText.text += ".";
                count = 0;
            }
            count++;
            Debug.Log("Here");
            if (operation.isDone)
            {
                SceneManager.UnloadSceneAsync(0);
            }
        }
    }

    private void NewGame()
    {
        LoadingScreen();
        SaveManager.startNewGame = true;
        operation = SceneManager.LoadSceneAsync("OpeningMenu");
    }

    private void ContinueGame()
    {
        LoadingScreen();
        SaveManager.startNewGame = false;
        SaveManager.gameInitialized = false;
        operation = SceneManager.LoadSceneAsync("ShopScene");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
    private void OpenSaveScreen()
    {

        saveScreen.BringToFront();

        foreach(Button button in saveFiles)
        {
            button.clicked += () =>
            {
                SaveManager.currentSaveFile = button.name;
            };
            if (SaveManager.TryLoadGame(button.name))
            {
                
                button.clicked += ContinueGame;
                button.text = button.name;
            }
            else
            {
                button.clicked += NewGame;
                button.text = "Empty File";
            }
        }
    }



    private void OpenMainMenu()
    {
        mainScreen.BringToFront();
    }

    private void LoadingScreen()
    {
        loadingScreen.BringToFront();
        loading = true;
    }
}
