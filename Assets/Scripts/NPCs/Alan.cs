using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alan : NPC
{
    public Alan()
    {
        reputation = 40;
        reliability = 40;
        completion = 0;
    }

    public override void NpcCheck()
    {
        if (!sent && TimeManager.instance.day > LastDaySent)
        {
            Email email = this.CreateEmail();
            bool important = false;
            if (completion == 0)
            {
                important = true;
                email.title = "Alan";
                email.subjectLine = "My name is ALAN";
                email.mainText = "I have an offer for you~!";
                email.CreateEmailButton("press here for FRIENDS", () =>
                {
                    completion = 10;
                },
                true);
                email.CreateEmailButton("press here to disappoint me", () =>
                {
                    completion = -1;
                },
                true);
            }
            else if (completion == 10)
            {
                email.subjectLine = "I'm glad you said yes!";
                email.title = "Alan is HAPPY";
                email.mainText = "Have some money!";
                email.CreateEmailButton("Press here for MONEY", () =>
                {
                    Money.instance.AddMoney(100);
                },
                true);
                completion = 12;
                important = true;
            }
            else if (completion == -1)
            {
                email.subjectLine = "WHY????!!!??!!?! :(";
                email.title = "Alan";
                email.mainText = "Sad Now :(";
                email.CreateEmailButton("Continue to disapoint", () =>
                {
                    completion = -1;
                },
                true);
                email.CreateEmailButton("press here for FRIENDS", () =>
                {
                    completion = 10;
                },
                true);
                important = true;
            }
            else if (completion > 10)
            {
                email.subjectLine = "I can count";
                email.title = "This is what friends do";
                email.mainText = "AAAA" + completion;
                completion++;
                important = false;
            }
            if(email.mainText != null)
            {
                email.mainText += "\nAlan";

                NpcEmail(email, important);
            }
        }
        
    }
}
