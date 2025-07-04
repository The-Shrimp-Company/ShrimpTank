using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTablet : PlayerUIController
{
    [SerializeField]
    private GameObject tablet;
    private RectTransform _tabletRect;
    [SerializeField]
    private RectTransform _tabletRestingCoord;
    [SerializeField]
    private RectTransform _tabletActiveCoord;
    [SerializeField]
    private TabletInteraction _tabletInteraction;
    private PlayerInput _input;

    [SerializeField] private TextMeshProUGUI notifBar;


    // Start is called before the first frame update
    void Start()
    {
        _tabletRect = tablet.GetComponent<RectTransform>();
        _input = GetComponent<PlayerInput>();
        // Sets the tablets position to the resting position. It's already at that position, but just in case
        RectTools.ChangeRectTransform(_tabletRect, _tabletRestingCoord);
        UIManager.instance.Subscribe(this);
    }

    public void OnOpenTablet()
    {
        UIManager.instance.AssignNotifBar(notifBar);
        _tabletRect.gameObject.SetActive(true);
        UIManager.instance.GetCanvas().GetComponent<MainCanvas>().RaiseTablet();
        UIManager.instance.OpenTabletScreen();
        Tutorial.instance.OpenedTablet();
        _input.SwitchCurrentActionMap("UI");
    }

    public void OnCloseTablet()
    {
        if (UIManager.instance.IsTabletScreen())
        {
            UIManager.instance.AssignNotifBar(notifBar);
            _tabletRect.gameObject.SetActive(true);
            UIManager.instance.GetCanvas().GetComponent<MainCanvas>().LowerTablet();
            UIManager.instance.CloseTabletScreen();
        }
        else
        {
            UIManager.instance.AssignNotifBar(notifBar);
            _tabletRect.gameObject.SetActive(true);
            UIManager.instance.GetCanvas().GetComponent<MainCanvas>().LowerTablet();
            UIManager.instance.ClearScreens();
        }
        
        _input.SwitchCurrentActionMap("Move");
    }


}
