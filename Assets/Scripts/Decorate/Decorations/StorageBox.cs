using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBox : Interactable
{
    [SerializeField] List<InventoryTabs> tabsToShow = new List<InventoryTabs>();
    [SerializeField] List<ItemTags> itemTypesHeld = new List<ItemTags>();
    [SerializeField] string noItemsTooltip = "No Items Available";
    [SerializeField] string itemsTooltip = "View Inventory";

    private void Awake()
    {
        tooltipAlwaysVisible = true;
    }

    private void Update()
    {
        MouseHover();

        if (hovering)
        {
            if (HoldingItem())
            {
                GetComponent<ToolTip>().toolTip = "Put " + Store.player.GetComponent<HeldItem>().GetHeldItem().itemName + " Away";
                interactable = true;
            }
            else if (ContainsItems())
            {
                GetComponent<ToolTip>().toolTip = itemsTooltip;
                interactable = true;
            }
            else
            {
                GetComponent<ToolTip>().toolTip = noItemsTooltip;
                interactable = false;
            }
        }
    }


    public override void Action()
    {
        if (HoldingItem())
        {
            Store.player.GetComponent<HeldItem>().StopHoldingItem();
        }
        else
        {
            Store.decorateController.OpenShopInventory(tabsToShow);
        }

        base.Action();
    }


    private bool ContainsItems()
    {
        int items = 0;
        if (itemTypesHeld != null && itemTypesHeld.Count != 0)
        {
            for (int i = 0; i < itemTypesHeld.Count; i++)
            {
                items += Inventory.GetInventoryItemsWithTag(itemTypesHeld[i]).Count;
            }
        }

        if (items == 0) return false;
        else return true;
    }

    private bool HoldingItem()
    {
        Item i = Store.player.GetComponent<HeldItem>().GetHeldItem();
        if (i == null) return false;

        ItemSO so = Inventory.GetSOForItem(i);
        if (so == null || so.tags.Count == 0) return false;

        for (int x = 0; x < so.tags.Count; x++)
            if (itemTypesHeld.Contains(so.tags[x])) return true;

        return false;
    }
}
