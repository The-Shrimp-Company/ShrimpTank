using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Decoration : MonoBehaviour
{
    public DecorationItemSO decorationSO;
    public bool locked;  // Whether the item can be removed
    [HideInInspector] public Dictionary<MeshRenderer, Material[]> materials = new Dictionary<MeshRenderer, Material[]>();
    [HideInInspector] public bool floating;
    [HideInInspector] public Interactable interactable;

    private void Awake()
    {
        MeshRenderer[] meshes = gameObject.GetComponentsInChildren<MeshRenderer>();

        if (meshes[0] == null) return;

        foreach (MeshRenderer me in meshes)
        {
            var ma = me.materials;

            materials.Add(me, ma);
        }

        if (decorationSO.tags.Contains(ItemTags.RoomDecoration))
        {
            if (GetComponent<Interactable>()) interactable = GetComponent<Interactable>();
            else if (GetComponentInChildren<Interactable>()) interactable = GetComponentInChildren<Interactable>();
            else
            {
                interactable = transform.GetChild(0).gameObject.AddComponent(typeof(Interactable)) as Interactable;
                interactable.interactable = false;
            }
        }
    }

    private void Start()
    {
        if (!locked && interactable)
        {
            interactable.AddHoldAction("Move Decoration", MoveDecoration);
            interactable.AddHoldAction("Remove Decoration", RemoveDecoration);
            interactable.decoration = this;
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

    public void MoveDecoration()
    {
        Debug.Log("Move");
    }

    public void RemoveDecoration()
    {
        Debug.Log("Remove");
    }

    public bool CheckForItemsOnShelf()
    {
        if (!decorationSO.shelf) return false;
        foreach(ShelfGridNode node in transform.GetComponentsInChildren<ShelfGridNode>())
        {
            if (node.roomGridNode.invalid) return true;
        }
        return false;
    }
}
