using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CrossHairSwitch : MonoBehaviour
{
    public Image crosshair;
    public TextMeshProUGUI text;
    private RectTransform crosshairRect;

    [SerializeField] private Sprite crosshairSprite;
    [SerializeField] private float crosshairSize = 1f;

    [SerializeField] private Sprite interactSprite;
    [SerializeField] private float interactSize = 1f;

    [SerializeField] private Ease crosshairEase;

    [HideInInspector] public bool hovering;

    private void Start()
    {
        crosshairRect = crosshair.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(hovering == false && text.enabled)
        {
            if (crosshair.sprite != crosshairSprite)
            {
                ChangeSprite(crosshairSprite, crosshairSize);
                FadeText(0);
            }
        }
        else
        {
            if (crosshair.sprite != interactSprite)
            {
                ChangeSprite(interactSprite, interactSize);
                FadeText(1);
            }
        }
    }

    private void ChangeSprite(Sprite sprite, float size)
    {
        crosshair.sprite = sprite;
        DOTween.Kill(crosshairRect);
        crosshairRect.DOScale(new Vector2(size, size), 0.2f).SetEase(crosshairEase);
    }

    private void FadeText(float alpha)
    {
        DOTween.Kill(text);
        text.DOFade(alpha, 0.15f);
    }
}
