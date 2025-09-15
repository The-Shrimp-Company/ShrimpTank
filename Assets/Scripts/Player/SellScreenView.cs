using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellScreenView : ScreenView
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject shrimpSlot;

    private List<ShrimpSlotScript> slots = new();

    public override void Open(bool switchTab)
    {
        if (!content) return;
        foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }
        for(int i = 0; i < CustomerManager.Instance.numSlots; i++)
        {
            slots.Add(Instantiate(shrimpSlot, content).GetComponent<ShrimpSlotScript>());
            slots[i].SetValues(i);
        }
    }
}
