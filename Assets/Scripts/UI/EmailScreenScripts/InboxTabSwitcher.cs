using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InboxTabSwitcher : MonoBehaviour
{
    public Color colour1;
    public Color colour2;

    private List<Image> otherTabs = new();

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform sibling in transform.parent)
        {
            if(sibling.GetSiblingIndex() != transform.GetSiblingIndex())
            {
                if(sibling.GetComponent<Image>() != null)
                {
                    otherTabs.Add(sibling.GetComponent<Image>());
                }
            }
        }
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GetComponent<Image>().color = colour1;
            foreach(Image img in otherTabs) img.color = colour2;
        });
    }
}
