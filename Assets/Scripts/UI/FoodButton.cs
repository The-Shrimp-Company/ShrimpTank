using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemCount;
    [SerializeField] private TextMeshProUGUI itemName;

    public void AssignFood(Item food, TankController tank)
    {
        itemCount.text = Inventory.GetItemQuantity(food).ToString();
        itemName.text = food.itemName;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (Inventory.HasItem(food) && !tank.FedShrimpToday())
            {
                Inventory.RemoveItem(food);
                tank.FeedShrimp();
                itemCount.text = Inventory.GetItemQuantity(food).ToString();
                if(Inventory.GetItemQuantity(food) <= 0)
                {
                    Destroy(gameObject);
                }
            }
        });
    }
}
