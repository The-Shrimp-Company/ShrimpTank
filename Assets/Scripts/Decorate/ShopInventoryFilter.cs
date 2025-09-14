using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI;
using System.Linq;

public class ShopInventoryFilter : MonoBehaviour
{
    [SerializeField] ShopInventory inventory;
    [SerializeField] List<ItemTags> tabFilters;
    [SerializedDictionary("Filter", "Item Tag")]
    public SerializedDictionary<Button, ItemTags> subFilters;
    public bool allTab;


    public List<ItemTags> GetTabFilters()
    {
        return tabFilters;
    }

    public List<ItemTags> GetActiveSubFilters()
    {
        List<ItemTags> tags = new List<ItemTags>();

        foreach (Button x in subFilters.Keys)
        {
            if (x.GetComponent<Image>().color == inventory.tabSelectedColour)
                tags.Add(subFilters[x]);
        }

        return tags;
    }


    public void SelectFilter(Button b)
    {
        if (b.GetComponent<Image>().color != inventory.tabSelectedColour)
            b.GetComponent<Image>().color = inventory.tabSelectedColour;
        else
            b.GetComponent<Image>().color = inventory.tabDeselectedColour;

        inventory.ChangeSelectedItem(null, null);
        inventory.UpdateContent();
    }


    public void DeselectAllFilters()
    {
        foreach (Button x in subFilters.Keys)
        {
            x.GetComponent<Image>().color = inventory.tabDeselectedColour;
        }
    }
}