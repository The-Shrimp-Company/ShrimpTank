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
