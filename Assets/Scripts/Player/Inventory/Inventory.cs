using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory
{
    private List<Item> newInventory = new List<Item>();

    public static Inventory instance = new Inventory();

    private ItemSO[] loadedItemList;
    private List<Item> inventory = new List<Item>();

    public List<TankController> activeTanks { get; private set; } = new List<TankController>();

    public Inventory()
    {

    }

    public void Initialize()
    {
        LoadItemsFromResources();
        GenerateInventory();
        activeTanks = new List<TankController>();
        newInventory = new List<Item>();
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

    public void AddItem(Item newItem, int quantity = 1)  // UPDATE
    {
        if (newItem == null) return;

        for(int i = 0; i < newInventory.Count; i++)
        {
            //if (newInventory[i] == newItem.itemName)
            //{
            //    newInventory[i].quantity += quantity;
            //    return;
            //}
        }
        newItem.quantity = quantity;
        newInventory.Add(newItem);
    }

    public bool RemoveItem(Item item, int quantity = 1)  // UPDATE
    {
        foreach(Item i in newInventory)
        {
            if(i.itemName == item.itemName)
            {
                if(i.quantity >= quantity)
                {
                    i.quantity -= quantity;
                    if(i.quantity <= 0)
                    {
                        newInventory.Remove(i);
                    }
                    return true;
                }
            }
        }
        return false;

    }


    public int GetItemCount() { return  newInventory.Count; }  // UPDATE

    public static int GetItemQuant(Item itemCheck)  // UPDATE
    {
        foreach(Item i in instance.newInventory)
        {
            if(i.itemName == itemCheck.itemName)
            {
                return i.quantity;
            }
        }
        return 0;
    }


    public static List<Item> GetInventory()  // UPDATE
    {
        return instance.newInventory;
    }

    public static bool Contains(Item itemCheck)  // UPDATE
    {
        foreach(Item i in instance.newInventory)
        {
            if(itemCheck.itemName == i.itemName)
            {
                return true;
            }
        }
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
}
