using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InboxTabSwitcher : MonoBehaviour
{
    public Color colour1;
    public Color colour2;

    public List<Image> otherTabs;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GetComponent<Image>().color = colour1;
            foreach(Image img in otherTabs) img.color = colour2;
        });
    }
}
