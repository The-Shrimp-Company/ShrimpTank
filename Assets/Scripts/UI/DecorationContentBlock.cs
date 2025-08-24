using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecorationContentBlock : ContentBlock
{
    private ItemSO so;

    public Image buttonSprite;
    public Button button;

    public TMP_Text priceText;
    public TMP_Text ownedText;

    public Color inInventoryColour;
    public Color notInInventoryColour;
    public Color cannotAffordColour;
    public Color selectedColour;

    public void SetDecoration(ItemSO d)
    {
        so = d;
    }
}
