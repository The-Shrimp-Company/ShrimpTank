using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Decoration Item", menuName = "ScriptableObjects/Item/Decoration Item")]
public class DecorationItemSO : ItemSO
{
    [SerializeReference] public GameObject decorationPrefab;

    [Header("Info")]
    [SerializeReference] public ItemSize itemSize;  // A rough description of how big the item is [1 tile is small, 2-9 is medium, anything bigger is large ]

    [Header("Placement")]
    [SerializeField] public Vector3 gridSnapOffset;  // Can be used to make sure the item is at the correct height or can be placed in the middle of a 2x2 square of nodes
    [SerializeField] public Vector3 rotationAxis = Vector3.zero;  // Used to tell the game which direction to rotate the object
    [SerializeReference] public List<PlacementSurfaces> placementSurfaces = new List<PlacementSurfaces>();  // Where it can be placed

    [Header("Shelf")]
    [SerializeReference] public bool shelf;  // Whether objects can be placed on top
    [SerializeReference] public Vector3 shelfItemOffset;  // Where items placed on the shelf appear
    [SerializeReference] public Vector3 shelfItemRotationOffset;

    [Header("Water")]
    [SerializeReference] public bool water;  // Whether floating objects can be placed on top
}