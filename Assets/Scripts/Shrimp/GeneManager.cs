using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public enum InheritanceType
{
    FullRandom,
    WeightedRandom,
    Punnett,
    FlatAverage,
    RandomInStore
}

public class GeneManager : MonoBehaviour
{
    public static GeneManager instance;

    [SerializeField][Range(0, 100)] int geneWeightingPercentage = 10;  // When using weighted random, how much should it be weighted by?
    [SerializeField][Range(0, 100)] int dominanceWeightingPercentage = 50;  // When using weighted random, how much should it be weighted by?
    [SerializeField][Range(0, 100)] float mutationChance = 5;  // Will pick a random number under 100, if it is under this, the trait will mutate

    [Header("Inheritance")]
    public InheritanceType sizeInheritance = InheritanceType.WeightedRandom;
    public InheritanceType temperamentInheritance = InheritanceType.WeightedRandom;
    public InheritanceType colourInheritance = InheritanceType.Punnett;
    public InheritanceType patternInheritance = InheritanceType.Punnett;
    public InheritanceType bodyPartInheritance = InheritanceType.Punnett;

    [Header("Mutation")]
    public bool dominanceCanMutate;
    public bool sizeCanMutate;
    public bool temperamentCanMutate;
    public bool colourCanMutate = true;
    public bool patternCanMutate = true;
    public bool bodyPartCanMutate = true;
    [Space(10)]
    public int dominanceWeighting;

    [Header("Owned Genes")]
    public bool weightRandomOwnedGene;

    [Header("Traits")]
    public List<TraitSO> colourSOs = new List<TraitSO>();
    public List<TraitSO> patternSOs = new List<TraitSO>();
    public List<TraitSO> bodySOs = new List<TraitSO>();
    public List<TraitSO> headSOs = new List<TraitSO>();
    public List<TraitSO> eyeSOs = new List<TraitSO>();
    public List<TraitSO> tailSOs = new List<TraitSO>();
    public List<TraitSO> tailFanSOs = new List<TraitSO>();
    public List<TraitSO> legsSOs = new List<TraitSO>();

    private List<GlobalGene> loadedGlobalGenes = new List<GlobalGene>();

    public List<string> allTraits { get { List<TraitSO> traits = colourSOs.Concat(patternSOs.Concat(bodySOs.Concat(headSOs.Concat(eyeSOs.Concat(tailSOs.Concat(tailFanSOs.Concat(legsSOs))))))).ToList();
            List<string> traitIDs = new List<string>();
            foreach(TraitSO trait in traits)
            {
                traitIDs.Add(trait.ID);
            }
            return traitIDs;
        } }


    public void Awake()
    {
        instance = this;
        LoadGenes();
    }


    private bool FullyRandomBool()
    {
        return (Random.value > 0.5f);
    }


    private int FullyRandomInt(int upperBound)
    {
        return (Random.Range(1, upperBound + 1));
    }


    private int WeightedRandomInt(int upperBound, float weight, int parentAVal, int parentBVal)
    {
        float val = Random.Range(1, upperBound + 1);
        weight = weight / 100;  // Convert percentage to decimal
        val += (((parentAVal + parentBVal) / 2) - val) * weight;
        return Mathf.RoundToInt(val);
    }


    private Trait FullyRandomTrait(Trait parentATrait)
    {
        char type = parentATrait.activeGene.ID[0];
        Trait t = new Trait();

        if (type == 'C')
            t = RandomTraitsFromList(colourSOs);

        else if (type == 'P')
            t = RandomTraitsFromList(patternSOs);

        else if (type == 'B')
            t = RandomTraitsFromList(bodySOs);

        else if (type == 'H')
            t = RandomTraitsFromList(headSOs);

        else if (type == 'E')
            t = RandomTraitsFromList(eyeSOs);

        else if (type == 'T')
            t = RandomTraitsFromList(tailSOs);

        else if (type == 'F')
            t = RandomTraitsFromList(tailFanSOs);

        else if (type == 'L')
            t = RandomTraitsFromList(legsSOs);

        else
            Debug.Log("ID prefix " + type + " could not be found");

        if (t.activeGene.ID == null || t.activeGene.ID == "")
        {
            Debug.LogWarning("Fully Random Trait using ID " + t.activeGene.ID + " could not be found");
        }

        return t;
    }


