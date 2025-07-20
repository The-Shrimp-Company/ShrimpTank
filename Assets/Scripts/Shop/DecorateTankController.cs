using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class DecorateTankController
{
    private static TankGrid currentTank;

    public static void StartDecorating(TankController t)
    {
        if (t == null) return;
        if (t.tankViewScript == null) return;
        currentTank = t.tankGrid;

        GameObject newMenu = GameObject.Instantiate(t.tankViewScript.tankDecorateView, t.transform);
        UIManager.instance.OpenScreen(newMenu.GetComponent<ScreenView>());
        newMenu.GetComponent<TankDecorateViewScript>().UpdateContent();
        newMenu.GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
        newMenu.GetComponent<Canvas>().planeDistance = 1;
        UIManager.instance.SetCursorMasking(false);
    }
}
