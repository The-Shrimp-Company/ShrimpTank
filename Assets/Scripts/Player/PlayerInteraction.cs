using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    [SerializeField] private float holdInteractionLength = 1f;

    private CameraLookCheck lookCheck;
    private Camera _camera;
    private PlayerInput _input;
    private GameObject _tankView;
    private Vector2 press;
    private bool _pressed;
    [SerializeField] public LayerMask layerMask;


    [Header("Hold Menu")]
    private float holdTime;
    private GameObject holdTarget;
    public RMF_RadialMenu radialMenu;
    public Image holdSlider1, holdSlider2;

    // Start is called before the first frame update
    void Start()
    {
        lookCheck = GetComponentInChildren<CameraLookCheck>();
        _camera = GetComponentInChildren<Camera>();
        _input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (holdTarget != null)
        {
            holdTime += Time.deltaTime;

            float l = Mathf.InverseLerp(holdInteractionLength, 0, holdTime) * 2;
            holdSlider1.fillAmount = Mathf.Clamp(l - 1, 0, 1);
            holdSlider2.fillAmount = Mathf.Clamp(l, 0, 1);

            if (holdTime >= holdInteractionLength)
            {
                HoldLeftClick(true);
                holdTarget = null;
                holdTime = 0f;
            }

            if (holdTarget != lookCheck.LookCheck(3, layerMask))
            {
                holdTarget = null;
                holdTime = 0f;
            }
        }
        else
        {
            holdSlider1.fillAmount = 0;
            holdSlider2.fillAmount = 0;
        }
    }

    /// <summary>
    /// Called by the input manager
    /// Can only be called from the move action map, used to interact with tanks
    /// Future functionality that requires the player click on something should take place
    /// here as well, although lookCheck should be refactored to allow more options
    /// if that happens.
    /// </summary>
    public void OnPlayerClick(InputValue key)
    {
        if (radialMenu.menuOpen) return;

        if (key.Get<float>() == 1)
        {
            holdTarget = lookCheck.LookCheck(3, layerMask);
        }
        if (key.Get<float>() == 0 && holdTarget != null)
        {
            if (holdTime < holdInteractionLength) LeftClick(key.isPressed);
            else HoldLeftClick(key.isPressed);
            holdTarget = null;
            holdTime = 0f;
        }
    }

    private void LeftClick(bool pressed)
    {
        if (Store.decorateController.decorating)
        {
            Store.decorateController.MouseClick(pressed);
        }
        else
        {
            if (holdTarget != null && holdTarget.GetComponent<Interactable>() != null)
            {
                holdTarget.GetComponent<Interactable>().Action();
            }
        }
    }

    private void HoldLeftClick(bool pressed)
    {
        if (holdTarget == null || holdTarget.GetComponent<Interactable>() == null || !holdTarget.GetComponent<Interactable>().HasHoldActions()) 
        {
            LeftClick(pressed);
            return;
        }

        radialMenu.DisplayMenu(holdTarget.GetComponent<Interactable>().GetHoldActions());
    }

    public void OnPlayerRightClick(InputValue key)
    {
        if (key.isPressed)
        {
            if (Store.decorateController.decorating)
                Store.decorateController.StopPlacing();
        }
    }


    public void SetTankFocus(TankController tankController)
    {
        if (tankController != null)
        {
            if (_tankView != null) _tankView.GetComponent<TankController>().StopFocusingTank();

            _camera.transform.position = tankController.GetCam().transform.position;
            _camera.transform.rotation = tankController.GetCam().transform.rotation;

            //Debug.Log("GetHere");

            _input.SwitchCurrentActionMap("TankView");

            tankController.FocusTank();
            _tankView = tankController.gameObject;
        }
        else Debug.LogWarning("Cannot find tank to focus.");
    }

    /// <summary>
    /// Can only be called from tank view action map
    /// Will switch the focus from one tank to the next in the direction clicked
    /// </summary>
    /// <param name="Key"></param>
    public void OnSwitchTank(InputValue Key)
    {
        TankViewScript view;
        UIManager.instance.GetScreen().TryGetComponent(out view);
        if (view != null)
        {
            if (Key.Get<Vector2>().normalized != press)
            {
                press = Key.Get<Vector2>().normalized;
                if (press != Vector2.zero || press != null)
                {

                    TankController nextTank;
                    foreach (RaycastHit hit in Physics.RaycastAll(_tankView.GetComponent<Collider>().bounds.center, _tankView.transform.TransformDirection(new Vector3(-press.x, press.y, 0)), 1f, layerMask: LayerMask.GetMask("RoomDecoration")))
                    {
                        nextTank = null;
                        hit.transform.TryGetComponent(out nextTank);
                        if (nextTank != null)
                        {
                            SetTankFocus(nextTank);
                        }
                    }
                }
                else
                {
                    //Debug.Log("HJEHFSDHFSD");
                }
            }
        }
    }

    /// <summary>
    /// Can only be called from the tank view action map
    /// Will leave tank view
    /// </summary>
    public void OnExitView()
    {
        if (_tankView != null)
        {
            _tankView.GetComponent<TankController>().StopFocusingTank();
            _camera.transform.localPosition = Vector3.up / 2;
            RoomGridNode pos = ShopGrid.Instance.GetTankTeleportPosition(_tankView.GetComponent<TankController>().GetCam().transform.position, 3);
            if (pos != null) transform.position = new Vector3(pos.worldPos.x, transform.position.y, pos.worldPos.z);
            _tankView = null;
        }
        UIManager.instance.ClearScreens();
    }



    /// <summary>
    /// Should only be called from the tank view action map. Used to interact with shrimp.
    /// </summary>
    /// <param name="point"></param>
    public void OnTankClick(InputValue point)
    {
        if (point.isPressed)
        {
            if (UIManager.instance.GetScreen().TryGetComponent<TankDecorateViewScript>(out TankDecorateViewScript tdv))
            {
                DecorateTankController.Instance.MouseClick(Mouse.current.position.value, point.isPressed);
            }
            if (UIManager.instance.GetScreen().TryGetComponent<TankViewScript>(out TankViewScript tv))
            {
                tv.MouseClick(Mouse.current.position.value, point.isPressed);
            }
            if(UIManager.instance.GetScreen().TryGetComponent<ShrimpView>(out ShrimpView sv))
            {
                sv.MouseClick(Mouse.current.position.value);
            }

            //UIManager.instance.GetFocus().GetComponent<TankViewScript>().MouseClick(Mouse.current.position.value, point.isPressed);
        }
    }

    public void OnCancel()
    {
        Debug.Log("Function Called");
        if (!UIManager.instance.IsTabletScreen())
        {
            UIManager.instance.GetScreen()?.Exit();
        }
    }
}
