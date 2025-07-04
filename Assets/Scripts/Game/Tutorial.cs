using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial instance;

    public struct TutorialFlags
    {
        public bool openTablet;
        public bool boughtShrimp;
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
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
