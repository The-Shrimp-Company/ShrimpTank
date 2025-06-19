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

    private Button newGameButton;
    private Button continueButton;
    private Button saveButton;
    private Button backButton;
    private Button QuitButton;
    private Label loadingText;
    private VisualElement mainScreen;
    private VisualElement loadingScreen;
    private VisualElement saveScreen;

    private Button[] saveFiles = new Button[3];


    private bool loading = false;
    private int count;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;


        newGameButton = root.Q<Button>("MainMenuButton");
        newGameButton.clicked += NewGame;

        continueButton = root.Q<Button>("ContinueButton");
        continueButton.clicked += ContinueGame;

        saveButton = root.Q<Button>("SaveButton");
        saveButton.clicked += OpenSaveScreen;

        backButton = root.Q<Button>("BackMainMenu");
        backButton.clicked += OpenMainMenu;

        QuitButton = root.Q<Button>("QuitButton");
        QuitButton.clicked += QuitGame;

        loadingText = root.Q<Label>("LoadingText");

        saveFiles[0] = root.Q<Button>("SaveFile1");
        saveFiles[1] = root.Q<Button>("SaveFile2");
        saveFiles[2] = root.Q<Button>("SaveFile3");

        mainScreen = root.Q<VisualElement>("MainScreen");
        loadingScreen = root.Q<VisualElement>("LoadingScreen");
        saveScreen = root.Q<VisualElement>("SaveMenu");

        mainScreen.BringToFront();
    }

    private void Update()
    {
        if (loading)
        {
            if(count >= 10)
            {
                loadingText.text += ".";
                count = 0;
            }
            count++;
            Debug.Log("here");
        }
    }

    private void NewGame()
    {
        LoadingScreen();
        SaveManager.startNewGame = true;
        SceneManager.LoadScene("ShopScene");
    }

    private void ContinueGame()
    {
        LoadingScreen();
        SaveManager.startNewGame = false;
        SaveManager.gameInitialized = false;
        SceneManager.LoadScene("ShopScene");
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
