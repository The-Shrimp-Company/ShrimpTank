using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeScreen : ScreenView
{
    protected override void Start()
    {
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
            if (so.tags.Contains(ItemTags.Food))
            {
                PlayerStats.stats.timesBoughtFood++;
            }
        }
    }
}
