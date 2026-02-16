using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceMainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject Player;

    [SerializeField]
    private SaveController SaveController;


    // Start is called before the first frame update
    void Start()
    {
        Camera cam = Player.GetComponentInChildren<Camera>();
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

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
