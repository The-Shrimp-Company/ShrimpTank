using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC
{
    protected int reputation;
    protected int reliability;
    protected int completion;

    protected bool sent = false;

    protected int LastDaySent = -1;

    protected List<ShrimpStats> shrimpBought = new List<ShrimpStats>();

    public virtual void NpcCheck()
    {
        Debug.LogWarning("Empty NPC Check called from:" + this.GetType());
    }

    protected void NpcEmail(Email email, bool important = true)
    {
        sent = true;
        EmailManager.SendEmail(email, important, Random.value * 5);
    }

    public virtual void EmailDestroyed()
    {
        LastDaySent = TimeManager.instance.day;
        sent = false;
    }

    public virtual void BoughtShrimp(ShrimpStats stats)
    {
        shrimpBought.Add(stats);
    }
}
