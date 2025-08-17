using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableJemma : NPC
{
    public StableJemma() : base("I.Love.Horses@HorseMail.com", 40, 50)
    {
        
    }

    /// <summary>
    /// Update for the NPCs. Each NPC's check is called in order, with one 
    /// check being called per frame.
    /// </summary>
    public override void NpcCheck()
    {
        if(TimeManager.instance.day > lastDaySent && IsAwake())
        {
            Email email = this.CreateEmail();
            bool important = true;

            //Place the actual email logic here
            if(TimeManager.instance.day > 5 && completion == 0)
            {
                email.subjectLine = "Can I trust you?";
                email.mainText = "I have some shrimp which I would Like to sell, but I need them to Go to a good home. How do I know that you're Not going to abuse these poor" +
                    " Disgusting creatures?";
                email.CreateEmailButton("That seems kind of mean?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 1);
                email.CreateEmailButton("I promise you can trust me, I really love shrimp", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 2);
                email.CreateEmailButton("How can I trust you?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 1)
            {
                email.subjectLine = "I don't think so";
                email.mainText = "I don't think it's Being mean to make sure you're Not going to abuse these Helpless, horrifying Creatures! " +
                    "I don't mean it personally, I just need to Know before I can get them off My hands!";
                email.CreateEmailButton("No, I meant it seems a bit mean calling shrimp disgusting", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 3);
                important = true;
            }

            if(completion == 2)
            {
                email.subjectLine = "You seem suspicious";
                email.mainText = "I'm not sure I can Trust someone who's so Eager about shrimp. I don't think You'll take good care Of them.";
                email.CreateEmailButton("Wait, no! You can trust me with shrimp! I promise I'll take really good care of them!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 4);
                email.CreateEmailButton("Well I don't think I can trust you then.", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 3)
            {
                email.subjectLine = "Do you expect me to believe you";
                email.mainText = "I don't believe you, You know. No one would actually Think this about shrimp. That's simply not How this works. I really don't Think that " +
                    "I can trust you.";
                email.CreateEmailButton("Well how do I know I can trust you, then?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 4)
            {
                email.subjectLine = "You seem way too eager";
                email.mainText = "I don't think I can trust Someone who's so eager to Look after these horrible shrimp Properly. Why would Anyone want that?";
                email.CreateEmailButton("Actually, you don't seem very trustworthy right now!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 10);
                important = true;
            }

            if(completion == 10)
            {
                email.subjectLine = "That's a Fair point";
                email.mainText = "That is very Fair. How can you Trust me? You've never met Me. I exist, in your Perspective, exclusively in the Context of your " +
                    "Email Client. You have no Knowledge of my existence other than through a Screen, a screen Which lies to you all the Time anyway. I guess I must " +
                    "trust You, for to not Trust you would be indicate that I Cannot be trusted, and to allow you To not trust me would imply that I Cannot trust myself. " +
                    "\n\nSorry if this is getting too philisophical for you, I understand that horse raising is more intellectually profound than shrimp raising, and that " +
                    "I shouldn't expect such feats of Cerebrality from such a hobbyist. Either way, you now Have my trust.";
                email.CreateEmailButton("What on earth are you talking about?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 11);
                email.CreateEmailButton("It's nice that you trust me!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 12);
                important = true;
            }

            if(completion == 11)
            {
                email.subjectLine = "Don't worry about it";
                email.mainText = "You don't Need to worry about That.";
                email.CreateEmailButton("Oh, Ok. Well then it's nice that you trust me!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 12);
            }

            if(completion == 12)
            {
                email.subjectLine = "Well I'm happy with that!";
                email.mainText = "Yes. And as A mark of my trust, I will Allow you to purchase shrimp from me! As of recieving this email, you now have access to my Shrimp Store," +
                    " aptly named the entirely accurate \"Horses are Better\". It is not fully set up yet, and may take me some time to put my stock up, but when it is there, you may " +
                    "purchase from it!";
                ShopManager.instance.shops.Add(new Shop(3) { NpcOwned = true, NpcName = name, maxShrimpStock = 4, name = "Horses are Better" });
                email.CreateEmailButton("Thanks! I'll be sure to buy many shrimp!", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 20);
                email.CreateEmailButton("Wait what? \"Horses are Better\"? Are they?", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 13);
            }

            if(completion == 13)
            {
                email.subjectLine = "Well, of course they are";
                email.mainText = "The noble art of Horse Rearing is a venerable And ancient tradition, Which has been Maintained since the early times, At the beginning of our humble " +
                    "Society, and it is much better than simply raising shrimp.\n I Will Speak No More Of The Matter";
                email.CreateEmailButton("Oh Ok.", true)
                    .SetFunc(EmailFunctions.FunctionIndexes.SetCompletion, name, 20);
            }

            if(completion == 20 && ShopManager.instance.FindNpcShop(name).shrimpSold > 5)
            {
                email.subjectLine = "You've bought many shrimp";
                email.mainText = "I've seen you buying many shrimp From me, " + ShopManager.instance.FindNpcShop(name).shrimpSold.ToString() + " In fact! This is very Kind of you " +
                    "And it is much Appreciated that you would buy so Many of my shrimp from me! Trusting you Has definitely been a worthwhile investment! But I still have very " +
                    "many shrimp and very much shrimp Stock to sell.";
            }

            if (ShopManager.instance.FindNpcShop(name)?.shrimpSold >= 50)
            {
                email.subjectLine = "Thank You!";
                email.mainText = "You have Bought all of My Shrimp from me! I no longer have a stupid number of shrimp! I am Very happy that this has happened! " +
                    "I can finally start buying horses!";
            }

            if(email.mainText != null)
            {
                data.completion.Dequeue();
                email.mainText += "\nJemma";
                NpcEmail(email, important);
            }
            else
            {
                if (data.completion.Count > 1)
                {
                    data.completion.Enqueue(data.completion.Dequeue());
                }
            }
        }

        if(ShopManager.instance.FindNpcShop(name) != null)
        {
            Shop shop = ShopManager.instance.FindNpcShop(name);
            if(shop.maxShrimpStock > 50 - shop.shrimpSold)
            {
                shop.maxShrimpStock = 50 - shop.shrimpSold;
                while (shop.maxShrimpStock > shop.shrimpStock.Count)
                {
                    shop.shrimpStock.RemoveAt(shop.shrimpStock.Count - 1);
                }
            }
        }
    }
}
