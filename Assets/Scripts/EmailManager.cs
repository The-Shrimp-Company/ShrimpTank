using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using System.Text.Json.Serialization;
using System;

[System.Serializable]
public class MyButton
{
    [JsonIgnore]
    public List<UnityAction> actions = new();

    public List<EmailFunctions.FunctionIndexes> funcID = new();
    public DataList[] data;
    public bool destroy;
    public string text;
    public string emailID;


    public MyButton SetFunc(EmailFunctions.FunctionIndexes func, params object[] funcData)
    {
        funcID.Add(func);
        data[(int)func] = new DataList() { data = funcData.ToList() };
        return this;
    }
}

public class DataList
{
    public string name;
    public List<object> data = new();
}

[System.Serializable]
public class Email
{
    public enum EmailTags
    {
        Spam,
        Important,
        Alarms
    }

    public string ID;

    public string sender;

    public string title;
    public string subjectLine;
    public string mainText;

    public int value;

    public float timeSent;

    public EmailTags tag;

    public List<MyButton> buttons;

    public void GiveMoney()
    {
        Money.instance.AddMoney(value);
    }

}

public class Notification
{
    public string text;
    public string ID;
}


public delegate void EmailEvent(Email email);

public class EmailManager
{
    static public EmailManager instance = new EmailManager();


    public event EmailEvent OnEmailSent;
    public event EmailEvent OnEmailRemoved;

    public int currentNotif = 0;
    public List<Notification> notifications = new ();
    public List<Notification> alarmNotifs = new ();

    private int IdChaff = 0;

    public FullEmail openEmail;

    public List<Email> emails { get; private set; } = new List<Email>();

    public EmailManager()
    {
        if(instance == null)
        {
            instance = this;
        }

        instance.Initialize();
    }

    public void Initialize()
    {
        if(SaveManager.CurrentSaveData.emails != null)
        {
            emails = SaveManager.CurrentSaveData.emails.ToList();
        }
        else
        {
            emails = new();
        }
    }

    public string CreateID()
    {
        return ((int)(DateTime.UtcNow.Ticks%100000)).ToString() + (instance.IdChaff++).ToString();
    }

    static public void SendEmail(Email email, bool important = false, float delay = 0)
    {
        CustomerManager.Instance.StartCoroutine(SendEmailDelayed(email, important, delay));
    }

    static IEnumerator SendEmailDelayed(Email email, bool important, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        SendEmailDirect(email, important);
    }

    static public void SendEmailDirect(Email email, bool important)
    {
        if (important) email.tag = Email.EmailTags.Important;
        instance.emails.Add(email);

        UIManager.instance.RefreshNotif();
        if (email.tag != Email.EmailTags.Spam) UIManager.instance.triggerSound = true;
        email.timeSent = TimeManager.instance.GetTotalTime();
        if (instance.OnEmailSent != null)
        {
            instance.OnEmailSent(email);
        }
        if(email.tag == Email.EmailTags.Alarms)
        {
            instance.alarmNotifs.Add(new Notification() { text = email.subjectLine, ID = email.ID });
            instance.currentNotif = instance.alarmNotifs.Count - 1;
            UIManager.instance.RefreshNotif();
        }
        else if(email.tag == Email.EmailTags.Important)
        {
            instance.notifications.Add(new Notification() { text = email.subjectLine, ID = email.ID });
            instance.currentNotif = instance.notifications.Count - 1;
            UIManager.instance.RefreshNotif();
        }
        
    }

    public string GetNotification()
    {
        string notif = "";

        
        if (alarmNotifs.Count != 0)
        {
            if (currentNotif >= alarmNotifs.Count) currentNotif = 0;
            notif += "<b><color=red>! </color></b>";
            notif += alarmNotifs[currentNotif].text;
        }
        else if(notifications.Count != 0)
        {
            if (currentNotif >= notifications.Count) currentNotif = 0;
            notif += notifications[currentNotif].text;
        }
        currentNotif++;

        return notif;
    }

    static public void RemoveEmail(Email email)
    {

        NPCManager.Instance.GetNPCFromName(email.sender)?.EmailDestroyed();

        if (instance.OnEmailRemoved != null)
        {
            instance.OnEmailRemoved(email);
        }

        if(instance.notifications.Find(x => x.ID == email.ID) != null)
        {
            instance.notifications.Remove(instance.notifications.Find(x => x.ID == email.ID));
            UIManager.instance.RefreshNotif();
        }

        if (instance.openEmail != null && instance.openEmail.ID == email.ID)
        {
            if(!instance.openEmail.gameObject.IsDestroyed() && instance.openEmail.gameObject != null)
            {
                GameObject.Destroy(instance.openEmail.gameObject);
            }
        }
        instance.emails.Remove(email);
    }
}

public static class EmailTools
{
    /// <summary>
    /// A tool to create buttons to appear in the email screen
    /// </summary>
    /// <param name="email">The email to add the button to, this uses the email ID instead of a reference</param>
    /// <param name="text">What the button should say</param>
    /// <param name="destroy">If true, the button will also have a listener added to delete the email when the button is pressed</param>
    static public MyButton CreateEmailButton( this Email email, string text, bool destroy = false)
    {
        email.buttons = email.buttons ?? new List<MyButton>();
        MyButton button = new()
        {
            text = text,
            destroy = destroy,
            data = new DataList[(int)EmailFunctions.FunctionIndexes.Count],
            emailID = email.ID
        };
        email.buttons.Add(button);
        return button;
    }

    static public void AddEmailText(this Email email, string textToAdd, string signiture = "")
    {
        email.mainText = email.mainText.Insert(email.mainText.Length - signiture.Length, textToAdd);
        UIManager.instance.PushNotification(email.subjectLine, true);
    }

    static public Email CreateEmail()
    {
        Email email = new() { ID = EmailManager.instance.CreateID() };
        return email;
    }

    static public Email CreateEmail(this NPC npc)
    {
        Email email = CreateEmail();
        email.sender = npc.name;
        return email;
    }
}
