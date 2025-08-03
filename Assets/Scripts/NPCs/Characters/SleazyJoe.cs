using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SleazyJoe : NPC
{


    public SleazyJoe() : base("Joe@ShrimpMail.com", 10, 50, 0)
    {
        if (flags.Count < 1)
        {
            flags.Add("0");
        }
    }

    public override void NpcCheck()
    {
        if (!sent && TimeManager.instance.day > lastDaySent && IsAwake())
        {
            Email email = this.CreateEmail();
            bool important = true;

            if(completion == 0 && ShrimpManager.instance.allShrimp.Count > 1)
            {
                email.mainText = "Hey, I noticed you had some shrimp, is there uhhhh, any chance you could ummmmm, sell some to me for ummm, dirt cheap maybe?";
                email.title = "New shrimp shop!";
                email.subjectLine = "YEAAAAAAHHHH";
                email.CreateEmailButton("Yes", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1);
                important = true;
            }
            else if (flags[0].TryCast<int>() >= 30 && completion == 1)
            {
                email.title = "Thanks so much";
                email.subjectLine = "The shrimp have been great";
                email.mainText = "I really appreciate all of the shrimp you've given me now, and I know that they've been worth more than I was paying you." +
                    "\nI still don't have much money, but you've inspired me to get out of my current job, and try and get into shrimp breeding as well! I hope" +
                    " I can do as well as you, and I hope you like the gift!";
                email.CreateEmailButton("Accept the gift", true).SetFunc(EmailFunctions.FunctionIndexes.SpawnShrimp, ShrimpManager.instance.CreateRandomShrimp(true));
                completion = 2;
                important = true;
            }
            else if(completion >= 1)
            {
                email.mainText = "Thanks for offering me some shrimp. I'd really like one, but I don't have much cash. Could you sell me one of your shrimp for £" + Math.Round((float)completion + flags[0].TryCast<int>()/10, 2) + ". I don't mind which one.";
                email.title = "Please";
                email.subjectLine = "Please";
                email.CreateEmailButton("I will sell you this one", false).SetFunc(EmailFunctions.FunctionIndexes.GiveAnyShrimp, completion + flags[0].TryCast<int>()/10);
                important = true;
            }

            if (email.mainText != null)
            {
                email.mainText += "\n\nI love SHRIMP (also my name is Joe)";

                NpcEmail(email, important);
            }
        }
    }

    public override void BoughtShrimp(ShrimpStats stats)
    {
        flags[0] = (flags[0].TryCast<int>() + EconomyManager.instance.GetShrimpValue(stats) - completion + flags[0].TryCast<int>() / 10).ToShortString();
    }
}
