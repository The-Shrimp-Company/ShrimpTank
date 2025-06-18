using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public struct MyButton
{
    public UnityAction action;
    public bool destroy;
    public string text;
}


public struct Email
{
    public int ID;

    public string title;
    public string subjectLine;
    public string mainText;

    public int value;

    public bool important;

    public List<MyButton> buttons;

    public void GiveMoney()
    {
        Money.instance.AddMoney(value);
    }

    public override bool Equals(object obj)
    {
        if (obj is Email m)
        {
            return m.ID == ID;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public class EmailManager
{
    static public EmailManager instance = new EmailManager();

    private int IdChaff = 0;

    public List<Email> emails { get; private set; } = new List<Email>();

    public EmailManager()
    {
        Initialize();
    }

    public void Initialize()
    {
        emails = new List<Email>();
    }

    static public void SendEmail(Email email, bool important = false, int delay = 0)
    {
        CustomerManager.Instance.StartCoroutine(SendEmailDelayed(email, important, delay));
    }

    static IEnumerator SendEmailDelayed(Email email, bool important = false, int delay = 0)
    {
        yield return new WaitForSeconds(delay);
        email.important = important;
        email.ID = (int)Time.time + instance.IdChaff++;
        instance.emails.Add(email);
        UIManager.instance.SendNotification(email.title);
    }
}

public static class EmailTools
{
    /// <summary>
    /// A tool to create buttons to appear in the email screen
    /// </summary>
    /// <param name="email">The email to add the button to</param>
    /// <param name="text">What the button should say</param>
    /// <param name="action">The function to run on click</param>
    /// <param name="destroy">If true, the button will also have a listener added to delete the email when the button is pressed</param>
    static public void CreateEmailButton(ref this Email email, string text, UnityAction action, bool destroy = false)
    {
        if(email.buttons == null)
        {
            email.buttons = new List<MyButton>();
        }
        MyButton button = new MyButton();
        button.text = text;
        button.action = action;
        button.destroy = destroy;
        email.buttons.Add(button);
        //Debug.Log("Added button");
    }
}
