using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rival : NPC
{
    public Rival()
    {
        reputation = 0;
        reliability = 0;
        completion = 0;
        name = "John@YourRivalMail.com";
    }



    /// <summary>
    /// Update for the NPCs. Each NPC's check is called in order, with one 
    /// check being called per frame.
    /// </summary>
    public override void NpcCheck()
    {
        if(!sent && TimeManager.instance.day > LastDaySent)
        {
            Email email = this.CreateEmail();
            bool important = true;

            // The first email from the rival. Requires reputation as a timer.
            if(Reputation.GetReputation() > 5 && completion == 0)
            {
                email.subjectLine = "Really?";
                email.mainText = "So.\nYou actually did it.\nYou actually started the store.\nI can't believe that.\nYou thought that by calling it that <i>stupid</i>" +
                    " name you would get away with it?" +
                    "\nYeah that's right. I said it.\nI think the name is dumb, ok? Look lets not beat around the bush, ok? Just give up. It's not worth it. You'll never complete a" +
                    " request anyway. Not in <i>this</i> community.";
                email.CreateEmailButton("Who on earth are you?", () => 
                {
                    completion = 1;
                }, true);
                email.CreateEmailButton("Ahh, my old nemesis... So you found me already?", () =>
                {
                    completion = 10;
                }, true);
            }

            // If you don't recognise the rival
            if(completion == 1)
            {
                email.subjectLine = "Don't play dumb with me";
                email.mainText = "You know who I am, don't try to pretend. For one thing, it won't work. For another, it makes you look like a fool. For a " +
                    "third, did I mention that it <i>won't work</i>. \nBut seriously, do you not read signitures on emails.\nI'm your rival? You know, I'm the one who" +
                    " told you you'd never make it in the shrimping world, and to just give up, and now you're here, I'm going to prove my own point, and " +
                    "try to show you that you'll never be as good at keeping and selling shrimp as me? Do you really not remember?\n...\n...\nOk, look I don't know if you're" +
                    "lying to me or not, so I'm gonna assume you are, OK?";
                email.CreateEmailButton("Look man, sure. I Guess", () => { completion = 10; }, true);
                email.CreateEmailButton("OH! Wait! You're my nemesis! I can't believe I forgot! How're you doing! It's been a while!", () => { completion = 9; }, true);
            }

            // If you remember who the rival is
            if(completion == 9)
            {
                email.subjectLine = "Great!";
                email.mainText = "I'm doing great! I'm glad you remembered, I was getting worried for a second there. Either way, I'm looking forward to rivalling with you! Or is it rivalrying?" +
                    "Ahh, I don't care. It doesn't matter. Point is, is you've opened your store, so our rivalry can begin for <i>real</i>. Good luck!";
                email.CreateEmailButton("Yeah, Good luck to you too!", () => { completion = 10; }, true);
            }

            // Once the rivalry is beginning
            if(completion == 10)
            {
                email.subjectLine = "Rivalry Begun";
                email.mainText = "Ok, as your rival, I'm going to do a better job of shrimping than you. Now, in order to ensure this is a fair fight, I have sold off my old" +
                    " shrimp store, and I've given all the money away. This means that right now I have the same amount of stuff, and reputation, as you.\n\n" +
                    "However! I will not be at the same pitiful level as you for long! In fact, I will achieve the first star of reputation before you! And when I do get the first star," +
                    " I will gloat for <i>so long</i> that you'll probably have enough time to catch up to me! Because I'm a <i>really</i> good rival! Ahahahahahahahahahahahaha!";
                email.CreateEmailButton("Yes! A real challenge! I will achieve this \"first star of reputation\" long before you get your hands on it!", () => { completion = 20; }, true);
                email.CreateEmailButton("What are reputation stars? Also, did you really sell of <i>all</i> of your stuff for this rivalry?", () => { completion = 11; }, true);
            }

            // Explaining what a "reputation star" is
            if(completion == 11)
            {
                email.subjectLine = "Reputation stars";
                email.mainText = "Wait, you <i>don't</i> know what reputation stars are? Didn't the admin explain already? Look, you can see your reputation when you go to your store page." +
                    " You <i>do</i> know how to go to your store page right? It's really simple. It's the light blue shrimp app, the one where it's facing left. And reputation is shown in stars, " +
                    "and at each star you can access new services in the community. I told you it's a weird place to be. Getting the first star is a big deal, because it allows access to buying " +
                    "more tanks, which is, I'm sure you can see, <i>very</i> useful.\n\n\nAlso: yes, of course I did. This had to be fair, after all.";
                email.CreateEmailButton("Oh thanks, that's good to know.", () => { completion = 20; }, true);
                email.CreateEmailButton("Actually, if you're answering my questions, the admin mentioned I can get" +
                    " support, but they didn't say where, do you know?", () => { completion = 12; }, true);
            }

            // If the player asks about admin support
            if(completion == 12)
            {
                email.subjectLine = "Admin support?";
                email.mainText = "Oh, the admin says that to everyone. There used to be a helpline, but it got shut down, and the automated emails still reference it, but because the" +
                    " helpline is gone it can't actually direct you anywhere, hence just telling you you can \"find support\". It's kinda sad really.";
                email.CreateEmailButton("Oh, Ok.", () => { completion = 20; }, true);
            }

            // When the player has the reputation, and completion is at 20
            if(Reputation.GetReputation() >= 20 && completion == 20)
            {
                email.subjectLine = "Well Done!";
                email.mainText = "I'm impressed, rival! I was not expecting you to manage to achieve the first star before me, but I am the best" +
                    " rival in the world, and so accepting this <i>initial</i> loss is easy for me. You've done a good job managing to best me to the first corner, but can you maintain your " +
                    "speed on the straight, and keep it further through the loop de loop?\nFine. I don't understand racing metaphors. But that's not the point. The point is, you might " +
                    "have beaten me to the first star, but only by an hour. In fact, I've achieved mine just now. And so the real race will be to see who can reach the second star first! " +
                    "Ahahahahahahahah";
                email.CreateEmailButton("Wait I won! Yay!", () => { completion = 21; }, true);
                email.CreateEmailButton("Do I get something from winning? Is there some kind of prize?", () => { completion = 22; }, true);
            }

            // When the player has the reputation, and completion is below 20, but above 10, meaning the 
            // player has gotten the first star while talking to the rival about getting the first star
            if(Reputation.GetReputation() >= 20 && completion < 20 && completion >= 10)
            {
                email.buttons.Clear();
                email.subjectLine = "You Sly Dog";
                email.mainText = "How dare you distract me with talk while you go ahead and win the challenge! This isn't sportsmanlike at all!\nAlthough...\n I guess that would be " +
                    "very rivallike...\nSo I guess I must say well done! You've got one over on me, but by tomorrow, I'll be at my first star too!";
                email.CreateEmailButton("Sorry, I didn't mean to cheat!", () => { }, true);
                email.CreateEmailButton("Yes! I win! I tricked you, and I did it good!", () => { }, true);
                email.CreateEmailButton("I distracted you? Wait so we hadn't started yet?", () => { }, true);
            }

            if(Reputation.GetReputation() >= 20 && completion < 10)
            {
                email.buttons.Clear();
                email.subjectLine = "You've been ignoring me";
                email.mainText = "How <i>could</i> you? You've been ignoring me completely, and you've beaten my first competition before I could even properly challenge you. " +
                    "This is simply too cruel. I just want to have a proper rivallry with you, ok? Give me a couple of days to get my first reputation star as well, and then we can " +
                    "continue on again, competing fairly. Just, don't get another star until I've got my first ok?";
                email.CreateEmailButton("Wait we were competing? Oh I'm so sorry, I didn't realise! I'll definitely wait", () => { }, true);
                email.CreateEmailButton("You Fool. You fell this far behind, and now you're begging to catch up? I will wait, but only so I can more thouroughly destroy you!!!", () => { }, true);
                email.CreateEmailButton("Why would I wait for someone who's fallen so far behind? *scoffs* Obviously, I'm just going to go ahead and get that next star before you can" +
                    " even get your first.", () => { }, true);
            }

            // Actually sending the email
            if (email.mainText != null) {
                email.mainText += "\n\nYour hateful rival";
                NpcEmail(email, important);
            }
        }
    }
}
