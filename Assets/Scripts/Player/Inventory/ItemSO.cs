using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "ItemList", menuName = "ScriptableObjects/Item/Item List")]
public class ItemList : ScriptableObject
{
    [SerializeReference] public ItemSO[] items;
}


[CreateAssetMenu(fileName = "Basic Item", menuName = "ScriptableObjects/Item/Basic Item")]
public class ItemSO : ScriptableObject
{
    [SerializeReference] public string itemName;
    [SerializeReference] public int purchaseValue;
    [SerializeReference] public int purchaseQuantity = 1;
    [SerializeReference] public int sellValue;
    [SerializeReference] public List<ItemTags> tags = new List<ItemTags>();
}


[CreateAssetMenu(fileName = "Medicine Item", menuName = "ScriptableObjects/Item/Medicine Item")]
public class MedicineItemSO : ItemSO
{
    [SerializeReference] public IllnessSymptoms[] symptoms;
    [SerializeReference][Range(0, 100)] public float strength = 100;  // 100 Will allways fully cure
}


[CreateAssetMenu(fileName = "Upgrade Item", menuName = "ScriptableObjects/Item/Upgrade Item")]
public class UpgradeItemSO : ItemSO
{
    
}


[CreateAssetMenu(fileName = "Food Item", menuName = "ScriptableObjects/Item/Food Item")]
public class FoodItemSO : ItemSO
{
    [SerializeReference] public GameObject foodPrefab;
}