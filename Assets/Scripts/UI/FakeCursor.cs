using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using DG.Tweening;

public class FakeCursor : MonoBehaviour
{
    private RectTransform rect;
    private RectTransform imageRect;
    private Image image;
    [SerializeField] private bool enableBounce = true;
    [SerializeField] private Ease crosshairEase = Ease.OutBack;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        imageRect = transform.GetChild(0).GetComponent<RectTransform>();
        image = transform.GetChild(0).GetComponent<Image>();
        UIManager.instance.SetCursor(gameObject);
    }

    private void OnEnable()
    {
        if (imageRect == null || !enableBounce) return;

        imageRect.localScale = Vector2.zero;
        imageRect.DOScale(Vector2.one, 0.2f).SetEase(crosshairEase);  // Make the cursor smoothly appear
    }

    private void Update()
    {
        rect.position = Mouse.current.position.value / rect.localScale;
    }

    public void SetCursorMasking(bool masking) { image.maskable = masking; }

    public Image GetImage() { return image; }
}
