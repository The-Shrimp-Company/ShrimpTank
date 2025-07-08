using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNPC : NPC
{
    public DebugNPC()
    {
        name = "DebugNPC";
    }

    public override void NpcCheck()
    {
        if (!sent)
        {
            Email email = this.CreateEmail();
            bool important = false;
            email.title = "DEBUG";
            email.subjectLine = "fghfghgghf";
            email.mainText = "ahhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n" +
                "ahhhhhh\n";


            if (email.mainText != null)
            {
                email.mainText += "\nThanks,\nTom (Three time Shrimper of the Year Award winner, 17 times nominee)";

                NpcEmail(email, important);
            }
        }
    }
}
