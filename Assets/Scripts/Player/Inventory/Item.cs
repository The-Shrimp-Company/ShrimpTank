using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public int quantity;


    public int value;
}


[System.Serializable]
public class MedicineItem : Item
{
    public IllnessSymptoms[] symptoms;
    [Range(0, 100)] public float strength = 100;  // 100 Will allways fully cure


    //public Medicine(string newName, int newValue, IllnessSymptoms symptom, int setStrength, int newQuantity = 0) : base(newName, newValue, newQuantity)
    //{
    //    symptoms = new IllnessSymptoms[] { symptom };
    //    strength = setStrength;
    //}
}


[System.Serializable]
public class UpgradeItem : Item
{


}
