using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

static public class FontTools
{
    static public void SizeFont(TextMeshProUGUI text)
    {
        Canvas.ForceUpdateCanvases();
        Rect textRect = text.GetComponent<RectTransform>().rect;
        int textLength = text.text.Length;
        //Debug.Log(textRect.width);
        //text.fontSize = textRect.width / textLength > textRect.height ? textRect.height : textRect.width / textLength;
    }
}
