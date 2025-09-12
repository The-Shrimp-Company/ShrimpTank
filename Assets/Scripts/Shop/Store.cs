using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(DecorateShopController))]
public class Store : MonoBehaviour
{
    public static string StoreName;
    public static GameObject player;
    public static DecorateShopController decorateController;



    void Awake()
    {
        decorateController = GetComponent<DecorateShopController>();
        player = GameObject.Find("Player");
    }






    public static bool SpawnShrimp(ShrimpStats s, float price)
    {
        if (Money.instance.WithdrawMoney(price))
        {
            s.name = ShrimpManager.instance.GenerateShrimpName();
            Inventory.AddShrimp(s);
            PlayerStats.stats.shrimpBought++;

            Email email = EmailTools.CreateEmail();
            email.title = "YourStore@notifSystem.store";
            email.subjectLine = "A new shrimp has arrived in the store";
            email.mainText = "The shrimp is in your storage";
            EmailManager.SendEmail(email);

            return true;
        }

        return false;
    }
}
