using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Upgrade Item", menuName = "ScriptableObjects/Item/Upgrade Item")]
public class UpgradeItemSO : ItemSO
{
    //[SerializeReference] public string ID;  // Matches the trait with the saved genes, do not change once we are able to save the game
    [SerializeReference] public UpgradeTypes upgradeType;

    [SerializeReference] public GameObject upgradePrefab;

    [SerializeReference] public float breakRate = 5;
    [SerializeReference] public float repairCost = 15;
    [SerializeReference] public float energyCost = 0;

    [Header("Filter")]
    [SerializeReference] public float filterCapacity;

    [Header("Heater")]
    [SerializeReference] public float heaterOutput;
    [SerializeReference] public Thermometer thermometer;
}


public enum UpgradeTypes
{
    Filter,
    Heater,
    Camera,
    PhIndicator,
    MineralRegulator,
    FoodDispenser,
    Label,
    Decorations
}


public enum Thermometer
{
    NoThermometer,
    ThermometerOnly,
    AutomaticThermometer
}