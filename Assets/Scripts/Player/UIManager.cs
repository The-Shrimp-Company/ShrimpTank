using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public bool subMenu = false;

    public PlayerInput input;

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
        if(EventSystem.current.currentSelectedGameObject != null && !EventSystem.current.currentSelectedGameObject.CompareTag("Input"))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OpenScreen(ScreenView newScreen)
    {
        newScreen.Open(false);
        if (_screenStack.Count != 0)
        {
            _screenStack.Peek().gameObject.SetActive(false);
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
                
                oldScreen?.Close(false);
            }
            OpenScreen(newScreen);
        }
    }

    

    public void ClearScreens()
    {
        for(int i = 0; i <= _screenStack.Count; i++)
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

    public void AssignNotifBar(TextMeshProUGUI notif)
    {
        notifBar = notif;
        notifBar.text = _currentText;
    }

    public void SendNotification(string notif, bool mute = false)
    {
        if(notifBar == null)
        {
            return;
        }
        if (!mute && _currentText != notifBar.text) notifBar.GetComponent<AudioSource>().Play();
        _currentText = notif;
        notifBar.text = notif;
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
