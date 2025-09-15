using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using UnityEngine.UI;
using System.Linq;

public class ItemShopFilter : MonoBehaviour
{
    [SerializeField] ItemShop shop;
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
            if (x.GetComponent<Image>().color == shop.tabSelectedColour)
                tags.Add(subFilters[x]);
        }

        return tags;
    }


    public void SelectFilter(Button b)
    {
        if (b.GetComponent<Image>().color != shop.tabSelectedColour)
            b.GetComponent<Image>().color = shop.tabSelectedColour;
        else
            b.GetComponent<Image>().color = shop.tabDeselectedColour;

        shop.ChangeSelectedItem(null, null);
        shop.UpdateContent();
    }


    public void DeselectAllFilters()
    {
        foreach (Button x in subFilters.Keys)
        {
            x.GetComponent<Image>().color = shop.tabDeselectedColour;
        }
    }
}