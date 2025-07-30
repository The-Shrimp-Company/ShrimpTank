using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Admin : NPC
{
    public Admin() : base("Admin@admin.ShrimpCo.com", 100, 100, 0)
    {
        if(completion == 0)
        {
            lastDaySent = -10;
        }
    }

    public override void NpcCheck()
    {
        if (!sent)
        {
            Email email = this.CreateEmail();
            bool important = false;
            if(completion == 0)
            {
                email.subjectLine = "Account activation";
                email.mainText = "You must activate your account to be able to sell shrimp.";
                important = true;
                email.CreateEmailButton("Activate Account", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1);
            }

            if(completion == 1)
            {
                email.subjectLine = "Welcome to the Shrimping Community, " + Store.StoreName;
                email.mainText = "We have installed the community apps on your device for your convienience. You may choose to either have access to all of these applications now, " +
                    "or we can give you access over time, to walk you through the options available. Which would you like to pick?";
                email.CreateEmailButton("Walk me through the systems", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 2)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "ShrimpStoreOpen");
                email.CreateEmailButton("Give me everything (Not recommended for new players)", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "ShrimpStoreOpen")
                    .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "OwnStoreOpen")
                    .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "UpgradeStoreOpen")
                    .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "VetOpen");
                important = true;
            }

            if(completion == 2)
            {
                email.subjectLine = "Buying shrimp";
                email.mainText = "You have opted into a slow introduction to shrimping. The first thing you need to know is how to buy shrimp. You have now been given access to the shrimp " +
                    "store, where you can access other's stores, and you can buy shrimp from them. You should do that now.";
                important = true;
            }

            if(Reputation.GetReputation() >= 20 && Reputation.GetReputation() < 40 && !flags.Contains("star1"))
            {
                email.subjectLine = "You have achieved Star 1";
                email.mainText = "Congratulations " + Store.StoreName + ", on achieving the first reputation star! This is a big achievment, and now you can access many " +
                    "new features of the community, like the tank store.";
                email.CreateEmailButton("Thanks!", true).SetFunc(EmailFunctions.FunctionIndexes.SetFlag, name, "star1");
                important = true;
            }

            if (email.mainText != null)
            {
                email.mainText += "\n\nAdmin";

                NpcEmail(email, 0, important);
            }
        }
        else
        {
            if(completion == 2 && PlayerStats.stats.shrimpBought > 0)
            {
                Email email = EmailManager.instance.emails.Find((x) => { return x.sender == name; });
                if(email.buttons == null)
                {
                    email.CreateEmailButton("I've bought a shrimp", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "OwnStoreOpen")
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3);
                }
            }
        }
    }
}
