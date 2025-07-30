using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentTankScreen : ScreenView
{

    [SerializeField]
    private CurrentTanksContent content;

    protected override void Start()
    {
        base.Start();

    }


    public void SetShrimp(Shrimp shrimp)
    {
        content.SetShrimp(new Shrimp[] { shrimp });
    }

    public void SetShrimp(Shrimp[] shrimp)
    {
        content.SetShrimp(shrimp);
    }
}
