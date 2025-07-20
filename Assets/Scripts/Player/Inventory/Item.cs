using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTags
{
    Tank,
    Shelf,

    Food,
    Medicine,

    TankUpgrade,
    Filter,
    Heater,

    RoomDecoration,
    TankDecoration,
}

[System.Serializable]
public class Item
{
    public string itemName;
    public int quantity;
}


[System.Serializable]
public class MedicineItem : Item
{

}

[System.Serializable]
public class FoodItem : Item
{

}

[System.Serializable]
public class UpgradeItem : Item
{

}

[System.Serializable]
public class DecorationItem : Item
{

}