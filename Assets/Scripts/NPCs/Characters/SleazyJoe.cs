using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SleazyJoe : NPC
{


    public SleazyJoe() : base("Joe@ShrimpMail.com", 10, 50)
    {
        if (flags.Count < 1)
        {
            flags.Add("2");
        }
    }

    public override void NpcCheck()
    {
        if (TimeManager.instance.day > lastDaySent && IsAwake())
        {
            Email email = this.CreateEmail();
            bool important = true;

            if(completion == 0 && ShrimpManager.instance.allShrimp.Count > 1)
            {
                email.mainText = "Hey, I heard there was a new shrimp shop open, and I just wanted to say congrats! This must be a dream come true for you! I love the name by the way," +
                    " " + Store.StoreName + " is just the perfect name for a shrimp store, and it has so much <b>character</b>, you know. What am I talking about, of <b>course</b> you know, " +
                    "you named it! Also, I'm kinda looking to get some more shrimp, but I'm running a little low on cash right now, is there any chance you could maybe give me a discount? " +
                    "I'll take any shrimp you have, as long as it's cheap enough. I won't even mind if their second hand, I'll take anything I can get.";
                email.subjectLine = "You're the new Shrimp Shop right?";
                email.CreateEmailButton("Sure I can give you some shrimp for cheap, but they probably won't be very good...", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                email.CreateEmailButton("What do you mean by \"even if their second hand\"?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1);
                email.CreateEmailButton("I'm not too sure about that to be honest.", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3);
                important = true;
            }
            if(completion == 1)
            {
                email.subjectLine = "Second hand shrimp";
                email.mainText = "You didn't know? Oh, I guess you are <b>really</b> new to this, so that makes sense. Look, people round here pay attention to where shrimp come from, " +
                    "and people are gonna pay more for a shrimp that you bred yourself than for one you bought from someone else. The shrimp'll be younger, it's less likely to have had " +
                    "an illness, and travel for shrimps can be very stressful. Did <b>no one</b> tell you this? Try and make sure you read through the vet guides at some point, because they'll " +
                    "explain a lot of this stuff.";
                email.CreateEmailButton("Oh, thanks for explaining that", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 2);
                important = true;
            }
            if(completion == 2)
            {
                email.subjectLine = "So about my question...";
                email.mainText = "So, <b>are</b> you gonna sell me some shrimp for cheap?";
                email.CreateEmailButton("Yeah, sure, you've been nice enough", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                email.CreateEmailButton("I'm still not too sure about that", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3);
            }
            if(completion == 3)
            {
                email.subjectLine = "Aww, that's a shame.";
                email.mainText = "Please though? No one else will sell any shrimp to me at all. You're the only one left. I'll find a way to pay you back some day, I promise!";
                email.CreateEmailButton("Ahh, sure. I'll sell you some shrimp for cheap", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                email.CreateEmailButton("Nah, I think I don't want to sell you anything", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 7000);
            }
            else if (flags[0].TryCast<float>() >= 30 && completion == 10)
            {
                email.title = "Thanks so much";
                email.subjectLine = "The shrimp have been great";
                email.mainText = "I really appreciate all of the shrimp you've given me now, and I know that they've been worth more than I was paying you." +
                    "\nI still don't have much money, but you've inspired me to get out of my current job, and try and get into shrimp breeding as well! I hope" +
                    " I can do as well as you, and I hope you like the gift!";
                email.CreateEmailButton("Accept the gift", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SpawnShrimp, ShrimpManager.instance.CreateRandomShrimp(true))
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 2)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, NPCManager.Instance.NPCs.Find(x => x.GetType() == typeof(Rival)), 1001);
                important = true;
            }
            else if(completion == 10 && TimeManager.instance.day > lastDaySent + 1)
            {
                email.mainText = "Thanks for offering me some shrimp. I'd really like one, but I don't have much cash. Could you sell me one of your shrimp for £" + (flags[0].TryCast<float>()/10).RoundMoney() + ". I don't mind which one.";
                email.title = "Please";
                email.subjectLine = "Please";
                email.CreateEmailButton("I will sell you this one", false)
                    .SetFunc(EmailFunctions.FunctionIndexes.GiveAnyShrimp, completion + flags[0].TryCast<float>()/10)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1);
                important = true;
            }

            if(completion == 7000)
            {
                email.sender = "";
                email.subjectLine = "I think you might have made a mistake";
                email.mainText = "<size=30><color=purple>Do you want to try again? I can reset time, just by a bit, to let you try that again, because as things stand, you're going to lose out on accessing " +
                    "all the content of this game. I'm not going to tell you whats changed, but I'm sure you can figure it out. Try not to let us have this conversation again.</color></size>";
                email.CreateEmailButton("I'll try", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 0);
                email.CreateEmailButton("I'll try", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 0);
            }

            if (email.mainText != null)
            {
                data.completion.Dequeue();
                email.mainText += "\n\nI love SHRIMP (also my name is Joe)";

                NpcEmail(email, important);
            }
            else
            {
                if (data.completion.Count > 1)
                {
                    data.completion.Enqueue(data.completion.Dequeue());
                }
            }
        }
    }

    public override void BoughtShrimp(ShrimpStats stats)
    {
        flags[0] = (flags[0].TryCast<float>() + EconomyManager.instance.GetShrimpValue(stats) - completion + flags[0].TryCast<float>() / 5).ToShortString();
        Debug.Log(flags[0]);
    }
}
