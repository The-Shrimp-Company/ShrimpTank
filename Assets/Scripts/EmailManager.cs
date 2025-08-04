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
    public int emailID;


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
        Important
    }

    public int ID;

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

public class EmailManager
{
    static public EmailManager instance = new EmailManager();

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

    public int CreateID()
    {
        return (int)DateTime.UtcNow.Ticks + instance.IdChaff++;
    }

    static public void SendEmail(Email email, bool important = false, float delay = 0, NPC sender = null)
    {
        CustomerManager.Instance.StartCoroutine(SendEmailDelayed(email, important, delay));
    }

    static IEnumerator SendEmailDelayed(Email email, bool important = false, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        if (important) email.tag = Email.EmailTags.Important;
        instance.emails.Add(email);
        email.timeSent = TimeManager.instance.GetTotalTime();
        UIManager.instance.SendNotification(email.subjectLine);
    }

    static public void RemoveEmail(Email email)
    {

        NPCManager.Instance.GetNPCFromName(email.sender)?.EmailDestroyed();

        if(instance.openEmail != null)
        {
            GameObject.Destroy(instance.openEmail.gameObject);
        }
        instance.emails.Remove(email);
    }
}

public static class EmailTools
{
    /// <summary>
    /// A tool to create buttons to appear in the email screen
    /// </summary>
    /// <param name="emailID">The ID of the email to add the button to</param>
    /// <param name="text">What the button should say</param>
    /// <param name="data">The information about the function to run on click </param>
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
