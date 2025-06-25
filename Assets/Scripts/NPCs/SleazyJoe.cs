using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleazyJoe : NPC
{
    public SleazyJoe()
    {
        reputation = 10;
        reliability = 50;
        completion = 0;
    }

    public override void NpcCheck()
    {
        if (!sent)
        {
            Email email = new Email();
            bool important = false;

            if(completion == 0 && ShrimpManager.instance.allShrimp.Count > 1)
            {
                email.mainText = "Hey, I noticed you had some shrimp, is there uhhhh, any chance you could ummmmm, sell some to me for ummm, dirt cheap maybe?";
                email.title = "New shrimp shop!";
                email.subjectLine = "YEAAAAAAHHHH";
                email.CreateEmailButton("Yes", () =>
                {
                    completion = 1;
                }
                , true);
                important = true;
            }

            if(completion >= 1)
            {
                email.mainText = "Thanks for offering me some shrimp. I'd really like one, but I don't have much cash. Could you sell me one of your shrimp for £" + completion + ". I don't mind which one.";
                email.title = "Please";
                email.subjectLine = "Please";
                email.CreateEmailButton("I will sell you this one", () =>
                {
                    CustomerManager.Instance.emailScreen.OpenFullSelection(completion);
                    completion += 1;
                }, false);
                important = true;
            }

            if (email.mainText != null)
            {
                email.mainText += "\n\nI love SHRIMP (also my name is Joe)";

                NpcEmail(email, important);
            }
        }
    }
}