    private Trait WeightedRandomTrait(Trait parentATrait)
    {
        Trait t = new Trait();
        if (parentATrait.activeGene.ID != null)
        {
            char type = parentATrait.activeGene.ID[0];

            if (type == 'C')
                t = WeightedRandomTraitsFromList(colourSOs);

            else if (type == 'P')
                t = WeightedRandomTraitsFromList(patternSOs);

            else if (type == 'B')
                t = WeightedRandomTraitsFromList(bodySOs);

            else if (type == 'H')
                t = WeightedRandomTraitsFromList(headSOs);

            else if (type == 'E')
                t = WeightedRandomTraitsFromList(eyeSOs);

            else if (type == 'T')
                t = WeightedRandomTraitsFromList(tailSOs);

            else if (type == 'F')
                t = WeightedRandomTraitsFromList(tailFanSOs);

            else if (type == 'L')
                t = WeightedRandomTraitsFromList(legsSOs);

            else
                Debug.Log("ID prefix " + type + " could not be found");
        }

        if (t.activeGene.ID == null || t.activeGene.ID == "")
        {
            //Debug.LogWarning("Weighted Random Trait using ID prefix " + type + " could not be found");
        }

        return t;
    }


    private Trait PunnetSquareTrait(Trait parentATrait, Trait parentBTrait)
    {
        // Seperate the genes of both traits
        Gene AA = parentATrait.activeGene;
        Gene AB = parentATrait.inactiveGene;
        Gene BA = parentBTrait.activeGene;
        Gene BB = parentBTrait.inactiveGene;

        // List all combinations of genes in seperate traits
        Trait[] c = new Trait[4];
        c[0] = new Trait(AA, BA);
        c[1] = new Trait(AA, BB);
        c[2] = new Trait(AB, BA);
        c[3] = new Trait(AB, BB);

        // Select a random index
        int chosenTrait = Random.Range(0, 4);


        Trait RT;  // Trait to return
        if (c[chosenTrait].activeGene.dominance >= c[chosenTrait].inactiveGene.dominance)  // If gene A is more dominant, set it as the active gene
        {
            RT.activeGene = c[chosenTrait].activeGene; 
            RT.inactiveGene = c[chosenTrait].inactiveGene;
        }
        else
        {
            RT.activeGene = c[chosenTrait].inactiveGene;
            RT.inactiveGene = c[chosenTrait].activeGene;
        }
        RT.obfuscated = false;
        return RT;
    }


    private Trait RandomOwnedTrait(Trait parentATrait)
    {
        Trait t = new Trait();
        if (parentATrait.activeGene.ID != null)
        {
            int errorCheck = 0;

            do
            {
                if (weightRandomOwnedGene)
                    t = WeightedRandomTrait(parentATrait);
                else
                    t = FullyRandomTrait(parentATrait);

                errorCheck++;
                if (errorCheck > 100)
                    break;
            }
            while (GetGlobalGene(t.activeGene.ID).instancesInStore > 0);
        }

        return t;
    }


    private int FlatAverageInt(int parentAVal, int parentBVal)
    {
        return Mathf.RoundToInt((parentAVal + parentBVal) / 2);
    }


    public bool RandomGender()
    {
        return FullyRandomBool();  // 0 = Female, 1 = Male
    }


    public int IntGene(InheritanceType type, int upperBound, int parentAVal, int parentBVal, bool canMutate)
    {
        if (Random.value * 100 < mutationChance)  // Gene mutation
        {
            type = InheritanceType.FullRandom;
        }

        switch (type)
        {
            case InheritanceType.FullRandom:
            {
                return FullyRandomInt(upperBound);
            }

            case InheritanceType.WeightedRandom:
            {
                return WeightedRandomInt(upperBound, geneWeightingPercentage, parentAVal, parentBVal);
            }

            case InheritanceType.Punnett:
            {
                Debug.LogWarning("Punnet squares are not supported for integer genes, please ask Aaron to implement this");
                return FullyRandomInt(upperBound);
            }

            case InheritanceType.FlatAverage:
            {
                return FlatAverageInt(parentAVal, parentBVal);
            }

            default:  // Error case
            {
                return FullyRandomInt(upperBound);
            }
        }
    }


