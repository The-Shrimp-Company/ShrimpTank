using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Medicine Item", menuName = "ScriptableObjects/Item/Medicine Item")]
public class MedicineItemSO : ItemSO
{
    [SerializeReference] public IllnessSymptoms[] symptoms;
    [SerializeReference][Range(0, 100)] public float strength = 100;  // 100 Will allways fully cure
}