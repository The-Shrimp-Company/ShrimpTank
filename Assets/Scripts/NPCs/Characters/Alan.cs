using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alan : NPC
{
    public Alan() : base("Alan@ShrimpMail.com", 40, 40, 0)
    {

    }

    public override void NpcCheck()
    {
        if (!sent && TimeManager.instance.day > lastDaySent)
        {
            Email email = this.CreateEmail();
            bool important = false;
            if (completion == 0)
            {
                important = true;
                email.title = "Press E to open your tablet";
                email.subjectLine = "My name is Alan";
                email.mainText = "I have an offer for you~!";
                email.CreateEmailButton("press here for FRIENDS", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                email.CreateEmailButton("press here to disappoint me", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, -1);
            }
            else if (completion == 10)
            {
                email.subjectLine = "I'm glad you said yes!";
                email.title = "Alan is HAPPY";
                email.mainText = "Have some money!";
                email.CreateEmailButton("Press here for MONEY", true).SetFunc(EmailFunctions.FunctionIndexes.AddMoney, 100);
                completion = 12;
                important = true;
            }
            else if (completion == -1)
            {
                email.subjectLine = "WHY????!!!??!!?! :(";
                email.title = "Alan";
                email.mainText = "Sad Now :(";
                email.CreateEmailButton("Continue to disapoint", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, -1);
                email.CreateEmailButton("press here for FRIENDS", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }
            else if (completion > 10)
            {
                email.subjectLine = "I can count";
                email.title = "This is what friends do";
                email.mainText = "AAAA" + completion;
                completion++;
                important = false;
            }
            if(email.mainText != null)
            {
                email.mainText += "\nAlan";

                NpcEmail(email, important);
            }
        }
        
    }
}