    public Trait TraitGene(InheritanceType type, int upperBound, Trait parentAVal, Trait parentBVal, bool canMutate)
    {
        bool mutating = false;
        if (Random.value * 100 < mutationChance)  // Gene mutation
        {
            type = InheritanceType.WeightedRandom;
        }

        Trait t = new Trait();

        switch (type)
        {
            case InheritanceType.FullRandom:
            {
                t = FullyRandomTrait(parentAVal);
                break;
            }

            case InheritanceType.WeightedRandom:
            {
                t = WeightedRandomTrait(parentAVal);
                break;
            }

            case InheritanceType.Punnett:
            {
                t = PunnetSquareTrait(parentAVal, parentBVal);
                break;
            }

            case InheritanceType.FlatAverage:
            {
                Debug.LogWarning("Flat Average is not supported for traits, please ask Aaron to implement this");
                t = PunnetSquareTrait(parentAVal, parentBVal);
                break;
            }

            case InheritanceType.RandomInStore:
            {
                t = RandomOwnedTrait(parentAVal);
                break;
            }

            default:  // Error case
            {
                t = PunnetSquareTrait(parentAVal, parentBVal);
                break;
            }
        }

        if (t.activeGene.ID != null && t.activeGene.ID != "")  // If the trait actually exists
        {
            if (mutating && dominanceCanMutate)
            {
                t.activeGene.dominance = WeightDominance(GetTraitSO(t.activeGene.ID).weightDominanceTowards);
                t.inactiveGene.dominance = WeightDominance(GetTraitSO(t.inactiveGene.ID).weightDominanceTowards);
            }
            else
            {
                if (t.activeGene.dominance == 0) t.activeGene.dominance = WeightDominance(GetTraitSO(t.activeGene.ID).weightDominanceTowards);
                if (t.inactiveGene.dominance == 0) t.inactiveGene.dominance = WeightDominance(GetTraitSO(t.inactiveGene.ID).weightDominanceTowards);
            }
        }

        return t;
    }



    private void LoadGenes()
    {
        loadedGlobalGenes.Clear();

        LoadTraitType(colourSOs);
        LoadTraitType(patternSOs);
        LoadTraitType(bodySOs);
        LoadTraitType(headSOs);
        LoadTraitType(eyeSOs);
        LoadTraitType(tailSOs);
        LoadTraitType(tailFanSOs);
        LoadTraitType(legsSOs);
    }

    private void LoadTraitType(List<TraitSO> l)
    {
        EconomyManager em = GetComponent<EconomyManager>();
        foreach (TraitSO t in l)
        {
            GlobalGene g = new GlobalGene();
            g.ID = t.ID;
            g.dominance = WeightDominance(t.weightDominanceTowards);
            g.startingValue = em.SetInitialGeneValue(g.dominance);
            g.currentValue = g.startingValue;
            g.trueValue = g.currentValue;
            loadedGlobalGenes.Add(g);
        }
    }


    public TraitSO GetTraitSO(string ID)
    {
        TraitSO t = null;

        if (ID != null && ID != "")
        {
            if (ID[0] == 'C')
                t = GetSOFromList(colourSOs, ID);

            else if (ID[0] == 'P')
                t = GetSOFromList(patternSOs, ID);

            else if (ID[0] == 'B')
                t = GetSOFromList(bodySOs, ID);

            else if (ID[0] == 'H')
                t = GetSOFromList(headSOs, ID);

            else if (ID[0] == 'E')
                t = GetSOFromList(eyeSOs, ID);

            else if (ID[0] == 'T')
                t = GetSOFromList(tailSOs, ID);

            else if (ID[0] == 'F')
                t = GetSOFromList(tailFanSOs, ID);

            else if (ID[0] == 'L')
                t = GetSOFromList(legsSOs, ID);

            else
                Debug.Log("ID " + ID + " prefix could not be found");
        }


        if (t == null)
        {
            Debug.LogWarning("Trait SO with ID " + ID + " could not be found");
        }

        return t;
    }

