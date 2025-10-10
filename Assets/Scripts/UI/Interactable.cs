using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    protected bool hovering = false;
    protected bool hoveringEnter = false;
    protected bool wasHovering = false;
    [HideInInspector] public Decoration decoration;
    [HideInInspector] public bool interactable;
    [HideInInspector] public bool holdInteractable;
    [HideInInspector] public bool tooltipAlwaysVisible;  // Whether the tooltip can be seen when the item is not interactable
    protected Dictionary<string, UnityAction> holdActions = new Dictionary<string, UnityAction>();

    private void Awake()
    {
        interactable = true;
        holdInteractable = true;
    }


    protected virtual void MouseHover()  // Call in update on an object that you want the hover function on
    {
        if (hoveringEnter && !wasHovering)
        {
            OnHover();
        }
        if (!hoveringEnter && wasHovering)
        {
            OnStopHover();
        }
        wasHovering = hoveringEnter;
        hoveringEnter = false;
    }

    public void AddHoldAction(string name,  UnityAction action)
    {
        if (holdActions.ContainsKey(name)) return;

        holdActions.Add(name, action);
    }

    public void RemoveHoldAction(string name)
    {
        if (!holdActions.ContainsKey(name)) return;

        holdActions.Remove(name);
    }

    public void UseHoldAction(string name)
    {
        if (!holdActions.ContainsKey(name)) return;

        holdActions[name].Invoke();
    }

    public bool HasHoldActions() { return holdActions.Count != 0; }
    public Dictionary<string, UnityAction> GetHoldActions() { return holdActions; }

    public virtual void Show()
    {
        hoveringEnter = true;
    }

    public virtual void Action()
    {

    }

    public virtual void OnHover()
    {
        hovering = true;
    }

    public virtual void OnStopHover()
    {
        hovering = false;
    }
}
