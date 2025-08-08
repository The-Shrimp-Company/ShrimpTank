using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorSue : NPC
{

    private float lastHourSent;

    public CollectorSue() : base("Sue@ShrimpMail.com", 80, 80)
    {
        fallsAsleep = 19;
        wakesUp = 8;
    }

    public override void EmailDestroyed()
    {
        base.EmailDestroyed();
        lastHourSent = TimeManager.instance.hour;
    }

    public override void NpcCheck()
    {
        if ((TimeManager.instance.day > lastDaySent) && IsAwake())
        {
            Email email = this.CreateEmail();
            bool important = false;
            if (completion == 0)
            {
                email.subjectLine = "Hi, I collect shrimp";
                email.mainText = "You seem like a nice enough shrimper, so I thought I'd reach out. I like to keep up to" +
                    " date with all of the new shrimpers around here, keep my finger on the pulse you know.\nSo I really like shrimp" +
                    " and if you happen to find yourself with some spare shrimp around, you let me know, mkay?\n";
                email.CreateEmailButton("That's good to know", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1);
                important = true;
                data.completion.Dequeue();
            }

            if(completion == 1)
            {
                email.subjectLine = "My Goal";
                email.mainText = "So I'm currently trying to collect as many unique shrimp as possible, hopefully to end up having one of each possible type " +
                    "of shrimp. I've actually given everyone else in the community the same deal (well nearly everyone), but basically I only buy shrimp I don't" +
                    " have a good store of, but I will basically buy any shrimp I don't have enough stock of. Don't worry, I pay good money, and my word means " +
                    "quite a lot in the community (I'm very well known), so I'll be sure to let people know if the shrimp are healthy.";
                email.CreateEmailButton("Sounds like a good deal", true).SetFunc(EmailFunctions.FunctionIndexes.SetFlag, name, "WillBuy");
                important = true;
                data.completion.Dequeue();
            }

            if(flags.Contains("WillBuy"))
            {
                flags.Remove("WillBuy");
                email.subjectLine = "Could I have a shrimp please";
                email.mainText = "I am in need of a shrimp, are any of your's new?";
                email.CreateEmailButton("I have some shrimp you can look at", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.GiveSueShrimp, shrimpBought);
                important = true;
            }

            if (email.mainText != null)
            {
                email.mainText += "\nThanks,\nSue (Three time Shrimper of the Year Award winner, 17 times nominee)";

                NpcEmail(email, important);
            }
        }   
    }

    public override void BoughtShrimp(ShrimpStats stats)
    {
        shrimpBought.Add(stats);
        if(shrimpBought.Count > 10)
        {
            shrimpBought.RemoveAt(0);
        }
    }
}
