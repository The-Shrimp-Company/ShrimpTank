using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankContentBlock : ContentBlock
{
    private Shrimp _shrimp;
    [SerializeField]
    private GameObject shrimpView;

    public Button main, checkbutton;

    [SerializeField] private Image primaryColour, secondaryColour;

    public void SetShrimp(Shrimp shrimp)
    {
        _shrimp = shrimp;
        primaryColour.color = GeneManager.instance.GetTraitSO(_shrimp.stats.primaryColour.activeGene.ID).colour;
        secondaryColour.color = GeneManager.instance.GetTraitSO(_shrimp.stats.secondaryColour.activeGene.ID).colour;
    }

    public void Pressed()
    {
        GameObject newitem = Instantiate(shrimpView);
        UIManager.instance.OpenScreen(newitem.GetComponent<ScreenView>());
        newitem.GetComponent<ShrimpView>().Populate(_shrimp);
        _shrimp.GetComponentInChildren<ShrimpCam>().SetCam();
        newitem.GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
        newitem.GetComponent<Canvas>().planeDistance = 1;
        UIManager.instance.SetCursorMasking(false);
    }
}
