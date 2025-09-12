using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Basic Item", menuName = "ScriptableObjects/Item/Basic Item")]
public class ItemSO : ScriptableObject
{
    [SerializeReference] public string itemName;
    [SerializeReference] public Sprite itemImage;
    [SerializeReference] public float purchaseValue;
    [SerializeReference] public int purchaseQuantity = 1;
    [SerializeReference][TextArea(1, 10)] public string itemDescription;
    //[SerializeReference] public float sellValue;
    [SerializeReference] public List<ItemTags> tags = new List<ItemTags>();
    [SerializeReference] public GameObject heldItemPrefab;
}