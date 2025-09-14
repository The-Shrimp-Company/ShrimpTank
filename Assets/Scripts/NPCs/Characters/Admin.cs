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
            email.CreateEmailButton("Walk me through the systems", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 2);
            email.CreateEmailButton("Give me everything (Not recommended for new players)", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10)
                .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "ShrimpStoreOpen", "OwnStoreOpen", "UpgradeStoreOpen", "VetOpen");
            important = true;
        }

        #region Tutorial
        if(completion == 2)
        {
            email.subjectLine = "Setting up tanks";
            email.mainText = "It seems you already have some tanks in your store, but they're not quite set up yet. The first thing you'll need to do is to add water";
        }

        if(completion == 3)
        {
            email.subjectLine = "Buying shrimp";
            email.mainText = "Now that you have a tank set up, you'll want to buy some shrimp. There are several things to be aware of when doing this, the first being that not every shrimp is " +
                "suited for the tank you've set up, and the second being that if you want to get any more shrimp out of the purchase, you'll want to get at least two, of different sexes. So, go into " +
                "the store, and look for two shrimp, male and female, and both with similar needs.\nThen, make sure your tank is set up for them, so try and make sure the numbers are the " +
                "same as what they need, and then buy them both. It's important to remember, they <color=yellow>don't</color> need to be perfect for each other, the shrimp won't die as long as they " +
                "have at least <color=yellow>one</color> of their needs met";
        }

        if(completion == 4)
        {
            email.subjectLine = "Caring for shrimp";
            email.mainText = "Well, caring for shrimp isn't too hard. You need to make sure that the shrimp have something to eat, and you need to make sure that you keep their needs met. " +
                "The more of their needs you meet, the better, but you can get away with only meeting <color=yellow>one</color> of their needs at a time. If you don't meet any of their needs, " +
                "they will die pretty fast. You also have to feed them <color=yellow>at least once a day</color>, but as long as you've put the food in the tank every day, they should be able " +
                "to feed themselves as much as they need. To start with, go into the store on your tablet, and <color=yellow>buy some shrimp food</color>.";
        }

        if(completion == 5)
        {
            email.subjectLine = "A few more things";
            email.mainText = "Well, now that you have the caring for shrimp, and buying shrimp parts down, the rest of the process is quite simple. Firstly, if you have a healthy tank of shrimp, " +
                "eventually they will breed and you will get more shrimp. This is good for obvious reasons, but it's important to note that <color=yellow>if you have too many shrimp in the same tank, they " +
                "will stop breeding</color>. Next, you may be wondering how to make any money. Well, once you've bred some shrimp, all you need to do is to sell them on. Selling shrimp is done most " +
                "commonly in one of two ways: either <color=yellow>making the tank open to customers</color> or by instead <color=yellow>listing specific shrimp in your store</color>. These both have " +
                "their up and downsides. As a new store, you can only list so many shrimp at a time in your store, so you can't sell too many shrimp over there, and you have to set the price yourself, so " +
                "you might end up setting it too low, and giving someone a really good deal, or you might set it too high and lose reputation. The other option lets you open a tank to customers, which does " +
                "sell shrimp faster, but you can't set the price, and generally the shrimp will sell for a little less than what you could have gotten selling them on your store. You'll have to decide for " +
                "yourself which is best.\n\nAlso, another thing to keep in mind is that the vet app will both provide medical services for you and you shrimp while also <color=yellow>having some useful " +
                "information on shrimp care</color> which you can access for free. Check it out if you're feeling lost.";
            email.CreateEmailButton("I understand", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
        }

        if (completion == 10)
        {
            email.subjectLine = "Account Fully Activated";
            email.mainText = "Your account is now fully activated, and you will be able to recieve emails from other shrimp store owners, and members of the community. Please be civil, and if you need " +
                "assistance, please find support.";
            email.CreateEmailButton("Find support where?", true)
                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 11)
                .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "AccountActivated");
            important = true;
        }
        #endregion

        #region Other NPC Interactions
        if (completion == 3000)
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
        #endregion

        #region Reputation Points
        if (Reputation.GetReputation() >= 20 && !flags.Contains("star1"))
        {
            email.subjectLine = "You have achieved Star 1";
            email.mainText = "Congratulations " + Store.StoreName + ", on achieving the first reputation star! This is a big achievment, and now you can access many " +
                "new features of the community, like the tank store.";
            flags.Add("star1");
            email.CreateEmailButton("Thanks!", true);
            important = true;
        }

        if (Reputation.GetReputation() >= 40 && !flags.Contains("star2"))
        {
            email.subjectLine = "You have achieved Star 2";
            email.mainText = "Congratulations " + Store.StoreName + ", on achieving the second reputation star! This is a big achievment, and now you can access many new features " +
                "of the community, like the vet check.";
            flags.Add("star2");
            email.CreateEmailButton("Thanks!", true);
            important = true;
        }

        if(Reputation.GetReputation() >= 50 && !Tutorial.instance.flags.Contains("extraShop"))
        {
            email.subjectLine = "Project: Extra Shop";
            email.mainText = "We are pleased to announce a new update rolling out! From now on, select Shrimpers will be able to sell extra shrimp paraphenalia directly through " +
                "their shrimp store page! We hope this revolutionary idea of buying more things through our store fills you all with as much indescribable glee as it does us! " +
                "\n<size=10>Terms and conditions may apply, as we write them. We is used to refer to the group of people profiting from this endeavour, and we are " +
                "not legally liable for any of the products sold on the pages we put up. We are not condoning selling products, and we will never condone selling " +
                "products, no matter what you might tell people, Deborah. You are the group of people who are not profiting from this in any sense other than the spiritual " +
                "which we maintain is both completely intangible, and so any effect can be neither proven nor disproven, and also vehemently deny the existence of in the " +
                "case of one of you claiming that we have harmed you spiritually. You can get no warranty for the services we offer, as we refuse to offer any warrantable " +
                "services. If you think we do, we will sue you. Happy shrimping is a registered trademark. Also, happy shrimping.</size>";
            email.CreateEmailButton("That sounds cool!", true);
            Tutorial.instance.flags.Add("extraShop");
            important = true;
        }
        #endregion

        if (email.mainText != null)
        {
            if(data.completion.Count > 0)
            {
                data.completion.Dequeue();
            }
            email.mainText += "\n\nAdmin";

            NpcEmail(email, 0, important);
        }

        #region Tutorial Responsiveness
        if (sent)
        {
            Email sentEmail = EmailManager.instance.emails.Find((x) => { return x.sender == name; });
            if(sentEmail != null)
            {
                if (sentEmail.subjectLine == "Setting up tanks")
                {
                    if (!flags.Contains("TankFilled"))
                    {
                        if (Store.decorateController.tanksInStore.Exists(x => x.waterFilled))
                        {
                            sentEmail.AddEmailText("\nNow that you've filled the tank with water, you need to add some salt to the water. You'll need enough salt for your shrimp. " +
                                "Most shrimp like around 50 units of salt in their water, so if you add that much, it should be fine. <color=yellow>Salt will reduce with time</color> " +
                                "so don't worry too much, but it's easier to add more than it is to take it away. For now, if you get it wrong, you can empty the tank and refil it to " +
                                "correct the problem.", "\n\nAdmin");
                            flags.Add("TankFilled");
                        }
                    }
                    else if (!flags.Contains("AddedSalt"))
                    {
                        if (Store.decorateController.tanksInStore.Exists(x => x.waterFilled && x.waterSalt > 40))
                        {
                            sentEmail.AddEmailText("\nNow that the tank has a suitable amount of salt, you need to do the same thing for nitrate. Add around 50 nitrate to the tank, and if you " +
                                "add too much, just empty the tank and start again. If you do need to reset the tank, remember to add more salt as well. <color=yellow>Nitrate will also reduce " +
                                "with time</color>so you may have to top it up every once in a while.", "\n\nAdmin");
                            flags.Add("AddedSalt");
                        }
                    }
                    else if (!flags.Contains("AddedNitrate"))
                    {
                        if (Store.decorateController.tanksInStore.Exists(x => x.waterFilled && x.waterAmmonium > 40))
                        {
                            sentEmail.AddEmailText("\nIt seems you have got a tank to the right proportions. I do need to still explain pH. You tank has water in it, and that water is at a specific " +
                                "pH level. It will start at 7, and your shrimp will all have different pH preferences. pH is the easiest quality of your tank to change, and is rarely changed by time, " +
                                "so while you are beginning it is a good idea to <color=yellow>group shrimp by their pH preference</color>, but you can do what you want.", "\n\nAdmin");
                            sentEmail.CreateEmailButton("I understand", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3)
                                .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "ShrimpStoreOpen");
                            flags.Add("AddedNitrate");
                        }
                    }
                }
                else if (sentEmail.subjectLine == "Buying shrimp")
                {
                    if (!flags.Contains("BoughtShrimp"))
                    {
                        if(Store.decorateController.tanksInStore.Exists(x => x.shrimpInTank.Exists(x => x.stats.sex) && x.shrimpInTank.Exists(x => !x.stats.sex)))
                        {
                            flags.Add("BoughtShrimp");
                            sentEmail.CreateEmailButton("I've bought the shrimp", true)
                                .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 4)
                                .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "UpgradeStoreOpen");
                        }
                    }
                }
                else if(sentEmail.subjectLine == "Caring for shrimp")
                {
                    if (!flags.Contains("BoughtFood"))
                    {
                        if(Inventory.GetInventoryItemsWithTag(ItemTags.Food).Count > 0)
                        {
                            flags.Add("BoughtFood");
                            sentEmail.AddEmailText("\nNow that you have bought food, you must feed your shrimp. Go to the storage boxes, by your stores door, and take the shrimp food out of it. Then go " +
                                "and put some in the tank. The sign telling you to feed your shrimp will disapear when you have fed them.", "\n\nAdmin");
                        }
                    }
                    else if (!flags.Contains("FedShrimp"))
                    {
                        if(Store.decorateController.tanksInStore.Exists(x => x.shrimpInTank.Count > 0 && x.FedShrimpToday()))
                        {
                            flags.Add("FedShrimp");
                            sentEmail.AddEmailText("So now you have set up a tank for shrimp, and you've bought shrimp, and you've fed shrimp. Well done!", "\n\nAdmin");
                            sentEmail.CreateEmailButton("I understand", true).SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, 5)
                                .SetFunc(EmailFunctions.FunctionIndexes.SetTutorialFlag, "OwnStoreOpen", "VetOpen");
                        }
                    }
                }
            }
        }
        #endregion
    }
}
