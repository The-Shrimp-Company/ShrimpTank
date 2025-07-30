using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Admin : NPC
{
    public Admin() : base("Admin@admin.ShrimpCo.com", 100, 100, 0)
    {
        if(completion == 0)
        {
            lastDaySent = -10;
        }
    }

    public override void NpcCheck()
    {
        if (!sent)
        {
            Email email = this.CreateEmail();
            bool important = false;
            if(completion == 0)
            {
                email.subjectLine = "Account activation";
                email.mainText = "You must activate your account to be able to sell shrimp.";
                important = true;
                email.CreateEmailButton("Activate Account", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "activeAccount");
            }

            if(completion == 1)
            {
                email.subjectLine = "Welcome to the Shrimping Community, " + Store.StoreName;
                email.mainText = "We have installed the community apps on your device for your convienience. You may now use the community sale page, the community vet and" +
                    " the community supply store. Your email address has also been added to the community ring, so customers can send you requests for shrimp directly." +
                    "\n Please note that your behaviour on these services is being monitored, and your access to these services may be limited if you have too low of a reputation score. " +
                    "\nLike all new members, your reputation score is low, and so your access to some services is limited, but as you prove yourself, you can build up your standing in the community," +
                    " and many things will become easier for you. \nIf you have any questions, queries or complaints, please find support.";
                email.CreateEmailButton("Find support where?", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 2);
                important = true;
            }

            if(Reputation.GetReputation() >= 20 && Reputation.GetReputation() < 40 && !flags.Contains("star1"))
            {
                email.subjectLine = "You have achieved Star 1";
                email.mainText = "Congratulations " + Store.StoreName + ", on achieving the first reputation star! This is a big achievment, and now you can access many " +
                    "new features of the community, like the tank store.";
                email.CreateEmailButton("Thanks!", true).SetFunc(EmailFunctions.FunctionIndexes.SetFlag, name, "star1");
                important = true;
            }

            if (email.mainText != null)
            {
                email.mainText += "\n\nAdmin";

                NpcEmail(email, 0, important);
            }
        }
    }
}
