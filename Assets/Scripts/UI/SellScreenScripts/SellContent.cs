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
    [SerializeField] private TMP_InputField storeName;

    // Start is called before the first frame update
    void Awake()
    {
        shrimpCount.text = "You have [" + ShrimpManager.instance.allShrimp.Count + "] Shrimp.";
        float price = 0;
        foreach(Shrimp shrimp in ShrimpManager.instance.allShrimp)
        {
            price += EconomyManager.instance.GetShrimpValue(shrimp.stats);
        }
        shrimpPrice.text = "You have [£" + price + "] total value of shrimp.";
        storeName.text = Store.StoreName;
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
        storeName.text = Store.StoreName;
    }

    public void SetName(string text)
    {
        Debug.Log(text);
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
