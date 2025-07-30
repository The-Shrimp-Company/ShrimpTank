using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNPC : NPC
{
    public DebugNPC() : base("Debug")
    {

    }

    public override void NpcCheck()
    {
        if (!sent)
        {
            Email email = this.CreateEmail();
            bool important = false;
            email.subjectLine = "fghfghgghf";
            email.mainText = completion.ToString();
            completion += 1;


            if (email.mainText != null)
            {
                email.mainText += "\nThanks,\nTom (Three time Shrimper of the Year Award winner, 17 times nominee)";

                NpcEmail(email, important);
            }
        }
    }
}
