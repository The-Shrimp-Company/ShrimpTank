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
                EmailManager.SendEmail(email);
            }
            lastHour = (int)TimeManager.instance.hour;
        }
    }
}
