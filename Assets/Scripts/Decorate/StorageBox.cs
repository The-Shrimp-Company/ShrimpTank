using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBox : Interactable
{


    public override void Action()
    {
        DecorateShopController.Instance.StartDecorating();
        base.Action();
    }
}
