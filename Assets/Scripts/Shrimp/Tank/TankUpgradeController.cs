using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using static UnityEngine.Rendering.DebugUI;
using SaveLoadSystem;

public class TankUpgradeController : MonoBehaviour
{
    private Dictionary<UpgradeTypes, TankUpgrade> upgradeScripts = new Dictionary<UpgradeTypes, TankUpgrade>();
    public SerializedDictionary<UpgradeTypes, Transform> upgradeNodes = new SerializedDictionary<UpgradeTypes, Transform>();
    public List<UpgradeSO> startingUpgrades = new List<UpgradeSO>();
    private TankController tank;

    private void Start()
    {
        tank = GetComponent<TankController>();

        if (upgradeScripts.Count == 0)  // If we haven't already loaded upgrades
        {
            foreach (UpgradeSO u in startingUpgrades)
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


    public void AddUpgrade(UpgradeSO upgrade)
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
        // Add it back to the inventory?



        if (upgradeScripts.ContainsKey(upgradeType) && upgradeScripts[upgradeType] != null)
        {
            upgradeScripts[upgradeType].RemoveUpgrade();
            upgradeScripts[upgradeType] = null;
        }

        if (upgradeNodes.ContainsKey(upgradeType))
        {
            if (upgradeNodes[upgradeType].childCount != 0)
                Destroy(upgradeNodes[upgradeType].GetChild(0).gameObject);
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
            if (upgrade.Value.upgrade != null && index <= ids.Length)
            {
                ids[index] = upgrade.Value.upgrade.ID;
                index++;
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
                foreach (UpgradeSO so in UpgradeList.instance.Upgrades)
                {
                    if (so.ID == id)
                    {
                        AddUpgrade(so);
                        break;
                    }
                }
            }
        }
    }
}