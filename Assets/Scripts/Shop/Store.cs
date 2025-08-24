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
    public static DecorateShopController decorateController;

    private static TankController destinationTank;



    void Awake()
    {
        decorateController = GetComponent<DecorateShopController>();
    }



    public static TankController GetDestinationTank() { return destinationTank; }
    public static void SwitchDestinationTank(TankController newTank)
    {
        // If we previously had no sale tank
        if (destinationTank == null)
        {
            destinationTank = newTank;
            destinationTank.ToggleDestinationTank();
        }
        else if (destinationTank != newTank)
        {
            // Switch the old one
            destinationTank.ToggleDestinationTank();
            destinationTank = newTank;
            // Switch the new one
            destinationTank.ToggleDestinationTank();
        }

        ShrimpManager.instance.destTank = destinationTank;
    }



    public static bool SpawnShrimp(ShrimpStats s, float price)
    {
        if (destinationTank != null)
        {
            if (Money.instance.WithdrawMoney(price))
            {
                s.name = ShrimpManager.instance.GenerateShrimpName();
                destinationTank.SpawnShrimp(s);
                PlayerStats.stats.shrimpBought++;

                Email email = EmailTools.CreateEmail();
                email.title = "YourStore@notifSystem.store";
                email.subjectLine = "A new shrimp has arrived in the store";
                email.mainText = "The shrimp is in " + destinationTank.tankName;
                EmailManager.SendEmail(email);

                return true;
            }
        }
        return false;
    }
}
