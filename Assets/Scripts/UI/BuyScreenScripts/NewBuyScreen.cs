using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewBuyScreen : ScreenView
{

    [SerializeField]
    private Transform contentParent;

    [SerializeField]
    private GameObject shopLabel;

    [SerializeField]
    private GameObject shopScreen;

    private List<GameObject> shopLabels = new();

    protected override void Start()
    {
        base.Start();
        shelves = transform.parent.GetComponent<ShelfRef>().GetShelves();

        foreach(Shop shop in ShopManager.instance.shops)
        {
            GameObject newLabel = Instantiate(shopLabel, contentParent);
            shopLabels.Add(newLabel);
            newLabel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = shop.name;
            string reputation = "Reputation:\n";
            if (shop.NpcOwned)
            {
                reputation += Mathf.RoundToInt(NPCManager.Instance.GetNPCFromName(shop.NpcName).Data.reputation/10).ToString();
            }
            else
            {
                reputation += shop.reputation.ToString();
            }
            reputation += "/5";
            newLabel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = reputation;
            newLabel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Shrimp In Stock:\n" + shop.shrimpStock.Count.ToString();

            Shop tempShop = shop;
            newLabel.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject newScreen = Instantiate(shopScreen, UIManager.instance.tabletParent);
                newScreen.GetComponent<SingleShopScript>().Populate(shop);
                UIManager.instance.OpenScreen(newScreen.GetComponent<ScreenView>());
            });
        }
    }

    public void OnEnable()
    {
        foreach (GameObject label in shopLabels)
        {
            Shop shop = ShopManager.instance.shops.Find((x) => { return x.name == label.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text; });
            string reputation = "Reputation:\n";
            if (shop.NpcOwned)
            {
                reputation += Mathf.RoundToInt(NPCManager.Instance.GetNPCFromName(shop.NpcName).Data.reputation / 10).ToString();
            }
            else
            {
                reputation += shop.reputation.ToString();
            }
            reputation += "/5";
            label.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = reputation;
            label.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Shrimp In Stock:\n" + shop.shrimpStock.Count.ToString();
        }
    }
}