    private TraitSO GetSOFromList(List<TraitSO> l, string ID)
    {
        foreach (TraitSO t in l)
        {
            if (t.ID == ID)
            {
                return t;
            }
        }

        return null;
    }


    private Trait RandomTraitsFromList(List<TraitSO> l)
    {
        if (l.Count == 0) return new Trait();

        return new Trait(
            GetGlobalGene(l[Random.Range(0, l.Count)].ID), 
            GetGlobalGene(l[Random.Range(0, l.Count)].ID));
    }


    private Trait RandomTraitFromList(List<TraitSO> l)
    {
        if (l.Count == 0) return new Trait();

        return GlobalGeneToTrait(GetGlobalGene(l[Random.Range(0, l.Count)].ID));
    }


    private Trait TraitFromSetFromList(List<TraitSO> l, TraitSet set)
    {
        if (l.Count == 0) return new Trait();

        int i = 0;
        if (set == TraitSet.Cherry) i = 0;
        else if (set == TraitSet.Nylon) i = 1;
        else if (set == TraitSet.Anomalis) i = 2;
        else if (set == TraitSet.Caridid) i = 3;

        return new Trait(
            GetGlobalGene(l[i].ID),
            GetGlobalGene(l[i].ID));
    }


    private Trait WeightedRandomTraitsFromList(List<TraitSO> l)
    {
        Trait r = new Trait();
        if (l.Count == 0) return r;

        // Total rarity
        int totalRarity = 0;
        foreach(TraitSO t in l)
        {
            totalRarity += GetGlobalGene(t.ID).dominance;
        }


        // Active gene
        int rand = Random.Range(0, totalRarity);
        int i = 0;
        foreach (TraitSO t in l)
        {
            i += GetGlobalGene(t.ID).dominance;
            if (rand <= i)
            {
                r.activeGene = GlobalGeneToGene(GetGlobalGene(t.ID));
                break;
            }
        }

        // Inactive gene
        rand = Random.Range(0, totalRarity);
        i = 0;
        foreach (TraitSO t in l)
        {
            i += GetGlobalGene(t.ID).dominance;
            if (rand <= i)
            {
                r.inactiveGene = GlobalGeneToGene(GetGlobalGene(t.ID));
                break;
            }
        }

        // Error message
        if (r.activeGene.ID == null || r.activeGene.ID == "" || r.inactiveGene.ID == null || r.inactiveGene.ID == "")
        {
            Debug.LogWarning("Weighted Random Trait failed. Rand - " + rand + ". Total Rarity - " + totalRarity);
        }

        return r;
    }


    public Trait TraitFromSet(Trait parentATrait, TraitSet set)
    {
        char type = parentATrait.activeGene.ID[0];
        Trait t = new Trait();

        if (type == 'C')
            t = TraitFromSetFromList(colourSOs, set);

        else if (type == 'P')
            t = TraitFromSetFromList(patternSOs, set);

        else if (type == 'B')
            t = TraitFromSetFromList(bodySOs, set);

        else if (type == 'H')
            t = TraitFromSetFromList(headSOs, set);

        else if (type == 'E')
            t = TraitFromSetFromList(eyeSOs, set);

        else if (type == 'T')
            t = TraitFromSetFromList(tailSOs, set);

        else if (type == 'F')
            t = TraitFromSetFromList(tailFanSOs, set);

        else if (type == 'L')
            t = TraitFromSetFromList(legsSOs, set);

        else
            Debug.Log("ID prefix " + type + " could not be found");

        if (t.activeGene.ID == null || t.activeGene.ID == "")
        {
            Debug.LogWarning("Fully Random Trait using ID " + t.activeGene.ID + " could not be found");
        }

        return t;
    }


