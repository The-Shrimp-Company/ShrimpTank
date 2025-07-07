using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class UpgradeScreen : ScreenView
{
    protected override void Start()
    {
        shelves = transform.parent.GetComponent<ShelfRef>().GetShelves();
        base.Start();
    }



    public void BuyItem(string itemName)
    {
        ItemSO so = Inventory.GetSOUsingName(itemName);
        if (so == null)
        {
            Debug.LogWarning("Item with name " + itemName + " cannot be found through the shop");
            return;
        }

        if (Money.instance.WithdrawMoney(so.purchaseValue))
        {
            Inventory.AddItem(Inventory.GetItemUsingSO(so), so.purchaseQuantity);
        }
    }

    public void BuyShelf(string itemName)
    {
        ItemSO so = Inventory.GetSOUsingName(itemName);
        if (so == null)
        {
            Debug.LogWarning("Shelf with name " + itemName + " cannot be found through the shop");
            return;
        }

        if (Money.instance.WithdrawMoney(so.purchaseValue))
        {
            shelves.SpawnNextShelf();
        }
    }
}
