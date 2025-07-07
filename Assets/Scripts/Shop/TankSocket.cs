using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSocket : MonoBehaviour
{
    [HideInInspector] public TankController tank;
    private ShelfSpawn shelves;

    private void OnEnable()
    {
        shelves = GetComponentInParent<Shelf>().GetShelves();
    }

    public bool TankExists()
    {
        if (tank == null) return false;
        else return true;
    }

    public void AddSmallTank()
    {
        AddTank(TankTypes.Small);
    }

    public void AddLargeTank()
    {
        AddTank(TankTypes.Small);
    }

    public void AddTank(TankTypes type, bool loading = false)
    {
        if (!loading && !Inventory.instance.RemoveItem(Inventory.GetItemUsingName("Small Tank"))) return;

        GameObject prefab = null;
        switch (type)
        {
            case TankTypes.Small:
                {
                    prefab = shelves.smallTankPrefab;
                    if (!loading) PlayerStats.stats.smallTankCount++;
                    break;
                }
            case TankTypes.Large:
                {
                    prefab = shelves.largeTankPrefab;
                    if (!loading) PlayerStats.stats.largeTankCount++;
                    break;
                }
        }

        GameObject newTank = GameObject.Instantiate(prefab, transform.position, transform.rotation, transform);
        tank = newTank.GetComponent<TankController>();

        GetComponent<BoxCollider>().enabled = false;

        Inventory.instance.activeTanks.Add(tank);

        if (!loading)
        {
            PlayerStats.stats.tankCount++;
            shelves.SwitchDestinationTank(tank);
            tank.tankName = "Tank " + Inventory.instance.activeTanks.Count;
        }
    }

    public void LoadTank(TankSocketSaveData socketData)
    {
        TankSaveData data = socketData.tank;

        AddTank(socketData.type, true);

        tank.tankName = data.tankName;
        tank.openTankPrice = data.openTankPrice;
        if (data.destinationTank) shelves.SwitchDestinationTank(tank);
        if (data.openTank) tank.toggleTankOpen();

        foreach (ShrimpStats s in data.shrimp)
        {
            tank.SpawnShrimp(s, true);
        }

        tank.GetComponent<TankUpgradeController>().LoadUpgrades(data.upgradeIDs);
    }
}
