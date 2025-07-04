using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;


public class Inventory
{
    public static Inventory instance = new Inventory();

    private ItemSO[] loadedItemList;
    private List<Item> inventory = new List<Item>();

    public List<TankController> activeTanks { get; private set; } = new List<TankController>();

    public void Initialize()
    {
        LoadItemsFromResources();
        GenerateInventory();
        activeTanks = new List<TankController>();
    }

    private void LoadItemsFromResources()
    {
        loadedItemList = ((ItemList)Resources.Load("ItemList")).items;
        if (loadedItemList == null || loadedItemList.Length == 0)
            Debug.LogError("Inventory items failed to load from resources");
    }

    private void GenerateInventory()
    {
        if (loadedItemList == null || loadedItemList.Length == 0) return;

        if (inventory == null) inventory = new List<Item>();
        else inventory.Clear();

        for(int i = 0; i < loadedItemList.Length; i++)
        {
            Item item;
            if (loadedItemList[i] as MedicineItemSO != null)
            {
                item = new MedicineItem();
                Debug.Log("Medicine");
            }

            else if (loadedItemList[i] as UpgradeItemSO != null)
            {
                item = new UpgradeItem();
                Debug.Log("Upgrade");
            }

            else
            {
                item = new Item();
                Debug.Log("Basic");
            }

            item.itemName = loadedItemList[i].itemName;
            item.quantity = 0;

            inventory.Add(item);
        }
    }

    public void AddItem(Item newItem, int quantity = 1)
    {
        if (newItem == null) return;
        AddItem(newItem.itemName, quantity);
    }

    public void AddItem(string itemName, int quantity = 1)
    {
        if (itemName == null) return;

        for (int i = 0; i < instance.inventory.Count; i++)
        {
            if (instance.inventory[i].itemName == itemName)
            {
                instance.inventory[i].quantity += quantity;
                return;
            }
        }
    }

    public bool RemoveItem(Item item, int quantity = 1)  // UPDATE
    {
        if (item == null) return false;
        return RemoveItem(item.itemName, quantity);
    }

    public static bool RemoveItem(string itemName, int quantity = 1)
    {
        foreach(Item item in instance.inventory)
        {
            if(item.itemName == itemName)
            {
                if(item.quantity >= quantity)
                {
                    item.quantity -= quantity;

                    return true;
                }
            }
        }
        return false;
    }


    public static int GetItemCount() { return instance.inventory.Count; }

    public static int GetItemQuantity(Item itemCheck) { return GetItemQuantity(itemCheck.itemName); }
    public static int GetItemQuantity(string itemName)
    {
        Item item = GetItemUsingName(itemName);
        if (item != null) return item.quantity;
        else return 0;
    }

    public static List<Item> GetInventory() { return instance.inventory; }

    public static bool HasItem(Item itemCheck) { return HasItem(itemCheck.itemName); }
    public static bool HasItem(string itemName)
    {
        Item item = GetItemUsingName(itemName);
        if (item != null && item.quantity > 0)
            return true;

        return false;
    }

    public static ItemSO GetSOForItem(Item item)
    {
        ItemSO so = GetSOUsingName(item.itemName);
        if (so != null)
            return so;
        else
            return null;
    }

    public static Item GetItemUsingSO(ItemSO so)
    {
        Item item = GetItemUsingName(so.itemName);
        if (item != null)
            return item;
        else
            return null;
    }

    public static ItemSO GetSOUsingName(string name)
    {
        foreach (ItemSO so in instance.loadedItemList)
        {
            if (so.itemName == name)
            {
                return so;
            }
        }

        Debug.LogWarning("SO for item " + name + " could not be found");
        return null;
    }

    public static Item GetItemUsingName(string name)
    {
        foreach (Item item in instance.inventory)
        {
            if (item.itemName == name)
            {
                return item;
            }
        }

        Debug.LogWarning("Item " + name + " could not be found");
        return null;
    }
}
