using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class ShrimpSelectionBlock : ContentBlock
{
    [SerializeField] private TextMeshProUGUI breedname;
    [SerializeField] private Image primaryColour, secondaryColour;
    [SerializeField] private Sprite[] Rarity;

    private ShrimpStats _shrimp;

    public BuyScreen screen;
    public EmailScreen email;
    private ShrimpPurchaseContent parent;

    public void Populate(ShrimpStats shrimp)
    {
        _shrimp = shrimp;
        text.text = _shrimp.name;
        breedname.text = _shrimp.GetBreedname();
        primaryColour.color = GeneManager.instance.GetTraitSO(_shrimp.primaryColour.activeGene.ID).colour;
        secondaryColour.color = GeneManager.instance.GetTraitSO(_shrimp.secondaryColour.activeGene.ID).colour;
        int rarityApprox = Mathf.RoundToInt(EconomyManager.instance.GetShrimpValue(shrimp) / 4) - 1;
        if(rarityApprox < 3 && rarityApprox >= 0)
        {
            GetComponent<Image>().sprite = Rarity[rarityApprox];
        }
        else if(rarityApprox < 0)
        {
            GetComponent<Image>().sprite = Rarity[0];
        }
        else
        {
            GetComponent<Image>().sprite = Rarity[4];
        }
        //price.text = EconomyManager.instance.GetShrimpValue(shrimp).RoundMoney().ToString();
    }

    public void Populate(ShrimpStats shrimp, ShrimpPurchaseContent par)
    {
        parent = par;
        Populate(shrimp);
    }

    public void BuyThis()
    {
        if (screen.BuyShrimp(_shrimp))
        {
            EconomyManager.instance.UpdateTraitValues(true, _shrimp);
            parent._shrimp.Remove(_shrimp);
            Destroy(gameObject);
        }
    }

    public ShrimpStats GetShrimp()
    {
        return _shrimp;
    }

    public void SellThis()
    {
        email.CloseSelection();
    }
}
