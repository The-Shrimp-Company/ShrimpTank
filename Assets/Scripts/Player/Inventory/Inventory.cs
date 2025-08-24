using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Inventory
{
    public static Inventory instance = new Inventory();

    private ItemSO[] loadedItemList;
    private List<Item> inventory = new List<Item>();
    public List<TankController> activeTanks { get; private set; } = new List<TankController>();


    private static int maxItemCount = 999;


    public void Initialize(Item[] saveData = null)
    {
        LoadItemsFromResources();  // Gets a list of all items from the resources folder

        GenerateInventory();  // Initialises inventory with all items at 0 quantity

        if (saveData != null)
            LoadInventoryFromFile(saveData);  // Changes values such as quantity using the saved data

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
                item = new MedicineItem();

            else if (loadedItemList[i] as UpgradeItemSO != null)
                item = new UpgradeItem();

            else if (loadedItemList[i] as DecorationItemSO != null)
                item = new DecorationItem();

            else if (loadedItemList[i] as FoodItemSO != null)
                item = new FoodItem();

            else
                item = new Item();

            item.itemName = loadedItemList[i].itemName;
            item.quantity = 0;

            inventory.Add(item);
        }
    }

    private void LoadInventoryFromFile(Item[] saveData)
    {
        if (inventory == null || inventory.Count == 0) return;
        if (saveData == null || saveData.Length == 0) return;

        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            if (inventory[i] == null) continue;
            for (int x = saveData.Length - 1; x >= 0; x--)
            {
                if (saveData[x] == null) continue;
                if (saveData[x].itemName == inventory[i].itemName)
                {
                    inventory[i] = saveData[x];
                    break;
                }
            }
        }




        //if (loadedItemList == null || loadedItemList.Length == 0) return;

        //if (inventory == null) inventory = new List<Item>();
        //else inventory.Clear();

        //for (int i = 0; i < loadedItemList.Length; i++)
        //{
        //    Item item;
        //    if (loadedItemList[i] as MedicineItemSO != null)
        //        item = new MedicineItem();

        //    else if (loadedItemList[i] as UpgradeItemSO != null)
        //        item = new UpgradeItem();

        //    else if (loadedItemList[i] as DecorationItemSO != null)
        //        item = new DecorationItem();

        //    else if (loadedItemList[i] as FoodItemSO != null)
        //        item = new FoodItem();

        //    else
        //        item = new Item();

        //    //item = Array.Find(saveData, item => item.itemName == loadedItemList[i].itemName);

        //    inventory.Add(item);
        //}
    }

    public static void AddItem(Item newItem, int quantity = 1)
    {
        if (newItem == null) return;
        AddItem(newItem.itemName, quantity);
    }

    public static void AddItem(string itemName, int quantity = 1)
    {
        if (itemName == null) return;

        for (int i = 0; i < instance.inventory.Count; i++)
        {
            if (instance.inventory[i].itemName == itemName)
            {
                instance.inventory[i].quantity += quantity;

                if (instance.inventory[i].quantity > maxItemCount)
                    instance.inventory[i].quantity = maxItemCount;

                return;
            }
        }
    }

    public static bool RemoveItem(Item item, int quantity = 1)  // UPDATE
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


    public static int GetItemCount() { return GetInventory().Count; }

    public static int GetItemQuantity(Item itemCheck) { return GetItemQuantity(itemCheck.itemName); }
    public static int GetItemQuantity(string itemName)
    {
        Item item = GetItemUsingName(itemName);
        if (item != null) return item.quantity;
        else return 0;
    }

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

    public static List<Item> GetInventory(bool ownedItems = true, bool sort = true) 
    {
        List<Item> items = new List<Item>();

        foreach (Item item in instance.inventory)
        {
            if (ownedItems && item.quantity <= 0) continue;  // Skip this item if it is not owned

            items.Add(item);
        }

        if (sort) items = SortItemsByQuantityThenName(items);

        return items;
    }

    public static List<Item> GetInventoryItemsWithTag(ItemTags tag) { return FilterItemsWithTag(GetInventory(), tag); }

    public static List<Item> FilterItemsWithTag(List<Item> items, ItemTags tag)
    {
        List<Item> filteredItems = new List<Item>();

        foreach (Item item in items)
        {
            if (GetSOForItem(item).tags.Contains(tag))
                filteredItems.Add(item);
        }

        return filteredItems;
    }

    public static List<Item> GetInventoryItemsWithTags(List<ItemTags> tags) { return FilterItemsWithTags(GetInventory(), tags); }

    public static List<Item> FilterItemsWithTags(List<Item> items, List<ItemTags> tags)
    {
        if (tags.Count == 0) return items;

        List<Item> filteredItems = new List<Item>();

        foreach (Item item in items)
        {
            foreach (ItemTags tag in tags)
            {
                if (GetSOForItem(item).tags.Contains(tag))
                {
                    filteredItems.Add(item);
                    break;
                }
            }
        }

        return filteredItems;
    }

    public static List<Item> SortItemsByQuantityThenName(List<Item> items)  // Sorts a list of items by quantity and then alphabetically
    {
        List<Item> sortedItems = items.OrderByDescending(i => i.quantity).ThenBy(n => n.itemName).ToList();
        return sortedItems;
    }

    public static List<Item> SortItemsByQuantity(List<Item> items)   // Sorts a list of items by quantity in descending order
    {
        List<Item> sortedItems = items.OrderByDescending(i => i.quantity).ToList();
        return sortedItems;
    }

    public static List<Item> SortItemsByName(List<Item> items)  // Sorts a list of items from A-Z
    {
        List<Item> sortedItems = items.OrderBy(i => i.itemName).ToList();
        return sortedItems;
    }

    public static ItemSO[] GetLoadedItems() { return instance.loadedItemList; }  // Returns all item scriptable objects
    public static int GetMaxItemCount() { return maxItemCount; }

    public static void ClearInventory()
    {
        foreach(Item item in instance.inventory)
        {
            item.quantity = 0;
        }
    }
}