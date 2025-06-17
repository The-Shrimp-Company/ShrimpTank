using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alan : NPC
{
    public Alan()
    {
        reputation = 40;
        reliability = 40;
        completion = 0;
    }

    public override void NpcCheck()
    {
        if(completion == 0)
        {
            Email email = new Email();
            email.important = true;
            email.title = "Alan";
            email.subjectLine = "My name is ALAN";
            email.mainText = "I have an offer for you~!";
            email.CreateEmailButton("press here for FRIENDS", () =>
            {
                completion = 10;
            },
            true);
            email.CreateEmailButton("press here to disappoint me", () =>
            {
                completion = -1;
            },
            true);
            EmailManager.SendEmail(email, true, 1);
            completion = 1;
        }
        else if(completion == 10)
        {
            Email email = new Email();
            email.subjectLine = "I'm glad you said yes!";
            email.title = "Alan";
            email.mainText = "Have some money!";
            email.CreateEmailButton("Press here for MONEY", () =>
            {
                Money.instance.AddMoney(100);
            },
            true);
            completion = 12;
            EmailManager.SendEmail(email, true, 2);
        }
        else if(completion == -1)
        {
            Email email = new Email();
            email.subjectLine = "WHY????!!!??!!?! :(";
            email.title = "Alan";
            email.mainText = "Sad Now :(";
            email.CreateEmailButton("Continue to disapoint", () =>
            {
                completion = -1;
            },
            true);
            email.CreateEmailButton("press here for FRIENDS", () =>
            {
                completion = 10;
            },
            true);
            completion = -2;
            EmailManager.SendEmail(email, true, 5);
        }
    }
}
