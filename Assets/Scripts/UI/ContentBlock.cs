using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Root content block class. Content blocks are the individual items on
/// the various tablet screens.
/// </summary>
public class ContentBlock : MonoBehaviour
{
    [SerializeField]
    protected Sprite[] backSprites;

    [SerializeField]
    protected TextMeshProUGUI text;

    public virtual void SetText(string textToSet)
    {
        Canvas.ForceUpdateCanvases();
        text.text = textToSet;
        //FontTools.SizeFont(text);
        //text.fontSize = text.fontSize * 0.9f;
    }

    public TextMeshProUGUI GetText()
    {
        return text;
    }

    public void AssignFunction(UnityAction func)
    {
        GetComponent<Button>().onClick.AddListener(func);
    }

    public void ClearFunctions()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }
}