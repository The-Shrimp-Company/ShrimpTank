using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleShopScript : ScreenView
{
    [SerializeField]
    private GameObject shrimpPreview;
    [SerializeField]
    private Transform contentParent;

    private List<ContentBlock> ContentBlocks = new List<ContentBlock>();


    public void Populate(Shop shop)
    {
        foreach (ShrimpStats stats in shop.shrimpStock)
        {
            ShrimpSelectionBlock block = Instantiate(shrimpPreview, contentParent).GetComponent<ShrimpSelectionBlock>();
            block.Populate(stats);
            block.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (Store.SpawnShrimp(stats, EconomyManager.instance.GetShrimpValue(stats)))
                {
                    shop.shrimpStock.Remove(block.GetComponent<ShrimpSelectionBlock>().GetShrimp());
                    EconomyManager.instance.UpdateTraitValues(true, stats);
                    Destroy(block.gameObject);
                }
            });
            ContentBlocks.Add(block);
        }
    }
}
