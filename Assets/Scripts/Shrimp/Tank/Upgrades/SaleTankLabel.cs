using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaleTankLabel : TankUpgrade
{
    public override void CreateUpgrade(UpgradeItemSO u, TankController t)
    {
        if (t.openTank != true)
        {
            t.toggleTankOpen();
        }

        gameObject.GetComponentInChildren<Button>().onClick.AddListener(() => ClickOnSign());

        base.CreateUpgrade(u, t);
    }

    public void ClickOnSign()
    {      
        tank.upgradeController.RemoveUpgrade(UpgradeTypes.Label);
    }

    public override void UpdateUpgrade(float elapsedTime)
    {    
        base.UpdateUpgrade(elapsedTime);
    }

    public override void RemoveUpgrade()
    {
        tank.toggleTankOpen(false);
        base.RemoveUpgrade();
    }
}
