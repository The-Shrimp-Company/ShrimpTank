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
        heldItem = i;
    }

    public void StopHoldingItem()
    {
        heldItem = null;
    }
}
