using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyContentBlock : ContentBlock
{

    private GameObject shop;

    [SerializeField]
    private GameObject shopPrefab;

    [SerializeField]
    private TraitSet[] legs, eyes, head, body, tail, tFan;

    [SerializeField] private int reputationRequirement = 0;

    [SerializeField] private BuyScreen _screen;

    [SerializeField] private Color interactiveTextColour;
    [SerializeField] private Color nonInteractiveTextColour;

    private List<ShrimpStats> shrimp = new List<ShrimpStats>();

    public enum BackgroundSprites : int
    {
        Common,
        Bronze,
        Silver,
        Gold,
        Diamond
    }

    private void Start()
    {
        if(Reputation.GetReputation() < reputationRequirement)
        {
            GetComponent<Button>().interactable = false;
            transform.GetChild(0).GetComponent<TMP_Text>().color = nonInteractiveTextColour;
        }
        else
        {
            GetComponent<Button>().interactable = true;
            transform.GetChild(0).GetComponent<TMP_Text>().color = interactiveTextColour;
        }
    }

    public void SetScreen(BuyScreen screen)
    {
        _screen = screen;
    }

    public void SetBackground(BackgroundSprites sprite)
    {
        GetComponent<Image>().sprite = backSprites[(int)sprite];
    }

    public void Click()
    {
        if (shop == null)
        {
            shop = Instantiate(shopPrefab, transform.parent.transform);
            if(shrimp.Count < 10)
            {
                while(shrimp.Count < 10)
                {
                    shrimp.Add(GenerateShrimp(false));
                }
            }
            shop.GetComponent<ShrimpPurchaseSelection>().Populate(_screen, ref shrimp);
        }
        else
        {
            Destroy(shop);
        }
    }

    private ShrimpStats GenerateShrimp(bool giveName = true)
    {
        GeneManager geneManager = GeneManager.instance;
        ShrimpStats s = ShrimpManager.instance.CreateRandomShrimp(true, giveName);

        string ID = "";
        TraitSet currentTrait;
        int count = 0;


        // Defining random Body
        do
        {
            currentTrait = body[count];
            count++;
        } while (UnityEngine.Random.Range(0, 50) < 15 && count < 4);
        count = 0;

       foreach(TraitSO trait in geneManager.bodySOs)
        {
            if(trait.set == currentTrait)
            {
                ID = trait.ID;
            }
        }

        s.body = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(ID));


        // Defining random Eyes
        do
        {
            currentTrait = eyes[count];
            count++;
        } while (UnityEngine.Random.Range(0, 50) < 15 && count < 4);
        count = 0;

        foreach (TraitSO trait in geneManager.eyeSOs)
        {
            if (trait.set == currentTrait)
            {
                ID = trait.ID;
            }
        }

        s.eyes = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(ID));


        // Defining random Legs
        do
        {
            currentTrait = legs[count];
            count++;
        } while (UnityEngine.Random.Range(0, 50) < 15 && count < 4);
        count = 0;

        foreach (TraitSO trait in geneManager.legsSOs)
        {
            if (trait.set == currentTrait)
            {
                ID = trait.ID;
            }
        }

        s.legs = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(ID));


        // Defining random Tail
        do
        {
            currentTrait = tail[count];
            count++;
        } while (UnityEngine.Random.Range(0, 50) < 15 && count < 4);
        count = 0;

        foreach (TraitSO trait in geneManager.tailSOs)
        {
            if (trait.set == currentTrait)
            {
                ID = trait.ID;
            }
        }

        s.tail = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(ID));


        // Defining random Tail Fan
        do
        {
            currentTrait = tFan[count];
            count++;
        } while (UnityEngine.Random.Range(0, 50) < 15 && count < 4);
        count = 0;

        foreach (TraitSO trait in geneManager.tailFanSOs)
        {
            if (trait.set == currentTrait)
            {
                ID = trait.ID;
            }
        }

        s.tailFan = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(ID));


        // Defining random Head
        do
        {
            currentTrait = head[count];
            count++;
        } while (UnityEngine.Random.Range(0, 50) < 15 && count < 4);
        count = 0;

        foreach (TraitSO trait in geneManager.headSOs)
        {
            if (trait.set == currentTrait)
            {
                ID = trait.ID;
            }
        }

        s.head = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(ID));


        return s;
    }
}
