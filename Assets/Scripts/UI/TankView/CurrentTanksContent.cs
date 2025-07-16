using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentTanksContent : ContentPopulation
{
    private Shrimp[] _shrimp;

    private void Start()
    {
        CreateContent(Inventory.instance.activeTanks.Count - 1);
        int i = 0;
        Debug.Log(Inventory.instance.activeTanks.Count);
        foreach(ContentBlock content in contentBlocks)
        {
            if (_shrimp[0].tank == Inventory.instance.activeTanks[i]) i++;
            content.GetComponent<CurrentTankContentBlock>().SetTank(Inventory.instance.activeTanks[i]);
            content.GetComponent<CurrentTankContentBlock>().SetShrimp(_shrimp);
            content.SetText(Inventory.instance.activeTanks[i].tankName);
            i++;
        }
    }

    public void SetShrimp(Shrimp[] shrimp)
    {
        _shrimp = shrimp;
    }
}
