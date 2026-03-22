using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldSpaceMainMenu : MonoBehaviour
{
    public GameObject Player;

    public SaveController SaveController;

    [SerializeField]
    private GameObject MainMenu;
    [SerializeField]
    private GameObject SaveMenu;

    [SerializeField]
    private List<SaveButtons> saveFiles;


    // Start is called before the first frame update
    void Start()
    {
        Camera cam = Player.GetComponentInChildren<Camera>();
        foreach (SaveButtons button in saveFiles)
        {
            
            if (SaveManager.TryLoadGame(button.name))
            {
                //SaveManager.LoadGame(button.name);
                button.GetComponentInChildren<TextMeshProUGUI>().text = button.name;
                //button.Q<Label>().text = TimeManager.DateFromTime(SaveManager.CurrentSaveData.totalTime);
            }
            else
            {
                button.GetComponentInChildren<TextMeshProUGUI>().text = "Empty File";
                //button.Q<Label>().text = "";
            }
        }
    }

    
    

    public void NewGame()
    {
        SaveManager.currentSaveFile = null;
        SaveManager.gameInitialized = false;
        SaveManager.startNewGame = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
