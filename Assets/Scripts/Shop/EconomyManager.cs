using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance;

    [Header("Starting Trait Values")]
    [SerializeField] float minTraitValue = 5;
    [SerializeField] float maxTraitValue = 30;
    [SerializeField][Range(0, 100)] float weightTowardsDominancePercentage = 20;

    [Header("Value Fluctuation")]
    [SerializeField] float valueFluctuation;  // How much the value is changed when a shrimp is sold. Will be multiplied by the valueFluctuationStrength
    [SerializeField] AnimationCurve valueFluctuationStrength;  // 0 is the starting value

    [Header("Value Multipliers")]
    [SerializeField] float pureShrimpMultiplier = 1.5f;
    [SerializeField] float pureColourShrimpMultiplier = 0.5f;
    [SerializeField] AnimationCurve healthMultiplier;

    [Header("Value Return")]
    [SerializeField] float timeBetweenValueReturnUpdates = 30;  // How often the values of traits will automatically be updated
    private float valueReturnUpdateTimer;
    [SerializeField] float valueReturnAmount = 0.2f;
    [SerializeField][Range(0, 100)] public int percentageOfTraitsToReturnValue = 15;  // How many of the traits will have their values updated

    [Header("Daily Value Updates")]
    [SerializeField][Range(0, 100)] public int percentageOfTraitsToUpdateDaily = 25;  // How many of the traits will have their values updated
    [SerializeField] float maxDailyValueUpdateAmount = 3.5f;

    public void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        valueReturnUpdateTimer += Time.deltaTime;
        if (valueReturnUpdateTimer >= timeBetweenValueReturnUpdates)
        {
            valueReturnUpdateTimer = 0;
            GeneManager.instance.ReturnGeneValues(percentageOfTraitsToReturnValue);
        }
    }

    public float SetInitialGeneValue(int dominance)
    {
        float val = Random.Range(minTraitValue, maxTraitValue);  // Pick a random initial value for the gene

        float d = Mathf.InverseLerp(0, 100, (float)dominance);
        d = Mathf.Lerp(minTraitValue, maxTraitValue, d);  // Convert dominance from the dominance range to the value range

        float weighting = weightTowardsDominancePercentage / 100;  // Convert percentage to decimal
        val += (d - val) * weighting;  // Weight the value towards the dominance

        val = RoundMoney(val);  // Round to 2 decimal places

        return val;
    }

    public void UpdateTraitValues(bool purchased, ShrimpStats traits)
    {
        UpdateValueOfGene(purchased, traits.primaryColour.activeGene);
        UpdateValueOfGene(purchased, traits.secondaryColour.activeGene);
        UpdateValueOfGene(purchased, traits.pattern.activeGene);

        UpdateValueOfGene(purchased, traits.body.activeGene);
        UpdateValueOfGene(purchased, traits.head.activeGene);
        UpdateValueOfGene(purchased, traits.eyes.activeGene);
        UpdateValueOfGene(purchased, traits.tail.activeGene);
        UpdateValueOfGene(purchased, traits.tailFan.activeGene);
        UpdateValueOfGene(purchased, traits.legs.activeGene);
    }

    private void UpdateValueOfGene(bool purchased, Gene g)
    {
        GlobalGene global = GeneManager.instance.GetGlobalGene(g.ID);
        float x = (minTraitValue + maxTraitValue) / 2;
        float min = global.startingValue - x;
        float max = global.startingValue + x;
        float l = Mathf.InverseLerp(min, max, global.trueValue);

        if (purchased)
        {
            global.trueValue = Mathf.Clamp(global.trueValue + (valueFluctuation * valueFluctuationStrength.Evaluate(l)), minTraitValue, maxTraitValue);
        }

        else if (!purchased)
        {
            global.trueValue = Mathf.Clamp(global.trueValue - (valueFluctuation * valueFluctuationStrength.Evaluate(l)), minTraitValue, maxTraitValue);
        }

        GeneManager.instance.SetGlobalGene(global);
    }

    public float GetShrimpValue(ShrimpStats s)
    {
        // Add trait values
        float t = 0;
        t += GeneManager.instance.GetGlobalGene(s.primaryColour.activeGene.ID).currentValue;
        t += GeneManager.instance.GetGlobalGene(s.secondaryColour.activeGene.ID).currentValue;
        t += GeneManager.instance.GetGlobalGene(s.pattern.activeGene.ID).currentValue;

        t += GeneManager.instance.GetGlobalGene(s.body.activeGene.ID).currentValue;
        t += GeneManager.instance.GetGlobalGene(s.head.activeGene.ID).currentValue;
        t += GeneManager.instance.GetGlobalGene(s.eyes.activeGene.ID).currentValue;
        t += GeneManager.instance.GetGlobalGene(s.tail.activeGene.ID).currentValue;
        t += GeneManager.instance.GetGlobalGene(s.tailFan.activeGene.ID).currentValue;
        t += GeneManager.instance.GetGlobalGene(s.legs.activeGene.ID).currentValue;


        // Apply multipliers

        if (GeneManager.instance.CheckForPureShrimp(s)) t *= pureShrimpMultiplier;  // Pure Shrimp

        if (GeneManager.instance.CheckForPureColourShrimp(s)) t *= pureColourShrimpMultiplier;  // Pure Colour Shrimp

        t *= healthMultiplier.Evaluate(s.illnessLevel / ShrimpManager.instance.maxShrimpIllness);  // Shrimp Health


        t = RoundMoney(t);  // Round to 2 decimal places

        return t;
    }

    public float GetObfsShrimpValue(ShrimpStats s)  // Get Obfuscated Shrimp Value
    {
        // Add trait values
        float t = 0;
        if (!s.primaryColour.obfuscated) t += GeneManager.instance.GetGlobalGene(s.primaryColour.activeGene.ID).currentValue;
        if (!s.secondaryColour.obfuscated) t += GeneManager.instance.GetGlobalGene(s.secondaryColour.activeGene.ID).currentValue;
        if (!s.pattern.obfuscated) t += GeneManager.instance.GetGlobalGene(s.pattern.activeGene.ID).currentValue;

        if (!s.body.obfuscated) t += GeneManager.instance.GetGlobalGene(s.body.activeGene.ID).currentValue;
        if (!s.head.obfuscated) t += GeneManager.instance.GetGlobalGene(s.head.activeGene.ID).currentValue;
        if (!s.eyes.obfuscated) t += GeneManager.instance.GetGlobalGene(s.eyes.activeGene.ID).currentValue;
        if (!s.tail.obfuscated) t += GeneManager.instance.GetGlobalGene(s.tail.activeGene.ID).currentValue;
        if (!s.legs.obfuscated) t += GeneManager.instance.GetGlobalGene(s.legs.activeGene.ID).currentValue;
        if (!s.tailFan.obfuscated) t += GeneManager.instance.GetGlobalGene(s.tailFan.activeGene.ID).currentValue;

        t = RoundMoney(t*10);  // Round to 2 decimal places

        return t;
    }

    public void DailyUpdate()
    {
        GeneManager.instance.DailyGeneValueUpdate(percentageOfTraitsToUpdateDaily);
    }

    public void DailyValueUpdate(GlobalGene g)
    {
        float rand = Random.Range(-maxDailyValueUpdateAmount, maxDailyValueUpdateAmount);
        g.trueValue += rand;
    }

    public void ValueReturn(GlobalGene g)
    {
        if (g.trueValue == g.startingValue) return;

        float rand = Random.Range(0, 10);
        float r = valueReturnAmount / rand;

        if (g.trueValue > g.startingValue)
            g.trueValue = Mathf.Clamp(g.trueValue - r, g.startingValue, Mathf.Infinity);

        else if (g.trueValue < g.startingValue)
            g.trueValue = Mathf.Clamp(g.trueValue + r, g.startingValue, Mathf.Infinity);
    }

    public float RoundMoney(float m)
    {
        return Mathf.Round(m * 100f) / 100f;  // Round to 2 decimal places
    }
}

