using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Decoration Item", menuName = "ScriptableObjects/Item/Decoration Item")]
public class DecorationItemSO : ItemSO
{
    [SerializeReference] public GameObject decorationPrefab;

    [Header("Placement")]
    [SerializeField] public Vector3 gridSnapOffset;  // Can be used to make sure the item is at the correct height or can be placed in the middle of a 2x2 square of nodes
    [SerializeField] public Vector3 rotationAxis = Vector3.zero;  // Used to tell the game which direction to rotate the object
    [SerializeReference] public FloatingItem canFloat;  // Whether it can be placed in water
    [SerializeReference] public bool canSitOnShelf;  // Whether it can be placed on a shelf (Includes other surfaces such as tables)

    [Header("Placement")]
    [SerializeReference] public bool shelf;  // Whether objects can be placed on top
}