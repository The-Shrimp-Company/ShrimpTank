using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class PlayerUIController : MonoBehaviour
{
    private ShrimpCam _cam;

    protected Rect _currentAreaRect;
    protected GameObject cursor;
    protected RectTransform _cursorRect;

    private float control;
    private float zoom;

    public void OnShrimpCamMove(InputValue input)
    {
        ShrimpView view;
        UIManager.instance.GetScreen().TryGetComponent(out view);
        if (view != null)
        {
            if(_cam != null)
            {
                control = input.Get<Vector2>().x;
                zoom = input.Get<Vector2>().y;
            }
        }
    }


    public void Update()
    {
        if (control != 0)
        {
            _cam.transform.parent.Rotate(0, -control, 0);
        }
        if (zoom < 0)
        {
            _cam.ChangeZoom(false);
        }
        else if (zoom > 0)
        {
            _cam.ChangeZoom(true);
        }
    }

    public virtual void SwitchFocus()
    {
        if (UIManager.instance.GetScreen() != null)
        {
            _currentAreaRect = UIManager.instance.GetCurrentRect();
        }
    }

    public void OnMoveMouse()
    {
        if (UIManager.instance.GetScreen() == null) return;

        Vector2 pos = Mouse.current.position.value;
        RectTransform uiPanel = UIManager.instance.GetScreen().GetComponent<RectTransform>();
        float localScale = UIManager.instance.GetCanvas().GetComponent<Canvas>().scaleFactor;
        Vector2 scale = UIManager.instance.GetCanvas().GetComponent<RectTransform>().localScale;

        //Vector2 bottomCorner = new Vector2(uiPanel.anchorMin, centre.center.y * localScale - (uiPanel.rect.height * localScale) / 2);
        //ector2 topCorner = new Vector2(centre.center.x * localScale + (uiPanel.rect.width * localScale) /2, centre.center.y * localScale + (uiPanel.rect.height * localScale) / 2);

        Vector2 centre = Camera.main.WorldToScreenPoint(uiPanel.position);
        float left = centre.x - uiPanel.rect.width / 2 * localScale;
        float right = centre.x + uiPanel.rect.width / 2 * localScale;
        float bottom = centre.y - uiPanel.rect.height / 2 * localScale;
        float top = centre.y + uiPanel.rect.height / 2 * localScale;



        pos.x = Mathf.Clamp(pos.x, left, right);
        pos.y = Mathf.Clamp(pos.y, bottom, top);


        if(Mouse.current.position.value != pos) Mouse.current.WarpCursorPosition(pos);
    }

    public void SetShrimpCam(ShrimpCam cam)
    {
        _cam = cam;
        control = 0;
        zoom = 0;
    }

    public void UnsetShrimpCam()
    {
        _cam = null;
        control = 0;
        zoom = 0;
    }
}
