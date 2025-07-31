using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EmailContent : ContentPopulation
{
    public EmailContentWindow window;

    public List<Email.EmailTags> tagsToShow = new() { Email.EmailTags.Important };

    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Email email in EmailManager.instance.emails)
        {
            ContentBlock block = Instantiate(contentBlock, transform) .GetComponent<ContentBlock>();
            block.GetComponent<EmailContentBlock>().SetEmail(email, window);
            contentBlocks.Add(block);
        }
        count = contentBlocks.Count;
    }

    private void Update()
    {
        
        if(count != EmailManager.instance.emails.Count)
        {
            /* Old way of doing this
            foreach(ContentBlock block in contentBlocks)
            {
                Destroy(block.gameObject);
            }
            contentBlocks.Clear();
            foreach (Email email in EmailManager.instance.emails)
            {
                ContentBlock block = Instantiate(contentBlock, transform).GetComponent<ContentBlock>();
                block.GetComponent<EmailContentBlock>().SetEmail(email, window);
                contentBlocks.Add(block);
            }
            count = contentBlocks.Count;
            */
        

            bool flag;

            foreach(EmailContentBlock block in contentBlocks)
            {
                flag = false;
                foreach(Email email in EmailManager.instance.emails)
                {
                    if(block.GetEmail().ID == email.ID && tagsToShow.Contains(email.tag))
                    {
                        flag = true;
                    }
                }
                if (!flag && block != null)
                {
                    Destroy(block.gameObject);
                }
            }

            foreach(Email email in EmailManager.instance.emails)
            {
                flag = false;
                foreach(EmailContentBlock block in contentBlocks)
                {
                    if(block != null)
                    {
                        if(block.GetEmail().ID == email.ID)
                        {
                            flag = true;
                        }
                    }
                }
                if (!flag && tagsToShow.Contains(email.tag))
                {
                    ContentBlock block = Instantiate(contentBlock, transform).GetComponent<ContentBlock>();
                    block.GetComponent<EmailContentBlock>().SetEmail(email, window);
                    contentBlocks.Add(block);
                }
            }
            for(int i = contentBlocks.Count-1; i >= 0; i--)
            {
                if (contentBlocks[i] == null)
                {
                    contentBlocks.RemoveAt(i);
                }
            }

            count = contentBlocks.Count;
        }
    }

    public void ClearSpam()
    {
        foreach(ContentBlock block in contentBlocks)
        {
            if (block != null)
            {
                if (block.GetComponent<EmailContentBlock>().GetEmail().tag == Email.EmailTags.Spam)
                {
                    block.GetComponent<EmailContentBlock>().DeleteEmail();
                }
            }
        }
    }

    public void ShowOnlyImportant()
    {
        tagsToShow = new() { Email.EmailTags.Important };
        count++;
    }
    
    public void ShowOnlySpam()
    {
        tagsToShow = new() { Email.EmailTags.Spam };
        count++;
    }

    public void ShowAll()
    {
        tagsToShow = Enum.GetValues(typeof(Email.EmailTags)).Cast<Email.EmailTags>().ToList();
        count++;
    }
}
