using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using SaveLoadSystem;

public class TankUpgradeController : MonoBehaviour
{
    private Dictionary<UpgradeTypes, TankUpgrade> upgradeScripts = new Dictionary<UpgradeTypes, TankUpgrade>();
    public SerializedDictionary<UpgradeTypes, Transform> upgradeNodes = new SerializedDictionary<UpgradeTypes, Transform>();
    public List<UpgradeItemSO> startingUpgrades = new List<UpgradeItemSO>();
    private TankController tank;

    private void Start()
    {
        tank = GetComponent<TankController>();

        if (upgradeScripts.Count == 0 && !tank.tankLoaded)  // If we haven't already loaded upgrades
        {
            foreach (UpgradeItemSO u in startingUpgrades)
            {
                AddUpgrade(u);
            }
        }
    }


    public void UpdateUpgrades(float elapsedTime)
    {
        foreach (KeyValuePair<UpgradeTypes, TankUpgrade> upgrade in upgradeScripts)
        {
            if (upgrade.Value != null)
            {
                upgrade.Value.UpdateUpgrade(elapsedTime);
            }
        }
    }


    public void AddUpgrade(UpgradeItemSO upgrade)
    {
        if (!upgradeNodes.ContainsKey(upgrade.upgradeType))
        {
            Debug.LogWarning("Upgrade Nodes is missing the key " + upgrade.upgradeType.ToString());
            return;
        }


        if (upgradeNodes[upgrade.upgradeType].childCount != 0)
        {
            RemoveUpgrade(upgrade.upgradeType);
        }


        GameObject newUpgrade = GameObject.Instantiate(upgrade.upgradePrefab, upgradeNodes[upgrade.upgradeType].position, upgradeNodes[upgrade.upgradeType].rotation, upgradeNodes[upgrade.upgradeType]);
        TankUpgrade upgradeScript = newUpgrade.GetComponent<TankUpgrade>();

        if (upgradeScript == null)
        {
            Debug.LogWarning("Upgrade script missing on " + upgrade.upgradePrefab);
            return;
        }

        if (!upgradeScripts.ContainsKey(upgrade.upgradeType))
            upgradeScripts.Add(upgrade.upgradeType, upgradeScript);
        else
            upgradeScripts[upgrade.upgradeType] = upgradeScript;

        upgradeScript.CreateUpgrade(upgrade, tank);


        tank.tankGrid.InitializeGrid();  // Rebake the pathfinding grid
    }


    public void RemoveUpgrade(UpgradeTypes upgradeType)
    {
        if (upgradeScripts.ContainsKey(upgradeType) && upgradeScripts[upgradeType] != null)
        {
            Inventory.AddItem(Inventory.GetItemUsingSO(upgradeScripts[upgradeType].upgrade));
            upgradeScripts[upgradeType].RemoveUpgrade();
            upgradeScripts[upgradeType] = null;
        }

        if (upgradeNodes.ContainsKey(upgradeType))
        {
            if (upgradeNodes[upgradeType].childCount != 0)
                Destroy(upgradeNodes[upgradeType].GetChild(0).gameObject);
        }

        if (tank.tankViewScript)
        {
            UpgradePanel panel = tank.tankViewScript.GetComponentInChildren<UpgradePanel>();
            if (panel) panel.UpdatePanel();
        }

        tank.tankGrid.InitializeGrid();  // Rebake the pathfinding grid
    }


    public bool CheckForUpgrade(UpgradeTypes type)
    {
        if (upgradeScripts.ContainsKey(type) && upgradeScripts[type] != null)
            return true;

        return false;
    }


    public void FixAllUpgrades()
    {
        foreach (KeyValuePair<UpgradeTypes, TankUpgrade> upgrade in upgradeScripts)
        {
            if (upgrade.Value != null && !upgrade.Value.working)
            {
                upgrade.Value.FixUpgrade();
            }
        }
    }

    public void BreakUpgrade(UpgradeTypes type)
    {
        foreach (KeyValuePair<UpgradeTypes, TankUpgrade> upgrade in upgradeScripts)
        {
            if (upgrade.Value != null && upgrade.Key == type && upgrade.Value.working)
            {
                upgrade.Value.BreakUpgrade();
            }
        }
    }


    public TankUpgrade GetUpgrade(UpgradeTypes type)
    {
        if (upgradeScripts.ContainsKey(type) && upgradeScripts[type] != null)
            return upgradeScripts[type];

        return null;
    }


    public string[] SaveUpgrades()
    {
        string[] ids = new string[upgradeScripts.Count];
        int index = 0;

        foreach (KeyValuePair<UpgradeTypes, TankUpgrade> upgrade in upgradeScripts)
        {
            if (index <= ids.Length)
            {
                if (upgrade.Value == null)
                {
                    ids[index] = "";
                    index++;
                }
                else if (upgrade.Value.upgrade != null)
                {
                    ids[index] = upgrade.Value.upgrade.itemName;
                    index++;
                }
            }
        }

        return ids;
    }


    public void LoadUpgrades(string[] ids)
    {
        tank = GetComponent<TankController>();

        foreach (string id in ids)
        {
            if (id != null && id != "")
            {
                foreach (ItemSO so in Inventory.GetLoadedItems())
                {
                    if (so.itemName == id)
                    {
                        AddUpgrade(so as UpgradeItemSO);
                        break;
                    }
                }
            }
        }
    }
}