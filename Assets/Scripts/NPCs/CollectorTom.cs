using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorTom : NPC
{
    public CollectorTom()
    {
        reputation = 80;
        reliability = 80;
        completion = 0;
    }

    public override void NpcCheck()
    {
        if (!sent && TimeManager.instance.day == LastDaySent)
        {
            Email email = this.CreateEmail();
            bool important = false;
            if (completion == 0 && TimeManager.instance.day > 1)
            {
                email.title = "My name is Tom";
                email.subjectLine = "Hi, I like shrimp";
                email.mainText = "You seem like a nice enough shrimper, so I thought I'd reach out. I like to keep up to" +
                    " date with all of the new shrimpers around here, keep my finger on the pulse you know.\nSo I really like shrimp" +
                    " and if you happen to find yourself with some spare shrimp around, you let me know, mkay?\n";
                email.CreateEmailButton("That's good to know", () =>
                {
                    completion = 1;
                },
                true);
                email.CreateEmailButton("Actually, I think I hate you.", () =>
                {
                    completion = -1;
                },
                true);
                important = true;
            }

            if(email.mainText != null)
            {
                email.mainText += "\nThanks,\nTom (Three time Shrimper of the Year Award winner, 17 times nominee)";

                NpcEmail(email, important);
            }
        }
        
    }
}
