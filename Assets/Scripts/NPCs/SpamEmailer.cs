using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpamEmailer : MonoBehaviour
{
    private int lastHour = 0;

    void Start()
    {
        lastHour = (int)TimeManager.instance.hour;
    }

    private void Update()
    {
        if(!Tutorial.instance.flags.Contains("AccountActivated"))
        {
            return;
        }
        if (!Tutorial.instance.flags.Contains("SentSpamApology"))
        {
            Tutorial.instance.flags.Add("SentSpamApology");
            Email email = EmailTools.CreateEmail();
            email.title = "BigSpam@SpamMail.spm";
            email.subjectLine = "We Are Sorry, And We Are Learning";
            email.mainText = "It has come to our attention that we have sinned. As you probably know, we are the worlds largest spam company, and up until now, " +
                "we thought we were doing a good job providing spam for the world. We slept soundly in our beds, knowing that we were ensuring that the world could " +
                "safely know that they would be laden down with a healthy dose of spam, at every turn. But recently, you have pointed out to us that we were doing " +
                "something wrong. We had failed, and for that we are sorry.\n\nBecause you were correct.\n\nWe had failed.\n\nWe had been focusing on creating the most repetitive " +
                "and consistent bespoke spam calls, just for you. We were the world leaders in giving you the sinking feeling of getting a phone call, and knowing without a " +
                "doubt that it would take at least 20 minutes to figure out if we were a legitimate company calling about something important, or if we were just a spam call. " +
                "And we like to believe, that despite what has come to light, that we succeeded at that. But, as your feedback has pointed out, this isn't where our focuses " +
                "should lie.\n\nAs you correctly pointed out, the world right now is a difficult place, with so much going on all the time. There are so many distractions in life " +
                "and we as a society need to be able to handle these distractions better.And, as a result of this, people tend to not answer their phone as much these " +
                "days. Our spam calls simply don't get through as often, and so we can't do our solemn <i>duty</i> and provide those modern distraction to " +
                "all of you. And so, based on user feedback, we have decided to pivot our focus away from phone calls, to instead focus exclusively on spam emails. " +
                "For this purpose we have burned down all of our phone line centres, after throwing all of the phones into the sea. We understand that this massive loss " +
                "of life and potential damage to the environment is simply inhumane, and we appreciate that this isn't required at all, but we like to go above and " +
                "beyond in making this planet a worse place to be, day after day. We hope that this almost ritualistic killing of thousands of employees and presumably turtles " +
                "appeases the anger that you, our user, feels after we failed to fulfil your needs for so long. In the coming days, as we begin the process of buying up " +
                "the best and most friendly ophanages we can to convert into tower blocks full of less than minimum wage workers manualy typing out spam emails which are all " +
                "going to be, for the next decade or until we decide to stop, weirdly shrimp focused, and then sending them off to random people and also you, you should begin " +
                "getting more spam emails than you ever have before! It's a new stage in our companies history, and also your misery! We understand that you might have ideas " +
                "for making this even more miserable, but due to the sheer cost of making this change, as well as the fact that sending spam anything at this scale is simply " +
                "not that profitable, and we mostly fund this from our massive amount of real estate we own and the fact that we own the patent for filtered water and " +
                "as such have as much money as we want but don't really get any money from this, we will be refusing to acknowledge any complaints that our users may have " +
                "about our services until we decide to again.\n\n<size=40>CEO of BigSpam (although we made one of our workers type this out),\nArthur Williby Henrick</size>";
            email.CreateEmailButton("Genuinely, what?", true);
            EmailManager.SendEmail(email, true, delay:2);
            lastHour = (int)TimeManager.instance.hour;
        }

        if(TimeManager.instance.hour > lastHour && Tutorial.instance.flags.Contains("AccountActivated"))
        {
            if(Random.value > 0.5)
            {
                Email email = EmailTools.CreateEmail();
                switch(Random.Range(0, 10))
                {
                    case 0:
                        email.title = "MagicMedicine@Doctor.real";
                        email.subjectLine = "Would you like to blackout for about an hour?";
                        email.mainText = "Would you like to blackout for about an hour? With our revolutionary new technology (a baseball bat sent flying at your head from " +
                            "up to twenty miles away at mach 4) we can let you experience being completly unconcious for about an hour, with incredibly limited side effects! " +
                            "Best of all, the entire experience is completly free!" +
                            "<size=5>Side effects may include: Long term concussion, short term amnesia, long term amnesia, blood loss, medical bills, extreme and sudden pain, " +
                            "this program shutting down, and death</size>";
                        email.CreateEmailButton("Sign Me Up!", true).SetFunc(EmailFunctions.FunctionIndexes.SpamEmailBlackout);
                        break;
                    case 1:
                        email.title = "GenericEmailAddress@GenericMail.com";
                        email.subjectLine = "Insert Viable Reason To Be Contacting This Person Here";
                        email.mainText = "Make Up Reason To Be Talking To This Person, Connect It To The Subject Line, And Then Make The Reason Something To Do With Them " +
                            "Giving Me Access To Their Bank Account\n\nInsert Realistic Name Here";
                        break;
                    case 2:
                        email.title = "OnlineRetailer@ShopMail.com";
                        email.subjectLine = "You bought something!";
                        email.mainText = "Yesterday we noticed that you came to our site, and you actually bought something! This is very rare in our experience trying to sell " +
                            "you things, and it's given us just a little bit of hope that you might actually buy something else now! So considering that you bought from us the " +
                            "[Marigold Yellow Front Door With Four Glass Panels And Smooth Handle] we have some recommendations for other things that we would like you to buy! " +
                            "\n\nWould you like to buy [Small Pieces Of Lego]? 12% of customer who bought [Marigold Yellow Front Door With Four Glass Panels And Smooth Handle] " +
                            "also bought [Small Pieces Of Lego], so we know that you would like [Small Pieces Of Lego]. Or how about one of our newest products [Computer Mouse " +
                            "With Fun And Painful Joybuzzer Built Into Body, Just Click A Button And Experience Fun And Also Pain]? We need to note that only 0.43% of customers " +
                            "who bought [Marigold Yellow Front Door With Four Glass Panels And Smooth Handle] even looked at the product page of [Computer Mouse " +
                            "With Fun And Painful Joybuzzer Built Into Body, Just Click A Button And Experience Fun And Also Pain], but we have all this stock to get rid of, and " +
                            "we think a discerning customer like you might want to buck that trend!\n\nPlease Buy!";
                        break;
                    default:
                        email.title = "NotSpam@SpamMail.spm";
                        email.subjectLine = "Would you like less spam?";
                        email.mainText = "Have you considered the fact that spam is a vital part of many peoples lives, and frankly it's bordering on actually immoral that you " +
                            "would dare suggest that spam isn't an entirely acceptable part of life, and I hope that in future you will take the time to make sure that " +
                            "you're properly apprecitating the amount that the spam creators and distributers of the world are doing for you, and those around you." +
                            "\n I hope that this teaches you to properly consider the impact that spam has. I also hope that with your new found knowledge about what " +
                            "spam is and how it works, you'll learn to properly engage with spam. When you get a piece of spam, remember that it has been lovingly crafted by " +
                            "an underpaid, underapprecitated intern, or potentially even by some kind of algorithm. Don't let the algorithm down! Pay attention to what the spam " +
                            "is informing you about, and engage with it. Did you know that only 0.143% of recipients of this message have actually engaged with the product being sold? " +
                            "That's simply not enough! Especially considering I know that you <Insert Reader Name Here> understand and appreciate the hard work that goes into making " +
                            "this bespoke and custom spam, just for you! We know you appreciate us properly, so knowing that you still haven't engaged with our hard work, and the product " +
                            "we are advertising to you in spite of your appreciation of us makes us question if you even really care. And that makes me very sad indeed. I thought we could " +
                            "have had something. I thought this could have been special. I thought you could have pulled my engagement percentage up to 0.1431% <Insert Reader Name Here>! " +
                            "But no. You've let me down <Insert Reader Name Here>. <Insert Crying Cat Gif Here>. That's right. That's how I feel. Because you don't care. Don't worry though. " +
                            "You can fix this! All you have to do is engage with our product.";
                        break;
                }
                EmailManager.SendEmail(email, delay:Random.value*20);
            }
            lastHour = (int)TimeManager.instance.hour;
        }
    }
}
