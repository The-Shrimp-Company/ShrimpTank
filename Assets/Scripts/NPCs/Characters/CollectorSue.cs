using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorSue : NPC
{

    private float lastHourSent;

    public CollectorSue() : base("Sue@ShrimpMail.com", 80, 80, 0)
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
        if (!sent && (TimeManager.instance.day > lastDaySent || TimeManager.instance.hour > lastHourSent + 1) && IsAwake())
        {
            Email email = this.CreateEmail();
            bool important = false;
            if (completion == 0)
            {
                email.title = "My name is Sue";
                email.subjectLine = "Hi, I like shrimp";
                email.mainText = "You seem like a nice enough shrimper, so I thought I'd reach out. I like to keep up to" +
                    " date with all of the new shrimpers around here, keep my finger on the pulse you know.\nSo I really like shrimp" +
                    " and if you happen to find yourself with some spare shrimp around, you let me know, mkay?\n";
                email.CreateEmailButton("That's good to know", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1);
                important = true;
            }

            if(completion == 1)
            {
                email.title = "Some Advice";
                email.subjectLine = "Everyone needs a hand at some point";
                email.mainText = "I just thought I'd clue you in on some stuff. First of all, you have figured out how to check" +
                    " your emails, so that's good. However, just in case you haven't figured out some of the other useful tools" +
                    " on your tablet, let me give you a rundown. First of all, there is the shop. You can buy shrimp there from" +
                    " other shrimpers, if you have a good enough reputation that they're willing to sell to you. You can also access" +
                    " your own shops stats in the your shop page, which is where you can see your own reputation. " +
                    " \nAnother useful tool is the vet. There are several paid services there, but there's also a free compendium of" +
                    " shrimp tips.";
                email.CreateEmailButton("Thanks for the advice", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 2);
                important = true;
            }


            if(email.mainText != null)
            {
                email.mainText += "\nThanks,\nSue (Three time Shrimper of the Year Award winner, 17 times nominee)";

                NpcEmail(email, important);
            }
        }
        
    }
}
