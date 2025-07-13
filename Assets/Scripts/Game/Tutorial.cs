using SaveLoadSystem;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    public struct TutorialFlags
    {
        public bool openTablet;
        public bool boughtShrimp;
        public bool activeAccount;
    }

    public TutorialFlags flags = new TutorialFlags();

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
            instance.flags.activeAccount = true;
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
        if(!flags.openTablet)UIManager.instance.SendNotification("Press E to open tablet");
    }

    public void OpenedTablet()
    {
        if (!flags.openTablet)
        {
            flags.openTablet = true;
            UIManager.instance.SendNotification("");
        }
    }
}
