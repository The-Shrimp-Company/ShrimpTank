using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableJemma : NPC
{
    public StableJemma() : base("I.Love.Horses@HorseMail.com", 40, 50, 0)
    {
        
    }

    /// <summary>
    /// Update for the NPCs. Each NPC's check is called in order, with one 
    /// check being called per frame.
    /// </summary>
    public override void NpcCheck()
    {
        if(!sent && TimeManager.instance.day > lastDaySent && IsAwake())
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
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3);
                important = true;
            }

            if(email.mainText != null) {
                email.mainText += "Jemma";
                NpcEmail(email, important);
            }
        }
    }
}
