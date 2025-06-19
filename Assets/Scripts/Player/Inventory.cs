using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Items
{
    public static Item SmallTank = new Item("Small Tank", 50);
    public static Item Shelf = new Item("Shelf", 100);
    public static Item AlgaeWafer = new Item("Algae Wafer", 10);
    public static Item FoodPellet = new Item("Food Pellet", 5);
    public static Item MedSmallHead = new Medicine("Small Head Pills", 5, IllnessSymptoms.BodySize, 100, 2);
    public static Item MedBubble = new Medicine("Bubble Be Gone", 5, IllnessSymptoms.Bubbles, 100, 2);
    public static Item MedVibrance = new Medicine("Vibrancee", 5, IllnessSymptoms.Discolouration, 100, 2);
    public static Item UpHeat0 = new Upgrade("H001");
    public static Item UpHeat1 = new Upgrade("H002");
    public static Item UpFilt0 = new Upgrade("F001");
    public static Item UpFilt1 = new Upgrade("F002");
    public static Item DecorGM = new Upgrade("D001");
    public static Item DecorDL = new Upgrade("D002");
    public static Item DecorRG = new Upgrade("D003");
    public static Item DecorWR = new Upgrade("D004");
    public static Item DecorLP = new Upgrade("D005");
}


public class Item
{
    public string itemName;
    public int value;
    public int quantity;

    public Item(string newName, int newValue, int newquantity = 0)
    {
        itemName = newName;
        value = newValue;
        quantity = newquantity;
    }

    public static implicit operator string(Item item)
    {
        return item.itemName;
    }
}


public class Inventory
{
    private List<Item> newInventory = new List<Item>();

    public static Inventory instance = new Inventory();

    public List<TankController> activeTanks { get; private set; } = new List<TankController>();

    public Inventory()
    {
        Initialize();
    }

    public void Initialize()
    {
        activeTanks = new List<TankController>();
        newInventory = new List<Item>();
    }

    public void AddItem(Item newItem, int quantity = 1)
    {
        if (newItem == null) return;

        for(int i = 0; i < newInventory.Count; i++)
        {
            if (newInventory[i] == newItem.itemName)
            {
                newInventory[i].quantity += quantity;
                return;
            }
        }
        newItem.quantity = quantity;
        newInventory.Add(newItem);
    }

    public bool RemoveItem(Item item, int quantity = 1)
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
    /*
    public Item GetItemFromName(string name)
    {
        Item item = null;

        foreach(Item i in Items)
        {
            
        }

        // Examining the name of all variables in a C# object
        // In this case, we'll list the variable in this NameToBlorp
        // class
        System.Reflection.PropertyInfo[] rProps = Items.GetType().GetProperties();
        foreach (System.Reflection.PropertyInfo rp in rProps)
            Debug.Log(rp.Name);

        // Getting the info of a specific variable name.
        // This gives us the ability to read/write it
        System.Reflection.PropertyInfo propName = this.GetType().GetProperty("name");
        if (propName != null)
        {
            // The PropertyInfo isn't the actual variable, just the "idea" of
            // the variable existing in an object.
            // 
            // It needs to be used in conjunction with the object...
            // Equivalent of this.name = "blorp"
            propName.SetValue(
                this, // So we specify who owns the object
                "blorp", // A C# object as the value, will be casted (if possible)
                null
              );

            // And GetValue can be used in a similar fassion.
            // Equivalent of Debug.log( "..." + this.name )
            Debug.Log("The name is " + propName.GetValue(this, null));
        }
    }

  
     * // Examining the name of all variables in a C# object
		// In this case, we'll list the variable in this NameToBlorp
		// class
		System.Reflection.PropertyInfo [] rProps = this.GetType().GetProperties();
		foreach(System.Reflection.PropertyInfo rp in rProps )
			Debug.Log( rp.Name );

		// Getting the info of a specific variable name.
		// This gives us the ability to read/write it
		System.Reflection.PropertyInfo propName = this.GetType().GetProperty( "name" );
		if( propName != null )
		{
			// The PropertyInfo isn't the actual variable, just the "idea" of
			// the variable existing in an object.
			// 
			// It needs to be used in conjunction with the object...
			// Equivalent of this.name = "blorp"
			propName.SetValue(	 
				this, // So we specify who owns the object
			    "blorp", // A C# object as the value, will be casted (if possible)
			    null
          	);

			// And GetValue can be used in a similar fassion.
			// Equivalent of Debug.log( "..." + this.name )
			Debug.Log( "The name is " + propName.GetValue( this, null ) );
		}
     */
    public int GetItemCount() { return  newInventory.Count; }

    public static int GetItemQuant(Item itemCheck)
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


    public static List<Item> GetInventory()
    {
        return instance.newInventory;
    }

    public static bool Contains(Item itemCheck)
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
}
