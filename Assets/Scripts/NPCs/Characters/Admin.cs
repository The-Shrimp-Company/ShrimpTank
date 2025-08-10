using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Admin : NPC
{
    public Admin() : base("Admin@admin.ShrimpCo.com", 100, 100)
    {
        if(completion == 0)
        {
            lastDaySent = -10;
        }
    }

    public override void NpcCheck()
    {
        Email email = this.CreateEmail();
        bool important = true;
        if(completion == 0)
        {
            email.subjectLine = "Account activation";
            email.mainText = "You must activate your account to be able to sell shrimp.";
            important = true;
            email.CreateEmailButton("Activate Account", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1);
        }

        if(completion == 1)
        {
            email.subjectLine = "Welcome to the Shrimping Community, " + Store.StoreName;
            email.mainText = "We have installed the community apps on your device for your convienience. You may choose to either have access to all of these applications now, " +
                "or we can give you access over time, to walk you through the options available. Which would you like to pick?";
            email.CreateEmailButton("Walk me through the systems", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 2)
                .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "ShrimpStoreOpen");
            email.CreateEmailButton("Give me everything (Not recommended for new players)", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10)
                .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "ShrimpStoreOpen", "OwnStoreOpen", "UpgradeStoreOpen", "VetOpen");
            important = true;
        }

        if(completion == 2)
        {
            email.subjectLine = "Buying shrimp";
            email.mainText = "You have opted into a slow introduction to shrimping. The first thing you need to know is how to buy shrimp. You have now been given access to the shrimp " +
                "store, where you can access other's stores, and you can buy shrimp from them. You should do that now.";
            important = true;
        }
        if(completion == 3)
        {
            email.subjectLine = "Checking your own store";
            email.mainText = "You can also see your own store, to get information about what is in your store. This will tell you how many shrimp you have, and what your reputation score is" +
                " at. Access this screen now.";
            important = true;
        }
        if (completion == 4)
        {
            email.subjectLine = "Buying shrimp apparatus";
            email.mainText = "From what the system shows, you already have several tanks in your store, but what you don't have is any shrimp food. You can buy this from the store, which you " +
                "now have access to. Buy some shrimp food now.";
            important = true;
        }
        if(completion == 5)
        {
            email.subjectLine = "Using the vet";
            email.mainText = "Sometimes, when you have more shrimp, you may notice illnesses spreading through your store, and your tanks. The best way of fighting these illnesses is, of course," +
                " to not let them happen in the first place, but sometimes this is not possible, and for that reason the community includes a very good shrimp vetinary service. You can find plenty " +
                "useful information in this service, and as you expand the store, you will get further access to the vetinary services.";
            important = true;
        }
        if(completion == 10)
        {
            email.subjectLine = "Account Fully Activated";
            email.mainText = "Your account is now fully activated, and you will be able to recieve emails from other shrimp store owners, and members of the community. Please be civil, and if you need " +
                "assistance, please find support.";
            email.CreateEmailButton("Find support where?", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 11)
                .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "AccountActivated");
            important = true;
        }

        if(completion == 3000)
        {
            email.subjectLine = "Complaint Investigation";
            email.mainText = "User Sue (Sue@ShrimpMail.com) has a logged a complaint against User Rival (Rival@YourRivalMail.com) on your behalf. " +
                "As part of our response to this complain, I will need to gather some information from you, directly. And not from User Sue (Sue@ShrimpMail.com). " +
                "A complaint on behalf of someone else requires additional investigation, which User Sue (Sue@ShrimpMail.com) should be well aware of, and yet User Sue (Sue@ShrimpMail.com) " +
                "chose to do this anyway, meaning I now need to ask you yet more questions. The first question is this:" +
                " Has User Rival (Rival@YourRivalMail.com) been negatively impacting your experience in this community?";
            email.CreateEmailButton("No", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3001);
            email.CreateEmailButton("Yes", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3004);
            important = true;
        }

        if(completion == 3001)
        {
            email.subjectLine = "Complaint Investigation";
            email.mainText = "So, based on your previous responses, User Rival (Rival@YourRivalMail.com) is having no negative impact on your experience in this community." +
                " Does this mean that User Sue (Sue@ShrimpMail.com) filed this complaint in error?";
            email.CreateEmailButton("No", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3002);
            email.CreateEmailButton("Yes", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3003);
        }

        if(completion == 3002)
        {
            email.subjectLine = "Inconsistency detected";
            email.mainText = "You appear to have stated an inconsistency. You claimed that User Rival (Rival@YourRivalMain.com) was not causing you any negative experiences, " +
                "but also that the report filed on your behalf by User Sue (Sue@ShrimpMail.com) about User Rival (Rival@RivalMain.com) causing you negative experiences " +
                "was not in error. This is an inconcistency, and will lead to the complaint being ignored, and all information you have provided being discarded. You are not " +
                "allowed to respond.";
            email.CreateEmailButton("What do you mean \"I can't respond\"?", true);
        }

        if(completion == 3003)
        {
            email.subjectLine = "Sue's Error";
            email.mainText = "User Sue (Sue@ShrimpMail.com) has been a member of this community for a very long time, and so this false report will be ignored. It is good to know " +
                "that User Rival (Rival@RivalMail.com) is not causing any negative experiences. No further actions need to be taken, and this investigation is now over. You do not need to, " +
                "and as such cannot, respond";
            email.CreateEmailButton("What do you mean \"I can't respond\"?", true);
        }

        if(completion == 3004)
        {
            email.subjectLine = "Investigation Continuation";
            email.mainText = "It is very troubling to hear that you have experienced negative experiences in the community because of a member of the community. As a small " +
                "shrimping community, there are very few things which can be done to prevent User Rival (Rival@RivalMail.com) from being able to make you experience negative " +
                "experiences in the community, but the few things which can be done will be done. Would you like a strongly worded letter to be written to User Rival (Rival@RivalMail.com)?";
            email.CreateEmailButton("Yes", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, NPCManager.Instance.NPCs.Find((x) => x.GetType() == typeof(Rival)), 6000);
            email.CreateEmailButton("No", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3005);
        }

        if(completion == 3005)
        {
            email.subjectLine = "Milder Letter?";
            email.mainText = "If we cannot send a strongly worded letter to User Rival (Rival@RivalMail.com) would you instead like a weakly worded letter to be sent to " +
                "User Rival (Rival@RivalMail.com)?";
            email.CreateEmailButton("Yes", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, NPCManager.Instance.NPCs.Find((x) => x.GetType() == typeof(Rival)), 6000);
            email.CreateEmailButton("No", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3006);
        }

        if(completion == 3006)
        {
            email.subjectLine = "Mind powers?";
            email.mainText = "If we cannot send a weakly worded letter to User Rival (Rival@RivalMail.com) would you instead like intent to stop causing negative experiences to be " +
                "thought at User Rival (Rival@RivalMail.com)?";
            email.CreateEmailButton("Yes", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, NPCManager.Instance.NPCs.Find((x) => x.GetType() == typeof(Rival)), 6001);
            email.CreateEmailButton("No", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3007);
        }

        if(completion == 3007)
        {
            email.subjectLine = "What Am I Supposed To Do Then?";
            email.mainText = "You have rejected every possible course of action that the Admin team can take. Your complaint will now be dropped. You cannot respond";
            email.CreateEmailButton("What do you mean I \"Cannot respond\"?", true);
        }

        if (Reputation.GetReputation() >= 20 && Reputation.GetReputation() < 40 && !flags.Contains("star1"))
        {
            email.subjectLine = "You have achieved Star 1";
            email.mainText = "Congratulations " + Store.StoreName + ", on achieving the first reputation star! This is a big achievment, and now you can access many " +
                "new features of the community, like the tank store.";
            email.CreateEmailButton("Thanks!", true).SetFunc(EmailFunctions.FunctionIndexes.SetFlag, name, "star1");
            important = true;
        }

        if (Reputation.GetReputation() >= 20 && Reputation.GetReputation() < 40 && !flags.Contains("star2"))
        {
            email.subjectLine = "You have achieved Star 2";
            email.mainText = "Congratulations " + Store.StoreName + ", on achieving the second reputation star! This is a big achievment, and now you can access many new features " +
                "of the community, like the vet check.";
            email.CreateEmailButton("Thanks!", true).SetFunc(EmailFunctions.FunctionIndexes.SetFlag, name, "star2");
            important = true;
        }

        if (email.mainText != null)
        {
            data.completion.Dequeue();
            email.mainText += "\n\nAdmin";

            NpcEmail(email, 0, important);
        }

        if(sent)
        {
            Email sentEmail = EmailManager.instance.emails.Find((x) => { return x.sender == name; });
            if (sentEmail != null && sentEmail.buttons == null)
            {
                if (completion == 2 && PlayerStats.stats.shrimpBought > 0)
                {
                    sentEmail.CreateEmailButton("I've bought a shrimp", true)
                        .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "OwnStoreOpen")
                        .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3);
                }
                if (completion == 3 && PlayerStats.stats.timesSellingAppOpened > 0)
                {
                    sentEmail.CreateEmailButton("I've opened the store app", true)
                        .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "UpgradeStoreOpen")
                        .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 4);
                }
                if (completion == 4 && PlayerStats.stats.timesBoughtFood > 0)
                {
                    sentEmail.CreateEmailButton("I've bought some food", true)
                        .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "VetOpen")
                        .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 5);
                }
                if(completion == 5 && PlayerStats.stats.timesVetOpened > 0)
                {
                    sentEmail.CreateEmailButton("I've opened the vet", true)
                        .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                }
            }
        }
    }
}
