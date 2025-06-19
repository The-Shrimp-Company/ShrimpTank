using System.Collections;
using System.Collections.Generic;
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
    }

    /// <summary>
    /// Only use when loading save data
    /// </summary>
    /// <param name="newRep"></param>
    public static void SetReputation(float newRep) { instance.reputation = newRep; }
}
