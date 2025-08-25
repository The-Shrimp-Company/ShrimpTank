using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ToolTip))]
public class Lamp : Interactable
{
    [SerializeField] GameObject lightObj;

    [SerializeField] string enabledTooltip;
    [SerializeField] string disabledTooltip;
    ToolTip tooltip;

    [SerializeField] bool defaultState;
    bool state;

    public void Start()
    {
        tooltip = GetComponent<ToolTip>();
        SetState(defaultState);
    }

    public override void Action()
    {
        SetState(!state);
        base.Action();
    }

    private void SetState(bool s)
    {
        state = s;
        lightObj.SetActive(s);

        if (tooltip == null) return;

        if (state) tooltip.toolTip = enabledTooltip;
        else tooltip.toolTip = disabledTooltip;
    }
}