    public GlobalGene GetGlobalGene(string ID)
    {
        GlobalGene r = new GlobalGene();

        foreach (GlobalGene g in loadedGlobalGenes)
        {
            if (g.ID == ID)
            {
                r = g; 
                break;
            }
        }

        if (r.ID == null || r.ID == "")
        {
            Debug.LogWarning("Global gene with ID " + ID + " could not be found");
        }

        return r;
    }


    public int GetGlobalGeneIndex(string ID)
    {
        int r = -1;

        int i = 0;
        foreach (GlobalGene g in loadedGlobalGenes)
        {
            if (g.ID == ID)
            {
                r = i;
                break;
            }
            i++;
        }

        if (r == -1)
        {
            Debug.LogWarning("Global gene with ID " + ID + " could not be found");
        }

        return r;
    }


    public void SetGlobalGene(GlobalGene g)
    {
        for (int i = 0; i < loadedGlobalGenes.Count - 1; i++)
        {
            if (loadedGlobalGenes[i].ID == g.ID)
            {
                loadedGlobalGenes[i] = g;
                return;
            }
        }
    }


    public Gene GlobalGeneToGene(GlobalGene gene)
    {
        Gene g = new Gene();
        g.ID = gene.ID;
        //g.value = gene.currentValue;
        g.dominance = gene.dominance;
        return g;
    }


    public Trait GlobalGeneToTrait(GlobalGene gene)
    {
        return new Trait(gene, gene);
    }


    public Trait GeneToTrait(Gene gene)
    {
        return new Trait(gene, gene);
    }


    public ShrimpStats ApplyStatModifiers(ShrimpStats s)
    {
        s = ApplyStatModifier(s.primaryColour.activeGene.ID, s);
        s = ApplyStatModifier(s.secondaryColour.activeGene.ID, s);

        s = ApplyStatModifier(s.pattern.activeGene.ID, s);

        s = ApplyStatModifier(s.body.activeGene.ID, s);
        s = ApplyStatModifier(s.head.activeGene.ID, s);
        s = ApplyStatModifier(s.eyes.activeGene.ID, s);
        s = ApplyStatModifier(s.tail.activeGene.ID, s);
        s = ApplyStatModifier(s.tailFan.activeGene.ID, s);
        s = ApplyStatModifier(s.legs.activeGene.ID, s);

        return s;
    }


    private ShrimpStats ApplyStatModifier(string ID, ShrimpStats s)
    {
        if (ID != null && ID != "")
        {
            foreach (Modifier m in GetTraitSO(ID).statModifiers)
            {
                switch (m.type)
                {
                    case ModifierEffects.temperament:
                        {
                            s.temperament = Mathf.Clamp(s.temperament + m.effect, 0, 100);
                            break;
                        }

                    case ModifierEffects.geneticSize:
                        {
                            s.geneticSize = Mathf.Clamp(s.geneticSize + m.effect, 0, 100);
                            break;
                        }
                    case ModifierEffects.salineLevel:
                        {
                            s.salineLevel = Mathf.Clamp(s.salineLevel + m.effect, 0, 100);
                            break;
                        }
                    case ModifierEffects.immunity:
                        {
                            s.immunity = Mathf.Clamp(s.immunity + m.effect, 0, 100);
                            break;
                        }
                    case ModifierEffects.metabolism:
                        {
                            s.metabolism = Mathf.Clamp(s.metabolism + m.effect, 0, 100);
                            break;
                        }
                    case ModifierEffects.filtration:
                        {
                            s.filtration = Mathf.Clamp(s.filtration + m.effect, 0, 100);
                            break;
                        }
                    case ModifierEffects.temperature:
                        {
                            s.temperaturePreference = Mathf.Clamp(s.temperaturePreference + m.effect, 0, 100);
                            break;
                        }
                    case ModifierEffects.ammonia:
                        {
                            s.ammoniaPreference = Mathf.Clamp(s.temperaturePreference + m.effect, 0, 100);
                            break;
                        }
                    case ModifierEffects.ph:
                        {
                            s.PhPreference = Mathf.Clamp(s.PhPreference + m.effect, 0, 14);
                            break;
                        }
                }
            }
        }

        return s;
    }


