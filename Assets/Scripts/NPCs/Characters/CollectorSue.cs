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
                email.CreateEmailButton("Sounds like a good deal", true).SetFunc(EmailFunctions.FunctionIndexes.SetFlag, name, "WillBuy")
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 2);
                important = true;
                data.completion.Dequeue();
            }

            if(completion == 2 && shrimpBought.Count > 3)
            {
                email.subjectLine = "Thanks for helping";
                email.mainText = "You've sold me a few shrimp now, and it's really helped out! I'm glad you've become a productive member of the community. I'll keep" +
                    " buying shrimp from you, so you just make sure you keep your shrimp healthy.";
                email.CreateEmailButton("Thanks, will do!", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3);
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

            if(completion == 3000)
            {
                email.subjectLine = "About your \"rival\"";
                email.mainText = "I've heard that you have had some trouble with a member of the community? Rival has started harrasing you? Look, " +
                    "they can be difficult to talk to, and they can get some funny old ideas into their head when they want to (they sent me a long message " +
                    "telling me that you were the best rival they could, so I think they're having fun at least), but I hope you know it's not something " +
                    "to take too seriously. Or at the very least, it's not something to let hurt your feelings (I know kids these days like to make things competitive)." +
                    " If you want them to stop, you just let me know, and I'll get the admin to do something about it. It really needs to take action about ensuring that " +
                    "the newcomers are playing nice with one another you know, and I've been meaning to file a complaint for a while, so I'd be more than happy to do that" +
                    " if you want deary.";
                if(shrimpBought.Count > 0)
                {
                    email.mainText += "\n\n(by the way, " + shrimpBought[shrimpBought.Count-1].name + " is doing great since I bought them from you, such a wonderful addition to " +
                        "my collection)";
                }
                email.CreateEmailButton("No, I won't need and help with Rival, I think it's fun", true);
                email.CreateEmailButton("If you want to make a complaint I won't stop you", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, NPCManager.Instance.NPCs.Find((x) => x.GetType() == typeof(Admin)), 3000);
                important = true;
                data.completion.Dequeue();
            }

            if (email.mainText != null)
            {
                email.mainText += "\nThanks,\nSue (Three time Shrimper of the Year Award winner, 17 times nominee)";

                NpcEmail(email, important);
            }
            else
            {
                if(data.completion.Count > 1)
                {
                    data.completion.Enqueue(data.completion.Dequeue());
                }
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
