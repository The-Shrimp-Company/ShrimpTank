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

    Floor,
    Wall,
    Ceiling,
    OnShelf,
    Floating,

    Garage,
    LivingRoom,

    Shrimp,
    Tools,
    Supplies,

    Small,
    Medium,
    Large,

    Lighting,
    Storage,
    Water,
}


public enum PlacementSurfaces
{
    Ground,
    Shelf,
    Water,
    Wall,
    Ceiling,
    Air
}

public enum ItemSize
{
    Small,
    Medium,
    Large,
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