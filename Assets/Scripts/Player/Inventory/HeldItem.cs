using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    [SerializeField] Transform handTransform;
    private Item heldItem;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public Item GetHeldItem() { return heldItem; }

    public void HoldItem(Item i)
    {
        StopHoldingItem();
        heldItem = i;
        HeldItemChanged();
    }

    public void StopHoldingItem()
    {
        if (heldItem == null) return;

        heldItem = null;
        HeldItemChanged();
    }

    private void HeldItemChanged()
    {
        if (handTransform.childCount > 0) Destroy(handTransform.GetChild(0).gameObject);
        
        if (heldItem != null)
        {
            ItemSO so = Inventory.GetSOForItem(heldItem);
            if (so == null || so.heldItemPrefab == null) return;

            GameObject go = Instantiate(so.heldItemPrefab, handTransform.position, handTransform.rotation, handTransform);
        }
    }
}
