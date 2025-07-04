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



[CreateAssetMenu(fileName = "ItemList", menuName = "ScriptableObjects/Item/Item List")]
public class ItemList : ScriptableObject
{
    public ItemSO[] items;
}


[CreateAssetMenu(fileName = "Basic Item", menuName = "ScriptableObjects/Item/Basic Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public int purchaseValue;
    public int sellValue;
    public List<ItemTags> tags = new List<ItemTags>();
    [HideInInspector] public int quantity;
}


[CreateAssetMenu(fileName = "Medicine Item", menuName = "ScriptableObjects/Item/Medicine Item")]
public class MedicineItemSO : ItemSO
{
    public IllnessSymptoms[] symptoms;
    [Range(0, 100)] public float strength = 100;  // 100 Will allways fully cure
}


[CreateAssetMenu(fileName = "Upgrade Item", menuName = "ScriptableObjects/Item/Upgrade Item")]
public class UpgradeItemSO : ItemSO
{
    
}