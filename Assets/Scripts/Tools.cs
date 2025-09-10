using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEngine;

public static class Tools
{
    public static void SetLayerRecursively(this Transform parent, int layer)
    {
        parent.gameObject.layer = layer;

        for (int i = 0, count = parent.childCount; i < count; i++)
        {
            parent.GetChild(i).SetLayerRecursively(layer);
        }
    }

    public static List<GameObject> FindDescendants(this GameObject obj)
    {
        List<GameObject> list = new List<GameObject>();

        foreach(Transform child in obj.transform)
        {
            list.Add(child.gameObject);
            list.AddRange(child.gameObject.FindDescendants());
        }

        return list;
    }

    static public NPC.NPCData NpcValidation(this NPC npc, NPC.NPCData[] data)
    {
        if(data != null)
        {
            data = data.Where((x) => { return x.name == npc.name; }).ToArray();
        }
        else
        {
            return null;
        }
        if (data.Length > 0)
        {
            return data[0];
        }
        else
        {
            return null;
        }
    }


    #nullable enable
    static public T? TryCast<T>(this object obj)
    {
        if (obj.GetType() == typeof(JsonElement))
        {
            try
            {
                return JsonSerializer.Deserialize<T>((JsonElement)obj, new JsonSerializerOptions() { IncludeFields = true });
            }
            catch
            {
                return default(T);
            }
        }
        else
        {
            try
            {
                return (T)obj;
            }
            catch
            {
                return default(T);
            }
        }
    }

    static public string RoundMoney(this float num)
    {
        string str = Math.Round(num, 2).ToString();
        string regEx = @"^\d*\.\d$";
        Regex regex = new Regex(regEx);
        if (regex.IsMatch(str))
        {
            str += "0";
        }
        else if (!str.Contains("."))
        {
            str += ".00";
        }

        return str;
    }

    public static Dictionary<TraitSet, int> CountBreed(this ShrimpStats stats)
    {
        Dictionary<TraitSet, int> traitCount = new Dictionary<TraitSet, int>() { { TraitSet.Nylon, 0 }, { TraitSet.Anomalis, 0 }, { TraitSet.Caridid, 0 }, { TraitSet.Cherry, 0 } };

        if (!stats.body.obfuscated) traitCount[GeneManager.instance.GetTraitSO(stats.body.activeGene.ID).set]++;
        if (!stats.legs.obfuscated) traitCount[GeneManager.instance.GetTraitSO(stats.legs.activeGene.ID).set]++;
        if (!stats.head.obfuscated) traitCount[GeneManager.instance.GetTraitSO(stats.head.activeGene.ID).set]++;
        if (!stats.eyes.obfuscated) traitCount[GeneManager.instance.GetTraitSO(stats.eyes.activeGene.ID).set]++;
        if (!stats.tail.obfuscated) traitCount[GeneManager.instance.GetTraitSO(stats.tail.activeGene.ID).set]++;
        if (!stats.tailFan.obfuscated) traitCount[GeneManager.instance.GetTraitSO(stats.tailFan.activeGene.ID).set]++;

        return traitCount;
    }

    public static string GetBreedname(this ShrimpStats stats)
    {
        Dictionary<TraitSet, int> traitCount = stats.CountBreed();

        string breedname = "";

        if (traitCount[TraitSet.Cherry] == 6)
        {
            return "Pure Cherry";
        }
        else if (traitCount[TraitSet.Cherry] >= 2)
        {
            breedname += "Cherry ";
        }

        if (traitCount[TraitSet.Nylon] == 6)
        {
            return "Pure Armoured Nylon";
        }
        else if (traitCount[TraitSet.Nylon] >= 2)
        {
            breedname += "Nylon ";
        }

        if (traitCount[TraitSet.Caridid] == 6)
        {
            return "Pure Caridid";
        }
        else if (traitCount[TraitSet.Caridid] >= 2)
        {
            breedname += "Caridid ";
        }

        if (traitCount[TraitSet.Anomalis] == 6)
        {
            return "Pure Anomalis";
        }
        else if (traitCount[TraitSet.Anomalis] >= 2)
        {
            breedname += "Anomalis ";
        }

        breedname += "Mix";

        return breedname;
    }

    public static void SetTraitID(this Trait trait, string ID)
    {
        trait.activeGene.ID = ID + trait.activeGene.ID.Substring(1);
        trait.inactiveGene.ID = ID + trait.inactiveGene.ID.Substring(1);
    }
}
