using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : Item
{
    public UpgradeSO upgrade;
    public bool decor = false;

    //public Upgrade(string newName, bool isDecor = false, int newValue = 0, int newQuantity = 0) : base(newName, newValue, newQuantity)
    //{
    //    foreach (UpgradeSO so in UpgradeList.instance.Upgrades)
    //    {
    //        if (so.ID == newName)
    //        {
    //            upgrade = so;
    //            value = (int)so.cost;
    //            itemName = so.upgradeName;
    //            decor = isDecor;
    //            return;
    //        }
    //    }
    //    Debug.LogWarning("An item is missing a SO. Are you sure you spelt everything right, or did you forget to add the SO to the upgrade list?");
    //}
}
