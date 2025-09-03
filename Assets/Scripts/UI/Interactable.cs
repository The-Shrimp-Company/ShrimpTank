using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    protected bool hovering = false;
    protected bool wasHovering = false;
    [HideInInspector] public Decoration decoration;
    [HideInInspector] public bool interactable;
    protected Dictionary<string, UnityAction> holdActions = new Dictionary<string, UnityAction>();

    private void Awake()
    {
        interactable = true;
    }

    // Update is called once per frame
    protected virtual void MouseHover()
    {
        if (hovering && !wasHovering)
        {
            OnHover();
        }
        if (!hovering && wasHovering)
        {
            OnStopHover();
        }
        wasHovering = hovering;
        hovering = false;
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
        hovering = true;
    }

    public virtual void Action()
    {

    }

    public virtual void OnHover()
    {

    }

    public virtual void OnStopHover()
    {

    }
}
