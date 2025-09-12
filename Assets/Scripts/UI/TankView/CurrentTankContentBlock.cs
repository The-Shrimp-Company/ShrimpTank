using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CurrentTankContentBlock : ContentBlock
{
    private TankController _tank;
    private Shrimp[] _shrimp;
    private PlayerInteraction player;

    [SerializeField] private TextMeshProUGUI saleSign, warningSign;

    public void Start()
    {

        player = GameObject.Find("Player").GetComponent<PlayerInteraction>();
    }

    public void SetTank(TankController tank)
    {
        _tank = tank;
        if (!tank.openTank) 
        {
            saleSign.gameObject.SetActive(false);
            FontTools.SizeFont(saleSign);
        }

        foreach (Shrimp shrimp in _shrimp)
        {
            if (Mathf.Abs(tank.waterAmmonium - shrimp.stats.ammoniaPreference) > 10 &&
            Mathf.Abs(tank.waterSalt - shrimp.stats.salineLevel) > 10 &&
            Mathf.Abs(tank.waterPh - shrimp.stats.PhPreference) > 2 &&
            Mathf.Abs(tank.waterTemperature - shrimp.stats.temperaturePreference) > 10)
            {
                warningSign.gameObject.SetActive(true);
            }
        }
    }

    public void SetShrimp(Shrimp[] shrimp) { _shrimp = shrimp; }

    public void Click()
    {
        foreach(Shrimp shrimp in _shrimp)
        {
            if(shrimp != null)
            {
                _tank.MoveShrimp(shrimp);
            }
        }
        player.OnExitView();
    }
}
