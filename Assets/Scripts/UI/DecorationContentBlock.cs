using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecorationContentBlock : ContentBlock
{
    private DecorationItemSO so;

    public Image buttonSprite;
    public Button button;

    public TMP_Text priceText;
    public TMP_Text ownedText;

    public Color inInventoryColour;
    public Color notInInventoryColour;
    public Color cannotAffordColour;
    public Color selectedColour;

    public void SetDecoration(DecorationItemSO d)
    {
        so = d;
    }

    public void Pressed()
    {
        //GameObject newitem = Instantiate(shrimpView);
        //UIManager.instance.OpenScreen(newitem.GetComponent<ScreenView>());
        //newitem.GetComponent<ShrimpView>().Populate(_shrimp);
        //_shrimp.GetComponentInChildren<ShrimpCam>().SetCam();
        //newitem.GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
        //newitem.GetComponent<Canvas>().planeDistance = 1;
        //UIManager.instance.SetCursorMasking(false);
    }
}
