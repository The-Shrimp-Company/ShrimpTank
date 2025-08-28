using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reputation
{
    private float reputation = 0;

    public static Reputation instance = new Reputation();

    public static float GetReputation()
    {
        return instance.reputation;
    }

    public static void AddReputation(float add)
    {
        instance.reputation += add;
        if (add > 0) PlayerStats.stats.reputationGained += add;
        if (add < 0) PlayerStats.stats.reputationLost += add;
        if(!Tutorial.instance.flags.Contains("RepLevel1") && instance.reputation >= 20)
        {
            foreach (Shop shop in ShopManager.instance.shops.Where(x => !x.NpcOwned))
            {
                shop.maxShrimpStock++;
            }
            Tutorial.instance.flags.Add("RepLevel1");
        }
    }

    /// <summary>
    /// Only use when loading save data
    /// </summary>
    /// <param name="newRep"></param>
    public static void SetReputation(float newRep) { instance.reputation = newRep; }
}
