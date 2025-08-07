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
        public Queue<int> completion;
        public List<string> flags = new();
        public string name;

        public bool sent = false;

        public int wakesUp = 10;
        public int fallsAsleep = 22;

        public int lastDaySent = -1;

        public List<ShrimpStats> shrimpBought = new();
    };

    protected NPCData data = new();

    #region Getters/Setters
    protected int reputation { get { return data.reputation; } set { data.reputation = value; } }
    protected int reliability { get { return data.reliability; } set { data.reliability = value; } }
    protected int completion { 
        get 
        {
            if (data.completion.Count > 0)
            {
                return data.completion.Peek();
            }
            else
            {
                return -10;
            }
        } 
        set { data.completion.Enqueue(value); } }
    protected List<string> flags { get { return data.flags; } set { data.flags = value; } }
    public string name { get { return data.name; } protected set { data.name = value; } }
    protected bool sent { get { return data.sent; } set { data.sent = value; } }
    protected int lastDaySent { get { return data.lastDaySent; } set { data.lastDaySent = value; } }
    protected List<ShrimpStats> shrimpBought { get { return data.shrimpBought; } set { data.shrimpBought = value; } }
    protected int wakesUp { get { return data.wakesUp; } set { data.wakesUp = value; } }
    protected int fallsAsleep { get { return data.fallsAsleep; } set { data.wakesUp = value; } }
    public NPCData Data { get { return data; } protected set { data = value; } }
    #endregion

    public NPC(string name, int reputation = 0, int reliability = 0, Queue<int> completion = null)
    {
        this.name = name;
        this.reputation = reputation;
        this.reliability = reliability;
        this.data.completion = completion ?? new Queue<int>();
        this.completion = 0;

        Data = this.NpcValidation(SaveManager.CurrentSaveData.npcs) ?? Data;


    }

    
    public virtual void NpcCheck()
    {
        Debug.LogWarning("Empty NPC Check called from:" + this.GetType());
        
    }

    public virtual bool IsAwake()
    {
        return (TimeManager.instance.hour > wakesUp && TimeManager.instance.hour < fallsAsleep);
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
