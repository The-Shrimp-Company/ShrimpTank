using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VetScreen : ScreenView
{

    public void BuyItem(string itemName)
    {
        ItemSO so = Inventory.GetSOUsingName(itemName);
        if (so == null)
        {
            Debug.LogWarning("Item with name " + itemName + " cannot be found through the vet screen");
            return;
        }

        if (Money.instance.WithdrawMoney(so.purchaseValue))
        {
            Inventory.AddItem(Inventory.GetItemUsingSO(so), so.purchaseQuantity);
        }
    }
}



