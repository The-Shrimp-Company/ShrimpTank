using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class SingleShopScript : ScreenView
{
    [SerializeField]
    private GameObject shrimpPreview;
    [SerializeField]
    private Transform contentParent;
    [SerializeField] TextMeshProUGUI ShopName;
    [SerializeField] TextMeshProUGUI ShopMotto;

    private List<ContentBlock> ContentBlocks = new List<ContentBlock>();
    private Shop thisShop;

    [Header("Context Panel")]
    [SerializeField] private GameObject ContextPanel;
    [SerializeField] private TextMeshProUGUI sexLabel, legsLabel, headLabel, tailLabel, tFanLabel, eyesLabel, bodyLabel, patternLabel;
    [SerializeField] private TextMeshProUGUI saltLabel, hnoLabel, tempLabel, PhLabel;
    [SerializeField] private Image colour1, colour2;
    [SerializeField] private TextMeshProUGUI price;
    

    private ShrimpSelectionBlock selectedBlock;


    public void Populate(Shop shop)
    {
        thisShop = shop;
        foreach (ShrimpStats stats in shop.shrimpStock)
        {
            ShrimpSelectionBlock block = Instantiate(shrimpPreview, contentParent).GetComponent<ShrimpSelectionBlock>();
            block.Populate(stats);
            block.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectShrimp(block);
            });
            ContentBlocks.Add(block);
        }
        ShopName.text = shop.name;
        ShopMotto.text = "\"" + shop.ShopMotto + "\"";
    }

    private void Update()
    {
        ContextPanel.SetActive(selectedBlock != null);
    }

    public void BuySelected()
    {

        if (Store.SpawnShrimp(selectedBlock.GetShrimp(), EconomyManager.instance.GetShrimpValue(selectedBlock.GetShrimp())))
        {
            thisShop.shrimpStock.Remove(selectedBlock.GetComponent<ShrimpSelectionBlock>().GetShrimp());
            EconomyManager.instance.UpdateTraitValues(true, selectedBlock.GetShrimp());
            Destroy(selectedBlock.gameObject);
        }
    }

    public void SelectShrimp(ShrimpSelectionBlock newSelectedBlock)
    {
        selectedBlock = newSelectedBlock;
        ShrimpStats shrimp = selectedBlock.GetShrimp();
        sexLabel.text = "Sex: " + (shrimp.sex == true ? "M" : "F");
        patternLabel.text = "Pattern: " + GeneManager.instance.GetTraitSO(shrimp.pattern.activeGene.ID).traitName;
        bodyLabel.text = "Body: " + GeneManager.instance.GetTraitSO(shrimp.body.activeGene.ID).set;
        legsLabel.text = "Legs: " + GeneManager.instance.GetTraitSO(shrimp.legs.activeGene.ID).set;
        eyesLabel.text = "Eyes: " + GeneManager.instance.GetTraitSO(shrimp.eyes.activeGene.ID).set;
        tailLabel.text = "Tail: " + GeneManager.instance.GetTraitSO(shrimp.tail.activeGene.ID).set;
        headLabel.text = "Head: " + GeneManager.instance.GetTraitSO(shrimp.head.activeGene.ID).set;
        tFanLabel.text = "Tail Fan: " + GeneManager.instance.GetTraitSO(shrimp.tailFan.activeGene.ID).set;
        colour1.color = GeneManager.instance.GetTraitSO(shrimp.primaryColour.activeGene.ID).colour;
        colour2.color = GeneManager.instance.GetTraitSO(shrimp.secondaryColour.activeGene.ID).colour;
        //hunger.value = shrimp.hunger;
        TankController tank = Store.GetDestinationTank();
        tempLabel.text = "Temperature:\n" + shrimp.temperaturePreference.ToString();
        if (Mathf.Abs(tank.waterTemperature - shrimp.temperaturePreference) >= tank.tempVariance) tempLabel.text += "<color=red><size=20>\nTank not suitable</size></color>";
        saltLabel.text = "Salt:\n" + shrimp.salineLevel.ToString();
        if (Mathf.Abs(tank.waterSalt - shrimp.salineLevel) >= tank.saltVariance) saltLabel.text += "<color=red><size=20>\nTank not suitable</size></color>";
        hnoLabel.text = "Ammonium:\n" + shrimp.ammoniaPreference.ToString();
        if (Mathf.Abs(tank.waterAmmonium - shrimp.ammoniaPreference) >= tank.nitrVariance) hnoLabel.text += "<color=red><size=20>\nTank not suitable</size></color>";
        PhLabel.text = "ph:\n" + shrimp.PhPreference.ToString();
        if (Mathf.Abs(tank.waterPh - shrimp.PhPreference) >= tank.pHVariance) PhLabel.text += "<color=red><size=20>\nTank not suitable</size></color>";

        /*
        if (Store.GetDestinationTank().foodInTank.Count <= 0)
        {
            price.text = "Current Destination tank has no food, can't buy shrimp into it";
            price.transform.parent.GetComponent<Button>().interactable = false;
        }*/
        if (Mathf.Abs(tank.waterAmmonium - shrimp.ammoniaPreference) >= tank.nitrVariance &&
            Mathf.Abs(tank.waterSalt - shrimp.salineLevel) >= tank.saltVariance &&
            Mathf.Abs(tank.waterPh - shrimp.PhPreference) >= tank.pHVariance &&
            Mathf.Abs(tank.waterTemperature - shrimp.temperaturePreference) >= tank.tempVariance)
        {
            price.text = "Can't buy this shrimp with current destination tank. Shrimp will die.";
            price.transform.parent.GetComponent<Button>().interactable = false;
        }
        else
        {
            price.text = "Buy Shrimp\n£" + EconomyManager.instance.GetShrimpValue(shrimp).RoundMoney().ToString();
            price.transform.parent.GetComponent<Button>().interactable = true;
        }
    }

    public void Deselect()
    {
        selectedBlock = null;
    }
}
