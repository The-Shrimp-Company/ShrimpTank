using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FakePlayerScript : MonoBehaviour
{
    public RectTransform uiPanel;

    public void OnMouseMove()
    {
        Vector2 pos = Mouse.current.position.value;
        pos.x = Mathf.Clamp(pos.x, uiPanel.position.x - uiPanel.rect.width/2, uiPanel.position.x + uiPanel.rect.width/2);
        pos.y = Mathf.Clamp(pos.y, uiPanel.position.y - uiPanel.rect.height/2, uiPanel.position.y + uiPanel.rect.height/2);
        Mouse.current.WarpCursorPosition(pos);
    }
}
