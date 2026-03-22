using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveButtons : MonoBehaviour
{
    private WorldSpaceMainMenu menu;
    private SaveController SaveController;
    private GameObject Player;

    private void Start()
    {
        menu = transform.GetComponentInParent<WorldSpaceMainMenu>();
        if (menu)
        {
            SaveController = menu.SaveController;
            Player = menu.Player;
        }
    }

    public void LoadGame()
    {
        SaveManager.currentSaveFile = name;
        if (SaveManager.TryLoadGame(name))
        {
            SaveController.LoadGame(name);
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
