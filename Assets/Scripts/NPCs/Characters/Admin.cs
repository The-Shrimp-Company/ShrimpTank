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
            if(completion == 3)
            {
                email.subjectLine = "Checking your own store";
                email.mainText = "You can also see your own store, to get information about what is in your store. This will tell you how many shrimp you have, and what your reputation score is" +
                    " at. Access this screen now.";
                important = true;
            }
            if (completion == 4)
            {
                email.subjectLine = "Buying shrimp apparatus";
                email.mainText = "From what the system shows, you already have several tanks in your store, but what you don't have is any shrimp food. You can buy this from the store, which you " +
                    "now have access to. Buy some shrimp food now.";
                important = true;
            }
            if(completion == 5)
            {
                email.subjectLine = "Using the vet";
                email.mainText = "Sometimes, when you have more shrimp, you may notice illnesses spreading through your store, and your tanks. The best way of fighting these illnesses is, of course," +
                    " to not let them happen in the first place, but sometimes this is not possible, and for that reason the community includes a very good shrimp vetinary service. You can find plenty " +
                    "useful information in this service, and as you expand the store, you will get further access to the vetinary services.";
                important = true;
            }
            if(completion == 10)
            {
                email.subjectLine = "Account Fully Activated";
                email.mainText = "Your account is now fully activated, and you will be able to recieve emails from other shrimp store owners, and members of the community. Please be civil, and if you need " +
                    "assistance, please find support.";
                email.CreateEmailButton("Find support where?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 11)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "AccountActivated");
                important = true;
            }

            if (Reputation.GetReputation() >= 20 && Reputation.GetReputation() < 40 && !flags.Contains("star1"))
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
            Email email = EmailManager.instance.emails.Find((x) => { return x.sender == name; });
            if (email != null && email.buttons == null)
            {
                if (completion == 2 && PlayerStats.stats.shrimpBought > 0)
                {
                    email.CreateEmailButton("I've bought a shrimp", true)
                        .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "OwnStoreOpen")
                        .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3);
                }
                if (completion == 3 && PlayerStats.stats.timesSellingAppOpened > 0)
                {
                    email.CreateEmailButton("I've opened the store app", true)
                        .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "UpgradeStoreOpen")
                        .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 4);
                }
                if (completion == 4 && PlayerStats.stats.timesBoughtFood > 0)
                {
                    email.CreateEmailButton("I've bought some food", true)
                        .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "VetOpen")
                        .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 5);
                }
                if(completion == 5 && PlayerStats.stats.timesVetOpened > 0)
                {
                    email.CreateEmailButton("I've opened the vet", true)
                        .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                }
            }
        }
    }
}
