using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveButtons : MonoBehaviour
{
    private SaveController SaveController;
    private GameObject Player;

    private void Start()
    {
        SaveController = transform.GetComponentInParent<WorldSpaceMainMenu>().SaveController;
        Player = transform.GetComponentInParent<WorldSpaceMainMenu>().Player;
    }

    public void LoadGame()
    {
        SaveManager.currentSaveFile = name;
        if (SaveManager.TryLoadGame(name))
        {
            SaveController.LoadGame(name);
        }
        else
        {
            SaveController.NewGame();
            SaveController.SaveGame(name);
        }
        //Player.GetComponent<PlayerInteraction>().SetTankFocus(FindAnyObjectByType<TankController>());
    }

    public void SaveGame()
    {
        SaveManager.currentSaveFile = name;
        SaveController.SaveGame(name);
        GetComponentInChildren<TextMeshProUGUI>().text = name;
    }
}
