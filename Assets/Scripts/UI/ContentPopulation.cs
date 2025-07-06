using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Root class of the content population class. This class is used to generate
/// numbers of content blocks when the screen is instantiated. Exists on the
/// content object in the screen prefab. Cannot access the screen object 
/// directly.
/// </summary>
public class ContentPopulation : MonoBehaviour
{

    [SerializeField]
    protected GameObject contentBlock;

    protected List<ContentBlock> contentBlocks = new List<ContentBlock>();

    

    protected virtual void CreateContent(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            contentBlocks.Add(Instantiate(contentBlock, transform).GetComponent<ContentBlock>());
        }
    }

    protected virtual void ClearContent()
    {
        for (int i = contentBlocks.Count - 1; i >= 0; i--)
        {
            Destroy(contentBlocks[i].gameObject);
            contentBlocks.RemoveAt(i);
        }

        contentBlocks.Clear();
    }
}
