using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food Item", menuName = "ScriptableObjects/Item/Food Item")]
public class FoodItemSO : ItemSO
{
    [SerializeReference] public GameObject foodPrefab;
}