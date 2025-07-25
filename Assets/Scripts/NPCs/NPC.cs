using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class NPC
{
    [System.Serializable]
    public class NPCData
    {
        public int reputation;
        public int reliability;
        public int completion;
        public List<string> flags = new();
        public string name;

        public bool sent = false;

        public int lastDaySent = -1;

        public List<ShrimpStats> shrimpBought = new();
    };

    protected NPCData data = new();

    #region Getters/Setters
    protected int reputation { get { return data.reputation; } set { data.reputation = value; } }
    protected int reliability { get { return data.reliability; } set { data.reliability = value; } }
    protected int completion { get { return data.completion; } set { data.completion = value; } }
    protected List<string> flags { get { return data.flags; } set { data.flags = value; } }
    public string name { get { return data.name; } protected set { data.name = value; } }
    protected bool sent { get { return data.sent; } set { data.sent = value; } }
    protected int lastDaySent { get { return data.lastDaySent; } set { data.lastDaySent = value; } }
    protected List<ShrimpStats> shrimpBought { get { return data.shrimpBought; } set { data.shrimpBought = value; } }
    public NPCData Data { get { return data; } protected set { data = value; } }
    #endregion

    public NPC(string name, int reputation = 0, int reliability = 0, int completion = 0)
    {
        this.name = name;
        this.reputation = reputation;
        this.reliability = reliability;
        this.completion = completion;

        Data = this.NpcValidation(SaveManager.CurrentSaveData.npcs) ?? Data;

        Debug.Log(this.name + this.completion);

    }

    
    public virtual void NpcCheck()
    {
        Debug.LogWarning("Empty NPC Check called from:" + this.GetType());
        
    }

    protected void NpcEmail(Email email, float delay, bool important = true)
    {
        sent = true;
        EmailManager.SendEmail(email, important, delay);
    }

    protected void NpcEmail(Email email, bool important = true)
    {
        sent = true;
        EmailManager.SendEmail(email, important, Random.value * 5);
    }

    public virtual void EmailDestroyed()
    {
        lastDaySent = TimeManager.instance.day;
        sent = false;
    }

    public virtual void BoughtShrimp(ShrimpStats stats)
    {
        shrimpBought.Add(stats);
    }

    public void SetCompletion(int comp)
    {
        completion = comp;
    }

    public void SetFlag(string flag)
    {
        flags.Add(flag);
    }
}
