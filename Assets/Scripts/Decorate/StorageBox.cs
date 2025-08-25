using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBox : Interactable
{
    [SerializeField] List<InventoryTabs> tabsToShow = new List<InventoryTabs>();

    public override void Action()
    {
        Store.decorateController.OpenShopInventory(tabsToShow);
        base.Action();
    }
}
