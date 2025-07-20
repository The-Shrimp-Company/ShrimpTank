using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecorationContentBlock : ContentBlock
{
    private DecorationItemSO so;

    public Button main;

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
