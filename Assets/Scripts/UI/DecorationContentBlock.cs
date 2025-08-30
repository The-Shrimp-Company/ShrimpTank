using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class DecorationContentBlock : ContentBlock, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private ItemSO so;

    public Image buttonSprite;
    public Button button;

    public TMP_Text priceText;
    public TMP_Text ownedText;

    [Header("Colours")]
    public Color inInventoryColour;
    public Color notInInventoryColour;
    public Color cannotAffordColour;
    public Color selectedColour;

    [Header("Scale")]
    public Vector3 hoverScale;
    public float hoverScaleDuration;

    public void SetDecoration(ItemSO d)
    {
        so = d;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(hoverScale, hoverScaleDuration);
        transform.DOLocalRotate(Vector3.zero, hoverScaleDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(Vector3.one, hoverScaleDuration);
        transform.DOLocalRotate(Vector3.zero, hoverScaleDuration);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOPunchRotation(new Vector3(0, 0, 10), 0.5f);
    }
}
