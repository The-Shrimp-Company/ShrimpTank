using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableJemma : NPC
{
    public StableJemma() : base("I.Love.Horses@HorseMail.com", 40, 50)
    {
        
    }

    /// <summary>
    /// Update for the NPCs. Each NPC's check is called in order, with one 
    /// check being called per frame.
    /// </summary>
    public override void NpcCheck()
    {
        if(TimeManager.instance.day > lastDaySent && IsAwake())
        {
            Email email = this.CreateEmail();
            bool important = false;

            //Place the actual email logic here
            if(TimeManager.instance.day > 5 && completion == 0)
            {
                email.subjectLine = "Can I trust you?";
                email.mainText = "I have some shrimp which I would Like to sell, but I need them to Go to a good home. How do I know that you're Not going to abuse these poor" +
                    " Disgusting creatures?";
                email.CreateEmailButton("That seems kind of mean?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1);
                email.CreateEmailButton("I promise you can trust me, I really love shrimp", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 2);
                email.CreateEmailButton("How can I trust you?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 1)
            {
                email.subjectLine = "I don't think so";
                email.mainText = "I don't think it's Being mean to make sure you're Not going to abuse these Helpless, horrifying Creatures! " +
                    "I don't mean it personally, I just need to Know before I can get them off My hands!";
                email.CreateEmailButton("No, I meant it seems a bit mean calling shrimp disgusting", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3);
                important = true;
            }

            if(completion == 2)
            {
                email.subjectLine = "You seem suspicious";
                email.mainText = "I'm not sure I can Trust someone who's so Eager about shrimp. I don't think You'll take good care Of them.";
                email.CreateEmailButton("Wait, no! You can trust me with shrimp! I promise I'll take really good care of them!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 4);
                email.CreateEmailButton("Well I don't think I can trust you then.", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 3)
            {
                email.subjectLine = "Do you expect me to believe you";
                email.mainText = "I don't believe you, You know. No one would actually Think this about shrimp. That's simply not How this works. I really don't Think that " +
                    "I can trust you.";
                email.CreateEmailButton("Well how do I know I can trust you, then?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 4)
            {
                email.subjectLine = "You seem way too eager";
                email.mainText = "I don't think I can trust Someone who's so eager to Look after these horrible shrimp Properly. Why would Anyone want that?";
                email.CreateEmailButton("Actually, you don't seem very trustworthy right now!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 10)
            {
                email.subjectLine = "That's a Fair point";
                email.mainText = "That is very Fair. How can you Trust me? You've never met Me. I exist, in your Perspective, exclusively in the Context of your " +
                    "Email Client. You have no Knowledge of my existence other than through a Screen, a screen Which lies to you all the Time anyway. I guess I must " +
                    "trust You, for to not Trust you would be indicate that I Cannot be trusted, and to allow you To not trust me would imply that I Cannot trust myself. " +
                    "\n\nSorry if this is getting too philisophical for you, I understand that horse raising is more intellectually profound than shrimp raising, and that " +
                    "I shouldn't expect such feats of Cerebrality from such a hobbyist. Either way, you now Have my trust.";
                email.CreateEmailButton("What on earth are you talking about?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 11);
                email.CreateEmailButton("It's nice that you trust me!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 12);
                important = true;
            }

            if(email.mainText != null)
            {
                data.completion.Dequeue();
                email.mainText += "\nJemma";
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
}
