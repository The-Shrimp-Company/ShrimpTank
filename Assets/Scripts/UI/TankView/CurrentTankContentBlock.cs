using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentTankContentBlock : ContentBlock
{
    private TankController _tank;
    private Shrimp[] _shrimp;
    private PlayerInteraction player;

    [SerializeField] private TextMeshProUGUI saleSign, destSign;

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
        if (!tank.destinationTank)
        {
            destSign.gameObject.SetActive(false);
            FontTools.SizeFont(destSign);
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
