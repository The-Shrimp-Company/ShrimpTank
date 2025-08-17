using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop
{
    public string name;
    public bool NpcOwned;
    public string NpcName;
    public int reputation;
    public List<ShrimpStats> shrimpStock = new List<ShrimpStats>();
    public List<Item> otherStock;
    public int maxShrimpStock;
    public int shrimpSold;

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
        shops = shopsToLoad ?? new List<Shop>();
        if(shops.Count == 0)
        {
            shops.Add(new Shop() { maxShrimpStock = 4, name = "ShrimpShopSupreme", NpcOwned = false, reputation = 2 });
            shops.Add(new Shop() { maxShrimpStock = 4, name = "Not the only shrimp shop", NpcOwned = false, reputation = 2 });
            shops.Add(new Shop() { maxShrimpStock = 4, name = "The best shrimp shop", NpcOwned = false, reputation = 1 });

            UpdateShops();
            UpdateShops();
            UpdateShops();
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
            if(shop.shrimpStock.Count == shop.maxShrimpStock)
            {
                shop.shrimpStock.RemoveAt(Random.Range(0, shop.shrimpStock.Count - 1));
            }
            shop.shrimpStock.Add(ShrimpManager.instance.CreateRandomShrimp(true, false));
            if(shop.shrimpStock.Count > shop.maxShrimpStock)
            {
                shop.shrimpStock = shop.shrimpStock.GetRange(0, shop.maxShrimpStock);
            }
        }
    }

    public Shop FindNpcShop(string NpcName)
    {
        return shops.Find((x) => { return x.NpcOwned && x.NpcName == NpcName; });
    }
}
