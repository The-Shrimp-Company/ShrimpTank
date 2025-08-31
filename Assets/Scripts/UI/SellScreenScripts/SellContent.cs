using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SellContent : ContentPopulation
{
    [SerializeField] private TextMeshProUGUI shrimpCount;
    [SerializeField] private TextMeshProUGUI shrimpPrice;
    [SerializeField] private TextMeshProUGUI reputation;
    [SerializeField] private TMP_InputField storeName;

    // Start is called before the first frame update
    void Awake()
    {
        CreateContent();
        shrimpCount.text = "You have [" + ShrimpManager.instance.allShrimp.Count + "] Shrimp.";
        float price = 0;
        foreach(Shrimp shrimp in ShrimpManager.instance.allShrimp)
        {
            price += EconomyManager.instance.GetShrimpValue(shrimp.stats);
        }
        shrimpPrice.text = "You have [£" + price + "] total value of shrimp.";
        reputation.text = "You have [" + Reputation.GetReputation() + "] Reputation";
        storeName.text = Store.StoreName;
    }

    /// <summary>
    /// Used to create a list of buttons when the screen is started to let the player sell
    /// their shrimp
    /// </summary>
    protected void CreateContent()
    {
        foreach (Shrimp shrimp in CustomerManager.Instance.ToPurchase)
        {
            SellContentBlock tempBlock = Instantiate(contentBlock, transform).GetComponent<SellContentBlock>();
            tempBlock.SetText(shrimp.name);
            tempBlock.SetShrimp(shrimp);
            tempBlock.SetSalePrice();
            contentBlocks.Add(tempBlock);
        }
    }

    /// <summary>
    /// To be called when a shrimp needs to be added to the list
    /// </summary>
    /// <param name="shrimp"></param>
    public void CreateContent(Shrimp shrimp)
    {
        SellContentBlock tempBlock = Instantiate(contentBlock, transform).GetComponent<SellContentBlock>();
        tempBlock.SetText(shrimp.name);
        tempBlock.SetShrimp(shrimp);
        tempBlock.SetSalePrice();
        contentBlocks.Add(tempBlock);
    }

    // Update is called once per frame
    void Update()
    {
        shrimpCount.text = "You have [" + ShrimpManager.instance.allShrimp.Count + "] Shrimp.";
        float price = 0;
        foreach (Shrimp shrimp in ShrimpManager.instance.allShrimp)
        {
            price += EconomyManager.instance.GetShrimpValue(shrimp.stats);
        }
        reputation.text = "You have [" + Reputation.GetReputation() + "] Reputation";
        storeName.text = Store.StoreName;
    }

    public void SetName(string text)
    {
        Store.StoreName = text;
        storeName.text = text;
    }

    public void UISelect()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerInput>().SwitchCurrentActionMap("Empty");
    }

    public void UIUnselect()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
    }
}
