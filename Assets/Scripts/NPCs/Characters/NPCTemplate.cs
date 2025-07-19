using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTemplate : NPC
{
    public NPCTemplate()
    {
        reputation = 0;
        reliability = 0;
        completion = 0;
        name = "Template";
    }

    /// <summary>
    /// Update for the NPCs. Each NPC's check is called in order, with one 
    /// check being called per frame.
    /// </summary>
    public override void NpcCheck()
    {
        if(!sent && TimeManager.instance.day > LastDaySent)
        {
            Email email = this.CreateEmail();
            bool important = false;

            //Place the actual email logic here

            if(email.mainText != null) {
                email.mainText += "Signiture";
                NpcEmail(email, important);
            }
        }
    }
}
