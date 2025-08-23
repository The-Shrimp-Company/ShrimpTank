using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    public DecorationItemSO decorationSO;
    public bool locked;  // Whether the item can be removed
    [HideInInspector] public Dictionary<MeshRenderer, Material[]> materials = new Dictionary<MeshRenderer, Material[]>();
    [HideInInspector] public bool floating;
    public List<Transform> shelfSlots = new List<Transform>();

    private void Awake()
    {
        MeshRenderer[] meshes = gameObject.GetComponentsInChildren<MeshRenderer>();

        if (meshes[0] == null) return;

        foreach (MeshRenderer me in meshes)
        {
            var ma = me.materials;

            materials.Add(me, ma);
        }
    }

    public void ResetMaterials()
    {
        foreach (MeshRenderer me in materials.Keys)
        {
            if (materials.ContainsKey(me))
                me.materials = materials[me];
        }
    }
}
