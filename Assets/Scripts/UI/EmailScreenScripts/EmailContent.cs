using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EmailContent : ContentPopulation
{
    public EmailContentWindow window;

    public List<Email.EmailTags> tagsToShow = new List<Email.EmailTags>() { Email.EmailTags.Important };

    // Start is called before the first frame update
    void Start()
    {
        tagsToShow = new List<Email.EmailTags>() { Email.EmailTags.Important };
        EmailManager.instance.OnEmailSent += AddEmail;
        EmailManager.instance.OnEmailRemoved += RemoveEmail;
        foreach (Email email in EmailManager.instance.emails)
        {
            ContentBlock block = Instantiate(contentBlock, transform) .GetComponent<ContentBlock>();
            block.GetComponent<EmailContentBlock>().SetEmail(email, window);
            contentBlocks.Add(block);
            block.transform.SetAsFirstSibling();
        }
    }

    private void Update()
    {
        // Apply the filter to the emails, while maintaining order.
        foreach(EmailContentBlock block in contentBlocks) block.gameObject.SetActive(tagsToShow.Contains(block.GetEmail().tag));
    }

    public void AddEmail(Email email)
    {
        ContentBlock block = Instantiate(contentBlock, transform).GetComponent<ContentBlock>();
        block.GetComponent<EmailContentBlock>().SetEmail(email, window);
        contentBlocks.Add(block);
        block.gameObject.SetActive(tagsToShow.Contains(block.GetComponent<EmailContentBlock>().GetEmail().tag));
        block.transform.SetAsFirstSibling();
    }

    public void RemoveEmail(Email email)
    {
        ContentBlock block = contentBlocks.Find(x => x.GetComponent<EmailContentBlock>().GetEmail().ID == email.ID);
        contentBlocks.Remove(block);
        Destroy(block.gameObject);
    }

    public void ClearSpam()
    {
        List<int> indexesToRemove = new();
        for(int i = contentBlocks.Count - 1; i >= 0; i--)
        {
            if (contentBlocks[i] != null)
            {
                if (contentBlocks[i].GetComponent<EmailContentBlock>().GetEmail().tag == Email.EmailTags.Spam)
                {
                    indexesToRemove.Add(i);
                }
            }
        }
        foreach(int index in indexesToRemove)
        {
            contentBlocks[index].GetComponent<EmailContentBlock>().DeleteEmail();
        }
    }

    public void ShowOnlyImportant()
    {
        tagsToShow = new() { Email.EmailTags.Important };
    }
    
    public void ShowOnlySpam()
    {
        tagsToShow = new() { Email.EmailTags.Spam };
    }

    public void ShowAll()
    {
        tagsToShow = Enum.GetValues(typeof(Email.EmailTags)).Cast<Email.EmailTags>().ToList();
    }

    public void OnDestroy()
    {
        EmailManager.instance.OnEmailSent -= AddEmail;
        EmailManager.instance.OnEmailRemoved -= RemoveEmail;
    }
}
