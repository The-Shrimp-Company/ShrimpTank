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
                email.mainText = "I have some shrimp which I would like to sell, but I need them to go to a good home. How do I know that you're not going to abuse these poor" +
                    " disgusting creatures?";
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
                email.mainText = "I don't think it's being mean to make sure you're not going to abuse these helpless, horrifying creatures! " +
                    "I don't mean it personally, I just need to know before I can get them off my hands!";
                email.CreateEmailButton("No, I meant it seems a bit mean calling shrimp disgusting", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3);
                important = true;
            }

            if(completion == 2)
            {
                email.subjectLine = "You seem suspicious";
                email.mainText = "I'm not sure I can trust someone who's so eager about shrimp. I don't think you'll take good care of them.";
                email.CreateEmailButton("Wait, no! You can trust me with shrimp! I promise I'll take really good care of them!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 4);
                email.CreateEmailButton("Well I don't think I can trust you then.")
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 3)
            {
                email.subjectLine = "Do you expect me to believe you";
                email.mainText = "I don't believe you, you know. No one would actually think this about shrimp. That's simply not how this works. I really don't think that " +
                    "I can trust you.";
                email.CreateEmailButton("Well how do I know I can trust you, then?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 4)
            {
                email.subjectLine = "You seem way too eager";
                email.mainText = "I don't think I can trust someone who's so eager to look after these horrible shrimp properly. Why would anyone want that?";
                email.CreateEmailButton("Actually, you don't seem very trustworthy right now!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 10)
            {
                email.subjectLine = "That's a fair point";
                email.mainText = "That is very fair. How can you trust me? You've never met me. I exist, in your perspective, exclusively in the context of your " +
                    "email client. You have no knowledge of my existence other than through a screen, a screen which lies to you all the time anyway. I guess I must " +
                    "trust you, for to not trust you would be indicate that I cannot be trusted, and to allow you to not trust me would imply that I cannot trust myself. " +
                    "\n\nSorry if this is getting too philisophical for you, I understand that horse raising is more intellectually profound than shrimp raising, and that " +
                    "I shouldn't expect such feats of cerebrality from such a hobbyist. Either way, you now have my trust.";
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
