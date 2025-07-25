using SaveLoadSystem;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    

    public List<string> flags = new();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        if (!SaveManager.startNewGame)
        {
            instance.flags = SaveManager.CurrentSaveData.tutorialFlags ?? new();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (DataStore.Assigned)
        {
            Debug.Log(DataStore.StoreName);
        }
        else
        {
            Debug.Log(SaveManager.CurrentSaveData.storeName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!flags.Contains("openTablet"))UIManager.instance.SendNotification("Press E to open tablet");
    }

    public void OpenedTablet()
    {
        if (!flags.Contains("openTablet"))
        {
            flags.Add("openTablet");
            UIManager.instance.SendNotification("");
        }
    }
}
