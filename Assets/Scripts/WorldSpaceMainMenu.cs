using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceMainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject Player;

    [SerializeField]
    private SaveController SaveController;

    [SerializeField]
    private GameObject MainMenu;
    [SerializeField]
    private GameObject SaveMenu;

    [SerializeField]
    private List<Button> saveFiles;


    // Start is called before the first frame update
    void Start()
    {
        Camera cam = Player.GetComponentInChildren<Camera>();
        foreach (Button button in saveFiles)
        {
            button.onClick.AddListener(() =>
            {
                SaveManager.currentSaveFile = button.name;
                SaveController.LoadGame(button.name);
            });
            if (SaveManager.TryLoadGame(button.name))
            {
                SaveManager.LoadGame(button.name);
                button.onClick.AddListener(ContinueGame);
                button.GetComponentInChildren<TextMeshProUGUI>().text = SaveManager.CurrentSaveData.storeName;
                //button.Q<Label>().text = TimeManager.DateFromTime(SaveManager.CurrentSaveData.totalTime);
            }
            else
            {
                button.onClick.AddListener(NewGame);
                button.GetComponentInChildren<TextMeshProUGUI>().text = "Empty File";
                //button.Q<Label>().text = "";
            }
        }
    }

    

    public void NewGame()
    {
        SaveController.NewGame();
        ContinueGame();
    }

    public void ContinueGame()
    {
        Player.GetComponent<PlayerInteraction>().SetTankFocus(FindAnyObjectByType<TankController>());
    }

    public void OpenSaves()
    {
        MainMenu.SetActive(false);
        SaveMenu.SetActive(true);
    }

    public void OpenMainMenu()
    {
        MainMenu.SetActive(true);
        SaveMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
