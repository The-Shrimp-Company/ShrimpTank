using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Decoration Item", menuName = "ScriptableObjects/Item/Decoration Item")]
public class DecorationItemSO : ItemSO
{
    [SerializeReference] public GameObject decorationPrefab;

    [Header("Placement")]
    [SerializeReference] public FloatingItem canFloat;  // Whether it can be placed in water
    [SerializeReference] public bool canSitOnShelf;  // Whether it can be placed on a shelf (Includes other surfaces such as tables)

    [Header("Placement")]
    [SerializeReference] public bool shelf;  // Whether objects can be placed on top
}