using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyScreen : ScreenView
{
    

    protected override void Start()
    {
        base.Start();
    }

    public bool BuyShrimp(ShrimpStats s)
    {
        return Store.SpawnShrimp(s, EconomyManager.instance.GetShrimpValue(s));
    }
}
