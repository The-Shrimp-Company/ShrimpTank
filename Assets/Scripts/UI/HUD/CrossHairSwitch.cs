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

    [SerializeField] private Sprite leftInteractSprite;
    [SerializeField] private Sprite rightInteractSprite;
    [SerializeField] private Sprite bothInteractSprite;
    [SerializeField] private float interactSize = 1f;

    [SerializeField] private Ease crosshairEase;

    public PlayerInteraction playerInteraction;

    private void Start()
    {
        crosshairRect = crosshair.GetComponent<RectTransform>();
    }

    private void Update()
    {
        Interactable target = playerInteraction.targetInteractable;
        if (target == null)
        {
            ChangeSprite(crosshairSprite, crosshairSize);
            FadeText(0);
        }
        else if (target.interactable && target.holdInteractable && target.HasHoldActions())
        {
            ChangeSprite(bothInteractSprite, interactSize);
            FadeText(1);
        }
        else if (target.holdInteractable && target.HasHoldActions())
        {
            ChangeSprite(rightInteractSprite, interactSize);
            FadeText(0);
        }
        else if (target.interactable)
        {
            ChangeSprite(leftInteractSprite, interactSize);
            FadeText(1);
        }
        else
        {
            ChangeSprite(crosshairSprite, crosshairSize);
            FadeText(0);
        }
    }

    private void ChangeSprite(Sprite sprite, float size)
    {
        if (crosshair.sprite == sprite) return;
        crosshair.sprite = sprite;
        DOTween.Kill(crosshairRect);
        crosshairRect.DOScale(new Vector2(size, size), 0.2f).SetEase(crosshairEase);
    }

    private void FadeText(float alpha)
    {
        if (text.alpha == alpha) return;
        DOTween.Kill(text);
        text.DOFade(alpha, 0.15f);
    }
}