    public int WeightDominance(int w)
    {
        float val = Random.Range(1, 101);
        float weighting = dominanceWeightingPercentage / 100;  // Convert percentage to decimal
        val += (w - val) * weighting;
        return Mathf.RoundToInt(val);
    }


    public bool CheckForPureShrimp(ShrimpStats s)
    {
        TraitSet ts = GetTraitSO(s.body.activeGene.ID).set;

        if (GetTraitSO(s.head.activeGene.ID).set != ts) return false;
        if (GetTraitSO(s.eyes.activeGene.ID).set != ts) return false;
        if (GetTraitSO(s.tail.activeGene.ID).set != ts) return false;
        if (GetTraitSO(s.tailFan.activeGene.ID).set != ts) return false;
        if (GetTraitSO(s.legs.activeGene.ID).set != ts) return false;

        //Debug.Log("Pure Shrimp!");
        return true;
    }


    public bool CheckForPureColourShrimp(ShrimpStats s)
    {
        if (s.primaryColour.activeGene.ID != s.secondaryColour.activeGene.ID) return false;

        //Debug.Log("Pure Colour Shrimp!");
        return true;
    }


    public void ReturnGeneValues(int p)
    {
        int numberOfGenesToUpdate = Mathf.RoundToInt(Mathf.Lerp(0, loadedGlobalGenes.Count - 1, p / 100));

        for (int i = numberOfGenesToUpdate; i >= 0; i--)
        {
            int rand = Random.Range(0, loadedGlobalGenes.Count);
            EconomyManager.instance.ValueReturn(loadedGlobalGenes[rand]);
        }
    }

    public void DailyGeneValueUpdate(int p)
    {
        int numberOfGenesToUpdate = Mathf.RoundToInt(Mathf.Lerp(0, loadedGlobalGenes.Count - 1, p / 100));

        for (int i = numberOfGenesToUpdate; i >= 0; i--)
        {
            int rand = Random.Range(0, loadedGlobalGenes.Count);
            EconomyManager.instance.DailyValueUpdate(loadedGlobalGenes[rand]);
        }

        for(int i = 0; i < loadedGlobalGenes.Count; i++)
        {
            GlobalGene gene = loadedGlobalGenes[i];
            gene.currentValue = loadedGlobalGenes[i].trueValue;
            loadedGlobalGenes[i] = gene;
        }
    }

    public void AddInstancesOfGenes(ShrimpStats s, bool add)
    {
        AddInstanceOfGene(GetGlobalGeneIndex(s.primaryColour.activeGene.ID), add);
        AddInstanceOfGene(GetGlobalGeneIndex(s.secondaryColour.activeGene.ID), add);
        AddInstanceOfGene(GetGlobalGeneIndex(s.pattern.activeGene.ID), add);

        AddInstanceOfGene(GetGlobalGeneIndex(s.body.activeGene.ID), add);
        AddInstanceOfGene(GetGlobalGeneIndex(s.head.activeGene.ID), add);
        AddInstanceOfGene(GetGlobalGeneIndex(s.eyes.activeGene.ID), add);
        AddInstanceOfGene(GetGlobalGeneIndex(s.tail.activeGene.ID), add);
        AddInstanceOfGene(GetGlobalGeneIndex(s.tailFan.activeGene.ID), add);
        AddInstanceOfGene(GetGlobalGeneIndex(s.legs.activeGene.ID), add);
    }

    private void AddInstanceOfGene(int i, bool add)
    {
        if (i != -1)
        {
            GlobalGene g;
            g = loadedGlobalGenes[i];
            g.instancesInStore += add ? 1 : -1;
            if (add) g.lifetimeInstances++;
            loadedGlobalGenes[i] = g;
        }
    }

    public GlobalGene[] GetGlobalGeneArray()
    {
        return loadedGlobalGenes.ToArray();
    }

}
