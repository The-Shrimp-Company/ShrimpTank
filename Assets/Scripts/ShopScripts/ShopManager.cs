using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shop
{
    public string name;
    public bool NpcOwned;
    public string NpcName;
    public int reputation;
    public int minPlayerReputation;
    public bool unlocked = false;
    public List<ShrimpStats> shrimpStock = new List<ShrimpStats>();
    public List<Item> otherStock;
    public int maxShrimpStock;
    public int shrimpSold;

    public List<Trait> partTraits = new();
    public List<Trait> patternTraits = new();
    public List<Trait> colourTraits = new();

    public Shop()
    {
        for(int i = 0; i < maxShrimpStock; i++)
        {
            shrimpStock.Add(ShrimpManager.instance.CreateShrimpForShop(this));
        }
    }

    public Shop(int count)
    {
        for(int i = 0; i < count; i++)
        {
            shrimpStock.Add(ShrimpManager.instance.CreateShrimpForShop(this));
        }
    }

    public NPC npc 
    {
        get
        {
            if (NpcOwned)
            {
                return NPCManager.Instance.GetNPCFromName(name);
            } else
            {
                return null;
            }
        }
        private set
        {
            Debug.Log("You should not try to set npc, set NpcName instead");
        }
    }
}


public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public List<Shop> shops;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(List<Shop> shopsToLoad = null)
    {
        shops = shopsToLoad ?? shops ?? new List<Shop>();
        if(shops.Count == 0)
        {
            shops.Add(new Shop(3) { maxShrimpStock = 4, name = "ShrimpShopSupreme", NpcOwned = false, reputation = 2 });
            shops.Add(new Shop(3) { maxShrimpStock = 4, name = "Not the only shrimp shop", NpcOwned = false, reputation = 2 });
            shops.Add(new Shop(3) { maxShrimpStock = 4, name = "The best shrimp shop", NpcOwned = false, reputation = 1 });

        }
        if(shopsToLoad == null)
        {
            foreach(Shop shop in shops)
            {
                for(int i = 0; i < shop.maxShrimpStock; i++)
                {
                    shop.shrimpStock.Add(ShrimpManager.instance.CreateShrimpForShop(shop));
                }
            }
        }
    }

    private void Start()
    {
        TimeManager.instance.onNewDay += UpdateShops;
    }

    public void UpdateShops()
    {
        foreach(Shop shop in shops)
        {
            if (shop.unlocked)
            {
                if (shop.shrimpStock.Count == shop.maxShrimpStock)
                {
                    shop.shrimpStock.RemoveAt(Random.Range(0, shop.shrimpStock.Count - 1));
                }
                shop.shrimpStock.Add(ShrimpManager.instance.CreateShrimpForShop(shop));
                if (shop.shrimpStock.Count > shop.maxShrimpStock)
                {
                    shop.shrimpStock = shop.shrimpStock.GetRange(0, shop.maxShrimpStock);
                }
            }
            else
            {
                if (!shop.NpcOwned)
                {
                    if(Reputation.GetReputation() >= shop.minPlayerReputation)
                    {
                        shop.unlocked = true;
                    }
                }
            }
        }
    }

    public Shop FindNpcShop(string NpcName)
    {
        return shops.Find((x) => { return x.NpcOwned && x.NpcName == NpcName; });
    }
}
