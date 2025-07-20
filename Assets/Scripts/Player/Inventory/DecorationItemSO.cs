using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Decoration Item", menuName = "ScriptableObjects/Item/Decoration Item")]
public class DecorationItemSO : ItemSO
{
    [SerializeReference] public GameObject decorationPrefab;
}