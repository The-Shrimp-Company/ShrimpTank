using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;


public class ShrimpManager : MonoBehaviour
{
    public static ShrimpManager instance;
    private GeneManager geneManager;

    public GameObject shrimpPrefab;

    public List<Shrimp> allShrimp = new List<Shrimp>();
    private int numberOfShrimp = 0;

    private List<string> names = new List<string>();


    [Header("Size")]
    [SerializeField][Range(0.15f, 0.75f)] float childSize = 0.25f;
    [SerializeField] AnimationCurve sizeThroughChildhood;  // Childhood ends when moltSpeed levels out
    [SerializeField][Range(0.15f, 0.75f)] float adultSize = 0.3f;
    [SerializeField] AnimationCurve sizeThroughAdulthood;
    [SerializeField][Range(0.15f, 0.75f)] float fullyGrownSize = 0.65f;
    [SerializeField][Range(0.1f, 1.0f)] float geneticSizeImpact = 0.25f;
    private int maxGeneticShrimpSize = 100;


    [Header("Illness")]
    [HideInInspector] public int maxShrimpIllness = 100;

    [Header("Age")]
    [SerializeField] int maxShrimpAge;
    [SerializeField] AnimationCurve moltSpeed;  // In minutes, realtime
    [SerializeField] float moltSpeedRequiredToBeAdult = 5;
    private int adultAge;
    [SerializeField] float minAgeForAShrimpBeingBought = 45;

    [Header("Breeding")]
    [SerializeField] AnimationCurve femaleBreedingTime;  // Time till they can breed again based on the number of shrimp in the tank
    [SerializeField] float femaleBreedingRandomVariance;
    [SerializeField] AnimationCurve maleBreedingTime;  // Time till they can breed again based on the number of shrimp in the tank
    [SerializeField] float maleBreedingRandomVariance;
    public int shrimpInTankBreedingLimit = 15;  // Shrimp will not breed at all once there are this many shrimp in the tank
    public int minChildrenToGiveBirthTo = 3;
    public int maxChildrenToGiveBirthTo = 5;

    [Header("Death")]
    [SerializeField] AnimationCurve shrimpNaturalDeathAge;
    [SerializeField] AnimationCurve shrimpIllnessDeathChance;
    [SerializeField] AnimationCurve shrimpTemperatureDeathChance;
    [SerializeField] AnimationCurve shrimpWaterQualityDeathChance;

    [Header("Temperament")]
    private int maxShrimpTemperament = 100;

    public TankController destTank;


    public void Awake()
    {
        instance = this;
        geneManager = GetComponent<GeneManager>();
        foreach (string line in File.ReadLines(Path.Combine(Application.streamingAssetsPath, "ShrimpNames.txt")))
        {
            names.Add(line);
        }
    }


    public ShrimpStats CreateShrimpThroughBreeding(ShrimpStats parentA, ShrimpStats parentB)
    {
        ShrimpStats s = new ShrimpStats();

        s.name = GenerateShrimpName();
        s.gender = geneManager.RandomGender();
        s.bornInStore = true;
        s.birthTime = TimeManager.instance.GetTotalTime();
        s.hunger = 0;
        s.illnessLevel = 0;
        s.geneticSize = geneManager.IntGene(geneManager.sizeInheritance, maxGeneticShrimpSize, parentA.geneticSize, parentB.geneticSize, geneManager.sizeCanMutate);
        s.temperament = geneManager.IntGene(geneManager.temperamentInheritance, maxShrimpTemperament, parentA.temperament, parentB.temperament, geneManager.temperamentCanMutate);

        s.salineLevel = 50;
        s.immunity = 0;
        s.metabolism = geneManager.IntGene(InheritanceType.FlatAverage, 100, parentA.metabolism, parentB.metabolism, true);
        s.filtration = 0;
        s.temperaturePreference = 50;
        s.PhPreference = 7;
        s.ammoniaPreference = 50;

        s.primaryColour = geneManager.TraitGene(geneManager.colourInheritance, 0, parentA.primaryColour, parentB.primaryColour, geneManager.colourCanMutate);
        s.secondaryColour = geneManager.TraitGene(geneManager.colourInheritance, 0, parentA.secondaryColour, parentB.secondaryColour, geneManager.colourCanMutate);

        s.pattern = geneManager.TraitGene(geneManager.patternInheritance, 0, parentA.pattern, parentB.pattern, geneManager.patternCanMutate);

        s.body = geneManager.TraitGene(geneManager.bodyPartInheritance, 0, parentA.body, parentB.body, geneManager.bodyPartCanMutate);
        s.head = geneManager.TraitGene(geneManager.bodyPartInheritance, 0, parentA.head, parentB.head, geneManager.bodyPartCanMutate);
        s.eyes = geneManager.TraitGene(geneManager.bodyPartInheritance, 0, parentA.eyes, parentB.eyes, geneManager.bodyPartCanMutate);
        s.tail = geneManager.TraitGene(geneManager.bodyPartInheritance, 0, parentA.tail, parentB.tail, geneManager.bodyPartCanMutate);
        s.tailFan = geneManager.TraitGene(geneManager.bodyPartInheritance, 0, parentA.tailFan, parentB.tailFan, geneManager.bodyPartCanMutate);
        s.legs = geneManager.TraitGene(geneManager.bodyPartInheritance, 0, parentA.legs, parentB.legs, geneManager.bodyPartCanMutate);

        s = geneManager.ApplyStatModifiers(s);

        return s;
    }


