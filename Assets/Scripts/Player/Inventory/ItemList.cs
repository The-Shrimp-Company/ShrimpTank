using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "ScriptableObjects/Item/Item List")]
public class ItemList : ScriptableObject
{
    [SerializeReference] public ItemSO[] items;
    [SerializeReference] public ItemSO[] roomDecorations;
    [SerializeReference] public ItemSO[] tankDecorations;
}