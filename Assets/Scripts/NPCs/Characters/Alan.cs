using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alan : NPC
{
    public Alan() : base("Alan@ShrimpMail.com", 40, 40, 0)
    {
        fallsAsleep = 23;
        wakesUp = 12;
    }

    public override void NpcCheck()
    {
        if (!sent && TimeManager.instance.day > lastDaySent && IsAwake())
        {
            Email email = this.CreateEmail();
            bool important = true;

            if (completion == 0)
            {
                important = true;
                email.subjectLine = "My name is Alan";
                email.mainText = "Would you like to be friends? I see that you've recently joined this little community, and so I think it's high time " +
                    "you get to know me. I like keeping in touch with all of the new store keepers. It does my heart good, seeing them all getting along." +
                    "\nI'm a great patron of the shrimp community after all, and as I say to every new shrimper, \"If you get in good with me, " +
                    "you're set for life\"";
                email.CreateEmailButton("Sure, I can be friends with you", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                email.CreateEmailButton("I dunno, I've been here a while and I haven't heard anyone mention you.", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1);
            }

            else if (completion == 1)
            {
                email.subjectLine = "Well that's disapointing";
                email.mainText = "You know, if you change your mind, I really will make it worth your time. And just so you know, I won't take no for an answer.";
                email.CreateEmailButton("Really? Fine. Sure. I'll be friends with you.", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 5);
                important = true;
            }

            else if (completion == 5)
            {
                email.subjectLine = "Well I'm glad you came around";
                email.mainText = "Isn't it nicer when we can all get along.";
                if (TimeManager.instance.day - lastDaySent > 3) email.mainText += " I don't know why you waited so long (" + (TimeManager.instance.day - lastDaySent) + " " +
                    "days, to be precise)";
                email.mainText += " And now that you have finally agreed to get along, we can get the other stuff out of the way. So, I'm going to give you some money (£100), and in " +
                    "exchange, you'll be my friend for good, right?";
                email.CreateEmailButton("Sure, I'll take the money. Not gonna say no after all!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.AddMoney, 100)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 20)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetFlag, name, "TookMoney", "Took100");
                email.CreateEmailButton("Wait, what do you mean \"in exchange\"? You're not gonna pay me to be friends with you, are you?")
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 11);
                important = true;
            }

            else if (completion == 10)
            {
                email.subjectLine = "I'm <i>So</i> Glad!";
                email.mainText = "That's just SO SWEET! Thank you very much for saying yes. As I said, getting in good with me will set you up for life, so as you've so kindy agreed to " +
                    "be my friend, do you want some money? How about £100?";
                email.CreateEmailButton("Sure, I'll take the money. Not gonna say no after all!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 20)
                    .SetFunc(EmailFunctions.FunctionIndexes.AddMoney, 100)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetFlag, name, "TookMoney", "Took100");
                email.CreateEmailButton("Wait what? I just said I would be friends with you, why would you give me money?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 11);
                important = true;
            }

            else if (completion == 11)
            {
                email.subjectLine = "How friendship works";
                email.mainText = "Look, I'll make it simple. If you want more money, you can have more money. But that <i>is</i> how friendship works. You find someone you want " +
                    "to be friends with, and then you pay them to be friends with you. Simple and clean (and tax deductible as well). So if you want to be my friend, take the money. " +
                    "It's clear you want more, so I'll give you £200 instead. But I won't take no for an answer.";
                email.CreateEmailButton("Wait £200! Sign me up!")
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 20)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetFlag, name, "TookMoney", "Took200")
                    .SetFunc(EmailFunctions.FunctionIndexes.AddMoney, 200);
                email.CreateEmailButton("Look, I'll be your friend, but I'm not taking any money for it.")
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 25)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetFlag, name, "NoMoney");
            }

            // Actually send the email
            if (email.mainText != null)
            {
                email.mainText += "\nAlan";

                NpcEmail(email, important);
            }
        }
        
    }
}
