using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairScript : PlayerUIController
{
    public static TextMeshProUGUI toolTipText;
    public static Image crosshair;

    private void Start()
    {
        UIManager.instance.Subscribe(this);
        toolTipText = GetComponent<CrossHairSwitch>().text;
        crosshair = GetComponent<CrossHairSwitch>().crosshair;
    }

    public override void SwitchFocus()
    {
        if (!toolTipText.IsDestroyed())
        {
            if (UIManager.instance.GetScreen() == null)
            {
                ShowCrosshair();
            }
            else
            {
                HideCrosshair();
            }
        }
    }

    public static void ShowCrosshair()
    {
        toolTipText.enabled = true;
        toolTipText.alpha = 0f;
        crosshair.enabled = true;
    }

    public static void HideCrosshair()
    {
        toolTipText.enabled = false;
        crosshair.enabled = false;
    }
}
