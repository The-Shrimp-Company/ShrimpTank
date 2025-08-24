using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDebug : MonoBehaviour
{
    public bool giveMoneyButton = false;
    private CameraLookCheck lookCheck;


    // Start is called before the first frame update
    void Start()
    {
        enabled = Debug.isDebugBuild;
        if (enabled)
        {
            lookCheck = GetComponentInChildren<CameraLookCheck>();
        }
    }

    public void OnTestingFunctions()
    {
        if (giveMoneyButton)
        {
            Money.instance.AddMoney(20);
            Reputation.AddReputation(10);
        }
    }


    public void OnMouseMove(InputValue val)
    {
        Debug.Log(val.Get<Vector2>());
    }
    /* Old Function to interact with tanks on click
    public void OnSpawnShrimp()
    {
        GameObject target = lookCheck.LookCheck(1, "Tanks");

        if (target != null)
        {

            TankController tankController = target.GetComponent<TankController>();

            shelves.SwitchSaleTank(tankController);
        }
    }
    */

    /*
    public void OnAddMoney()
    {
        Money.instance.AddMoney(5);
    }
    */

    public void OnAddRequest()
    {
        CustomerManager.Instance.MakeRequest();
    }
}
