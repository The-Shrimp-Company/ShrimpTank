using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSlot : MonoBehaviour
{
    [SerializeField] private GameObject protoButton;
    [SerializeField] private TankViewScript tankView;


    // Start is called before the first frame update
    void Start()
    {
        foreach(Item item in Inventory.GetInventoryItemsWithTag(ItemTags.Food))
        {
            Instantiate(protoButton, transform).GetComponent<FoodButton>().AssignFood(item, tankView.GetTank());
        }
    }
}
