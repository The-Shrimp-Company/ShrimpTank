using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public void OnRotate(InputValue value)
    {
        DecorateTankController.Instance.OnRotate(value);
        Store.decorateController.OnRotate(value);
    }

    public void OnChangeCam(InputValue value)
    {
        DecorateTankController.Instance.OnChangeCam(value);
    }

    public void OnHideRadialMenu(InputValue value)
    {
        GetComponent<PlayerInteraction>().radialMenu.HideMenu();
    }
}
