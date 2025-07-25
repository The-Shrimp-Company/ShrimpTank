using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTemplate : NPC
{
    public NPCTemplate() : base("Template", 0, 0, 0)
    {
        
    }

    /// <summary>
    /// Update for the NPCs. Each NPC's check is called in order, with one 
    /// check being called per frame.
    /// </summary>
    public override void NpcCheck()
    {
        if(!sent && TimeManager.instance.day > lastDaySent)
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
