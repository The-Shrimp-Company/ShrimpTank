using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;
using System.Linq;

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
}
