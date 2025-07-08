using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Admin : NPC
{
    public Admin()
    {
        reputation = 100;
        reliability = 100;
        completion = 0;
        name = "Admin@admin.ShrimpCo.com";
    }

    public override void NpcCheck()
    {
        if (!sent && (TimeManager.instance.day > LastDaySent + 1))
        {
            Email email = this.CreateEmail();
            bool important = false;
            if(completion == 0)
            {
                email.title = "Account activation";
                email.subjectLine = "Account activation";
                email.mainText = "You must activate your account to be able to sell shrimp.";
                important = true;
                email.CreateEmailButton("Activate Account", () =>
                {
                    completion = 1;
                    Tutorial.instance.flags.activeAccount = true;
                }, true);
            }

            if (email.mainText != null)
            {
                email.mainText += "\n\nAdmin";

                NpcEmail(email, important);
            }
        }
    }
}