    public ShrimpStats CreateRandomShrimp(bool adultAge, bool giveName = true)
    {
        ShrimpStats s = new ShrimpStats();

        if (giveName)
        {
            s.name = GenerateShrimpName();
        }
        else
        {
            s.name = "";
        }
        s.gender = geneManager.RandomGender();
        if (adultAge)
            s.birthTime = TimeManager.instance.CalculateBirthTimeFromAge(geneManager.IntGene(InheritanceType.FullRandom, Mathf.RoundToInt((maxShrimpAge - (1 + minAgeForAShrimpBeingBought)) * 0.9f), 0, 0, false) + Random.value + minAgeForAShrimpBeingBought);
        else s.birthTime = TimeManager.instance.CalculateBirthTimeFromAge(geneManager.IntGene(InheritanceType.FullRandom, Mathf.RoundToInt((maxShrimpAge - 1) * 0.9f), 0, 0, false) + Random.value);
        
        s.temperament = geneManager.IntGene(InheritanceType.FullRandom, maxShrimpTemperament, 0, 0, false);
        s.geneticSize = geneManager.IntGene(InheritanceType.FullRandom, maxGeneticShrimpSize, 0, 0, false);
        s.hunger = 0;
        s.illnessLevel = 0;

        s.salineLevel = 50;
        s.immunity = 0;
        s.metabolism = geneManager.IntGene(InheritanceType.FullRandom, 30, 0, 0, false);
        s.filtration = 0;
        s.temperaturePreference = 50;
        s.PhPreference = 7;
        s.ammoniaPreference = 50;

        Trait t = new Trait();
        t.activeGene.ID = "C";
        s.primaryColour = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);
        s.secondaryColour = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);

        t.activeGene.ID = "P";
        s.pattern = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);

        t.activeGene.ID = "B";
        s.body = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);

        t.activeGene.ID = "H";
        s.head = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);

        t.activeGene.ID = "E";
        s.eyes = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);

        t.activeGene.ID = "T";
        s.tail = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);

        t.activeGene.ID = "F";
        s.tailFan = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);

        t.activeGene.ID = "L";
        s.legs = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);

        s = geneManager.ApplyStatModifiers(s);

        return s;
    }

    public ShrimpStats CreateShrimpForShop(Shop shop)
    {
        if (shop.partTraits.Count == 0)
        {
            return CreateRandomShrimp(true, false);
        }

        ShrimpStats s = new ShrimpStats();


        s.gender = geneManager.RandomGender();
        
        s.birthTime = TimeManager.instance.CalculateBirthTimeFromAge(geneManager.IntGene(InheritanceType.FullRandom, Mathf.RoundToInt((maxShrimpAge - (1 + minAgeForAShrimpBeingBought)) * 0.9f), 0, 0, false) + Random.value + minAgeForAShrimpBeingBought);

        s.temperament = geneManager.IntGene(InheritanceType.FullRandom, maxShrimpTemperament, 0, 0, false);
        s.geneticSize = geneManager.IntGene(InheritanceType.FullRandom, maxGeneticShrimpSize, 0, 0, false);
        s.hunger = 0;
        s.illnessLevel = 0;

        s.salineLevel = 50;
        s.immunity = 0;
        s.metabolism = geneManager.IntGene(InheritanceType.FullRandom, 30, 0, 0, false);
        s.filtration = 0;
        s.temperaturePreference = 50;
        s.PhPreference = 7;
        s.ammoniaPreference = 50;

        Trait t1, t2 = new Trait();

        t1 = shop.colourTraits[Random.Range(0, shop.colourTraits.Count())];
        t2 = shop.colourTraits[Random.Range(0, shop.colourTraits.Count())];
        s.primaryColour = geneManager.TraitGene(InheritanceType.Punnett, 0, t1, t2, false);
        s.secondaryColour = geneManager.TraitGene(InheritanceType.Punnett, 0, t1, t2, false);

        t1 = shop.patternTraits[Random.Range(0, shop.patternTraits.Count())];
        t2 = shop.patternTraits[Random.Range(0, shop.patternTraits.Count())];
        s.pattern = geneManager.TraitGene(InheritanceType.Punnett, 0, t1, t2, false);

        shop.partTraits.ForEach(x => x.SetTraitID("B"));
        t1 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        t2 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        s.body = geneManager.TraitGene(InheritanceType.Punnett, 0, t1, t2, false);

        shop.partTraits.ForEach(x => x.SetTraitID("H"));
        t1 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        t2 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        s.head = geneManager.TraitGene(InheritanceType.Punnett, 0, t1, t2, false);

        shop.partTraits.ForEach(x => x.SetTraitID("E"));
        t1 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        t2 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        s.eyes = geneManager.TraitGene(InheritanceType.Punnett, 0, t1, t2, false);

        shop.partTraits.ForEach(x => x.SetTraitID("T"));
        t1 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        t2 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        s.tail = geneManager.TraitGene(InheritanceType.Punnett, 0, t1, t2, false);

        shop.partTraits.ForEach(x => x.SetTraitID("F"));
        t1 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        t2 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        s.tailFan = geneManager.TraitGene(InheritanceType.Punnett, 0, t1, t2, false);

        shop.partTraits.ForEach(x => x.SetTraitID("L"));
        t1 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        t2 = shop.partTraits[Random.Range(0, shop.partTraits.Count())];
        s.legs = geneManager.TraitGene(InheritanceType.Punnett, 0, t1, t2, false);

        s = geneManager.ApplyStatModifiers(s);

        return s;
    }

    

    public ShrimpStats CreatePureBreedShrimp(TraitSet set, bool adultAge)
    {
        ShrimpStats s = new ShrimpStats();

        s.name = GenerateShrimpName();
        s.gender = geneManager.RandomGender();
        if (adultAge)
            s.birthTime = TimeManager.instance.CalculateBirthTimeFromAge(geneManager.IntGene(InheritanceType.FullRandom, Mathf.RoundToInt((maxShrimpAge - (1 + minAgeForAShrimpBeingBought)) * 0.9f), 0, 0, false) + Random.value + minAgeForAShrimpBeingBought);
        else s.birthTime = TimeManager.instance.CalculateBirthTimeFromAge(geneManager.IntGene(InheritanceType.FullRandom, Mathf.RoundToInt((maxShrimpAge - 1) * 0.9f), 0, 0, false) + Random.value);

        s.temperament = geneManager.IntGene(InheritanceType.FullRandom, maxShrimpTemperament, 0, 0, false);
        s.geneticSize = geneManager.IntGene(InheritanceType.FullRandom, maxGeneticShrimpSize, 0, 0, false);
        s.hunger = 0;
        s.illnessLevel = 0;

        s.salineLevel = 50;
        s.immunity = 0;
        s.metabolism = geneManager.IntGene(InheritanceType.FullRandom, 30, 0, 0, false);
        s.filtration = 0;
        s.temperaturePreference = 50;
        s.PhPreference = 7;
        s.ammoniaPreference = 50;

        Trait t = new Trait();
        t.activeGene.ID = "C";
        s.primaryColour = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);
        s.secondaryColour = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);

        t.activeGene.ID = "P";
        s.pattern = geneManager.TraitGene(InheritanceType.WeightedRandom, 0, t, t, false);

        t.activeGene.ID = "B";
        s.body = geneManager.TraitFromSet(t, set);

        t.activeGene.ID = "H";
        s.head = geneManager.TraitFromSet(t, set);

        t.activeGene.ID = "E";
        s.eyes = geneManager.TraitFromSet(t, set);

        t.activeGene.ID = "T";
        s.tail = geneManager.TraitFromSet(t, set);

        t.activeGene.ID = "F";
        s.tailFan = geneManager.TraitFromSet(t, set);

        t.activeGene.ID = "L";
        s.legs = geneManager.TraitFromSet(t, set);

        s = geneManager.ApplyStatModifiers(s);

        return s;
    }



    public ShrimpStats CreateRequestShrimp()
    {
        ShrimpStats s = new ShrimpStats();

        s.name = GenerateShrimpName();
        s.gender = geneManager.RandomGender();
        s.birthTime = TimeManager.instance.CalculateBirthTimeFromAge(geneManager.IntGene(InheritanceType.FullRandom, Mathf.RoundToInt((maxShrimpAge - 1) * 0.9f), 0, 0, false) + Random.value);
        s.temperament = geneManager.IntGene(InheritanceType.FullRandom, maxShrimpTemperament, 0, 0, false);
        s.geneticSize = geneManager.IntGene(InheritanceType.FullRandom, maxGeneticShrimpSize, 0, 0, false);
        s.hunger = 0;
        s.illnessLevel = 0;

        s.salineLevel = 50;
        s.immunity = 0;
        s.metabolism = geneManager.IntGene(InheritanceType.FullRandom, 30, 0, 0, false);
        s.filtration = 0;
        s.temperaturePreference = 50;
        s.PhPreference = 7;
        s.ammoniaPreference = 50;

        Trait t = new Trait();
        t.activeGene.ID = "C";
        s.primaryColour = geneManager.TraitGene(InheritanceType.RandomInStore, 0, t, t, false);
        s.secondaryColour = geneManager.TraitGene(InheritanceType.RandomInStore, 0, t, t, false);

        t.activeGene.ID = "P";
        s.pattern = geneManager.TraitGene(InheritanceType.RandomInStore, 0, t, t, false);

        t.activeGene.ID = "B";
        s.body = geneManager.TraitGene(InheritanceType.RandomInStore, 0, t, t, false);

        t.activeGene.ID = "H";
        s.head = geneManager.TraitGene(InheritanceType.RandomInStore, 0, t, t, false);

        t.activeGene.ID = "E";
        s.eyes = geneManager.TraitGene(InheritanceType.RandomInStore, 0, t, t, false);

        t.activeGene.ID = "T";
        s.tail = geneManager.TraitGene(InheritanceType.RandomInStore, 0, t, t, false);

        t.activeGene.ID = "F";
        s.tailFan = geneManager.TraitGene(InheritanceType.RandomInStore, 0, t, t, false);

        t.activeGene.ID = "L";
        s.legs = geneManager.TraitGene(InheritanceType.RandomInStore, 0, t, t, false);

        s = geneManager.ApplyStatModifiers(s);

        return s;
    }



    public ShrimpStats CreateBlankShrimp()
    {
        ShrimpStats s = new ShrimpStats();

        s.name = GenerateShrimpName();
        s.gender = geneManager.RandomGender();
        s.birthTime = TimeManager.instance.GetTotalTime();
        s.hunger = 0;
        s.illnessLevel = 0;
        s.temperament = 0;

        s.primaryColour = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(geneManager.colourSOs[0].ID));
        s.secondaryColour = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(geneManager.colourSOs[0].ID));
        s.pattern = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(geneManager.patternSOs[0].ID));

        s.body = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(geneManager.bodySOs[0].ID));
        s.head = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(geneManager.headSOs[0].ID));
        s.eyes = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(geneManager.eyeSOs[0].ID));
        s.tail = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(geneManager.tailSOs[0].ID));
        s.tailFan = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(geneManager.tailFanSOs[0].ID));
        s.legs = geneManager.GlobalGeneToTrait(geneManager.GetGlobalGene(geneManager.legsSOs[0].ID));

        return s;
    }



    public void AddShrimpToStore(Shrimp s)
    {
        numberOfShrimp++;

        if (!s.loadedShrimp)  // If the game is not loading right now
        {
            geneManager.AddInstancesOfGenes(s.stats, true);
            PlayerStats.stats.totalShrimp++;
        }
    }


    public void RemoveShrimpFromStore(Shrimp s)
    {
        if (s == null) return;

        geneManager.AddInstancesOfGenes(s.stats, false);
        allShrimp.Remove(s);
    }



    public bool CheckForMoltFail(int age, ShrimpStats s, TankController t)  // Decides whether the shrimp dies of old age today. Will return true if it should.
    {
        float ageValue = shrimpNaturalDeathAge.Evaluate(age / maxShrimpAge);

        float tempValue = Mathf.Abs(s.temperaturePreference - t.waterTemperature);
        tempValue = shrimpTemperatureDeathChance.Evaluate(tempValue / 100);

        float illnessValue = Mathf.Clamp(s.illnessLevel, 0, 100);
        illnessValue = shrimpIllnessDeathChance.Evaluate(illnessValue / 100);

        float waterQualityValue = Mathf.Clamp(-t.waterQuality + 100, 0, 100);
        waterQualityValue = shrimpWaterQualityDeathChance.Evaluate(waterQualityValue / 100);


        float deathValue = ageValue + tempValue + illnessValue + waterQualityValue;
        
        if (Random.value < deathValue)
        {
            Debug.Log("Shrimp has died: " +
                "\nAge - " + ageValue + 
                "\nTemperature - " + tempValue + 
                "\nIllness - " + illnessValue + 
                "\nWater quality - " + waterQualityValue);
            return true;
        }

        return false;
    }


    public float GetMoltTime(int age) 
    { 
        return moltSpeed.Evaluate((float)age / (float)maxShrimpAge) * 60; 
    }


    public string GenerateShrimpName()
    {
        int randVal;
        int index = 0;
        Shrimp[] shrimpWithName;
        do
        {
            randVal = Random.Range(0, 21);
            index += randVal;
            while (randVal == 20)
            {
                randVal = Random.Range(0, 21);
                index += randVal;
            }

            if (index >= names.Count)
            {
                foreach(string name in names)
                {
                    shrimpWithName = allShrimp.Where(x => { return x.name == names[index]; }).ToArray();
                    if(shrimpWithName.Length == 0)
                    {
                        return name;
                    }
                }
                return "Shrimp " + allShrimp.Count.ToString();
            }

            shrimpWithName = allShrimp.Where(x => { return (x != null && x.name == names[index]); }).ToArray();

        } while (shrimpWithName.Length != 0);

        return names[index];
    }


    public int GetAdultAge()
    {
        if (adultAge == 0)
        {
            float e = 0;
            do
            {
                if (moltSpeed.Evaluate(e) >= moltSpeedRequiredToBeAdult)
                {
                    adultAge = Mathf.RoundToInt(e * maxShrimpAge);
                    break;
                }

                e += 0.01f;
            } while (e < 1);
        }

        return adultAge;
    }


    public bool IsShrimpAdult(ShrimpStats stats)
    {
        return TimeManager.instance.GetShrimpAge(stats.birthTime) >= GetAdultAge();
    }


    public Vector3 GetShrimpSize(int age, int geneticSize)
    {
        float s = 0;

        // Size over age
        if (age <= GetAdultAge())  // If they are a child
        {
            float l = Mathf.InverseLerp(0, GetAdultAge(), age);
            l = sizeThroughChildhood.Evaluate(l);
            s = Mathf.Lerp(childSize, adultSize, l);
        }
        else  // If they are an adult
        {
            float l = Mathf.InverseLerp(GetAdultAge(), maxShrimpAge, age);
            l = sizeThroughAdulthood.Evaluate(l);
            s = Mathf.Lerp(adultSize, fullyGrownSize, l);
        }

        // Genetic Size
        float gs = Mathf.InverseLerp(0, maxGeneticShrimpSize / geneticSizeImpact, geneticSize);
        gs = (-gs) + 1;
        s *= gs;

        return new Vector3(s, s, s);
    }


    public float GetBreedingCooldown(ShrimpStats stats, TankController tank)
    {
        if (stats.gender == true)  // Male
        {
            float t = maleBreedingTime.Evaluate(tank.shrimpInTank.Count);
            return Random.Range(t - maleBreedingRandomVariance, t + maleBreedingRandomVariance);
        }

        else if (stats.gender == false)  // Female
        {
            float t = femaleBreedingTime.Evaluate(tank.shrimpInTank.Count);
            return Random.Range(t - femaleBreedingRandomVariance, t + femaleBreedingRandomVariance);
        }

        return 0;
    }

    
}
