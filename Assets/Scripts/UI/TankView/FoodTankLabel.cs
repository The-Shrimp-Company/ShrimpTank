using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FoodTankLabel : MonoBehaviour
{
    [SerializeField] private TankViewScript tankView;

    // Update is called once per frame
    void Update()
    {
        Dictionary<string, int> shrimpFood = new Dictionary<string, int>();
        string message = "";
        if(tankView.GetTank().foodInTank != null)
        {
            foreach (ShrimpFood item in tankView.GetTank().foodInTank)
            {
                if (shrimpFood.ContainsKey(item.thisItem.itemName))
                {
                    shrimpFood[item.thisItem.itemName] += 1;
                }
                else
                {
                    shrimpFood.Add(item.thisItem.itemName, 1);
                }
            }
            foreach (KeyValuePair<string, int> entry in shrimpFood)
            {
                message += entry.Key + ":" + entry.Value.ToString() + "\n";
            }
        }
        message = message.Trim('\n');
        GetComponent<TextMeshProUGUI>().text = message;
    }
}
