using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailContent : ContentPopulation
{
    public EmailContentWindow window;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Email email in EmailManager.instance.emails)
        {
            ContentBlock block = Instantiate(contentBlock, transform).transform.GetChild(0).GetComponent<ContentBlock>();
            block.GetComponent<EmailContentBlock>().SetEmail(email, window);
            contentBlocks.Add(block);
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
