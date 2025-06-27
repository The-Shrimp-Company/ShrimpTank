using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailContent : ContentPopulation
{
    public EmailContentWindow window;

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
                    if(block.GetEmail().ID == email.ID)
                    {
                        flag = true;
                    }
                }
                if (!flag)
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
                if (!flag)
                {
                    ContentBlock block = Instantiate(contentBlock, transform).GetComponent<ContentBlock>();
                    block.GetComponent<EmailContentBlock>().SetEmail(email, window);
                    contentBlocks.Add(block);
                }
            }
            for(int i = contentBlocks.Count; i < 0; i--)
            {
                if (contentBlocks[i] == null)
                {
                    contentBlocks.RemoveAt(i);
                }
            }

        }
    }

    public void ClearSpam()
    {
        foreach(ContentBlock block in contentBlocks)
        {
            if (block != null)
            {
                if (!block.GetComponent<EmailContentBlock>().isImportant())
                {
                    block.GetComponent<EmailContentBlock>().DeleteEmail();
                }
            }
        }
    }
}
