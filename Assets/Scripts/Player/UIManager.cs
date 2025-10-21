using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UI;


public delegate void NotifEvent(string message, bool sound);

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public event NotifEvent SendNotification;

    public bool subMenu = false;

    public bool triggerSound = false;

    public PlayerInput input;

    public float notifTime = 0;
    public float notifTimer = 5;

    public GameObject tooltips { set; private get; }

    private Transform MainCanvas;
    private Camera SecondCamera;
    private TextMeshProUGUI notifBar;

    private string _currentText = "Notifications Online";

    private List<PlayerUIController> _playerControllers = new List<PlayerUIController>();

    private Rect _currentRect = new Rect();
    private GameObject _cursor;

    private Stack<ScreenView> _screenStack = new Stack<ScreenView>();
    private Stack<ScreenView> _tabletStack = new Stack<ScreenView>();

    [SerializeField] private ScreenView TabletView;
    public Transform tabletParent;

    public void Awake()
    {
        if(instance != this)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _tabletStack.Push(TabletView);
    }

    private void Update()
    {
        // Manually overrides the event systems selections to keep button animations working nicely. (Doesn't apply during input events like keyboard entries)
        if(EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.CompareTag("Input"))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        // If the timer is up, it get's the next notification to send to the player
        if(notifTime > notifTimer)
        {
            _currentText = EmailManager.instance.GetNotification();
            notifTime = 0;
        }
        else
        {
            notifTime += Time.deltaTime;
        }

        // If there is a notification bar available, it sends the current notification every frame
        if (SendNotification != null)
        {
            SendNotification(_currentText, triggerSound);
            triggerSound = false;
        }
    }

    public void OpenScreen(ScreenView newScreen)
    {
        newScreen.Open(false);
        if (_screenStack.Count != 0)
        {
            if (_screenStack.Peek() != null)
                _screenStack.Peek().gameObject.SetActive(false);
            else _screenStack.Pop();
        }

        _screenStack.Push(newScreen);

        SetPeripherals();

        foreach (PlayerUIController controller in _playerControllers)
        {
            controller.SwitchFocus();
        }
    }


    public void CloseScreen()
    {
        if(_screenStack.Count == 0)
        {
            Debug.LogWarning("You've called close screen with an empty screen stack. That shouldn't happen, probably fix it.");
            return;
        }
        else
        {
            ScreenView oldScreen = _screenStack.Pop();
            
            oldScreen?.Close(false);
        }

        if(_screenStack.Count != 0)
        {
            _screenStack.Peek().gameObject.SetActive(true);
        }

        SetPeripherals();

        foreach (PlayerUIController controller in _playerControllers)
        {
            controller.SwitchFocus();
        }
    }

    public void SwitchScreen(ScreenView newScreen)
    {
        if (_screenStack.Count == 0)
        {
            OpenScreen(newScreen);
            Debug.Log("You've called switch screen from an empty screen stack. Are you sure you meant to do that?");
        }
        else
        {
            if (_screenStack.Count == 0)
            {
                Debug.LogWarning("You've called close screen with an empty screen stack. That shouldn't happen, probably fix it.");
                return;
            }
            else
            {
                ScreenView oldScreen = _screenStack.Pop();
                
                if (oldScreen != null)
                    oldScreen?.Close(false);
            }
            OpenScreen(newScreen);
        }
    }

    

    public void ClearScreens()
    {
        while(_screenStack.Count > 0)
        {
            CloseScreen();
        }
    }

    public ScreenView GetScreen()
    {
        if(_screenStack.Count > 0)
        {
            return _screenStack.Peek();
        }
        else
        {
            return null;
        }
    }

    public int CheckLevel()
    {
        return _screenStack.Count;
    }

    public void OpenTabletScreen()
    {
        //ClearScreens();
        _screenStack = _tabletStack;

        SetPeripherals();

        foreach (PlayerUIController controller in _playerControllers)
        {
            controller.SwitchFocus();
        }
    }

    public void CloseTabletScreen()
    {
        _screenStack = new Stack<ScreenView>();

        SetPeripherals();

        foreach (PlayerUIController controller in _playerControllers)
        {
            controller.SwitchFocus();
        }
    }

    public bool IsTabletScreen()
    {
        return _tabletStack == _screenStack;
    }

    public void RefreshNotif()
    {
        _currentText = EmailManager.instance.GetNotification();
        notifTime = 0;
    }

    public void PushNotification(string text, bool sound)
    {
        if(SendNotification != null)
        {
            SendNotification(text, sound);
        }
    }

    /// <summary>
    /// Adds the given controller to the notification list, so when focus is switched, the necessary components will be alerted
    /// </summary>
    /// <param name="newController"></param>
    public void Subscribe(PlayerUIController newController)
    {
        _playerControllers.Add(newController);
    }

    public Rect GetCurrentRect() { return _currentRect; }

    public void SetCamera(Camera cam)
    {
        SecondCamera = cam;
    }

    public Camera GetCamera() { return SecondCamera; }

    public void SetCursor(GameObject cursor)
    {
        _cursor = cursor;
        Cursor.visible = false;
        _cursor.SetActive(false);
    }

    public GameObject GetCursor() { return _cursor; }

    public void SetCursorMasking(bool masking) { _cursor.GetComponent<FakeCursor>().SetCursorMasking(masking); }

    public void SetCanvas(Transform transform)
    {
        MainCanvas = transform;
    }

    public Transform GetCanvas() { return MainCanvas; }
    public GameObject GetTooltips() { return tooltips; }


    public void AssignNotifBar(TextMeshProUGUI notif)
    {
        notifBar = notif;
        notifBar.text = _currentText;
    }

    
    
    /// <summary>
    /// Function to handle setting all the strange settings based on whether there is
    /// currently a screen open.
    /// </summary>
    private void SetPeripherals()
    {
        if (_screenStack.Count == 0)
        {
            _cursor.SetActive(false);
            tooltips.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            input.SwitchCurrentActionMap("Move");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            tooltips.SetActive(false);
            _cursor.SetActive(true);
            if (IsTabletScreen())
            {
                input.SwitchCurrentActionMap("UI");
            }
            else
            {
                input.SwitchCurrentActionMap("TankView");
            }
        }
        Cursor.visible = false;
    }


    public void ToggleUIVisibility()  // Hides and shows the UI (ONLY FOR GETTING SCREENSHOTS, MAY BREAK THE GAME)
    {
        if (MainCanvas == null) return;

        bool enable = !MainCanvas.gameObject.activeInHierarchy;

        MainCanvas.gameObject.SetActive(enable);
        Cursor.visible = enable;
        TabletView.gameObject.SetActive(enable);
        _screenStack.Peek().gameObject.SetActive(enable);
        _tabletStack.Peek().gameObject.SetActive(enable);
    }
}
