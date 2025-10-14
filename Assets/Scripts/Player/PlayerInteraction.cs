using System.Reflection;
using TMPro;
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
    [SerializeField] public LayerMask shelfLayerMask;
    [SerializeField] public LayerMask layerMask;
    [SerializeField] private TextMeshProUGUI tooltip;

    [Header("Hold Menu")]
    private GameObject hoverTarget;
    [HideInInspector] public Interactable targetInteractable;
    public RMF_RadialMenu radialMenu;


    void Start()
    {
        lookCheck = GetComponentInChildren<CameraLookCheck>();
        _camera = GetComponentInChildren<Camera>();
        _input = GetComponent<PlayerInput>();
    }


    private void Update()
    {
        if (Store.decorateController.decorating) 
        {
            hoverTarget = null;
            targetInteractable = null;
            return;
        }


        hoverTarget = lookCheck.LookCheck(3, shelfLayerMask);
        if (hoverTarget != null)
        {
            if (hoverTarget.GetComponent<Decoration>()) targetInteractable = hoverTarget.GetComponent<Decoration>().interactable;
            else if (hoverTarget.GetComponent<Interactable>()) targetInteractable = hoverTarget.GetComponent<Interactable>();
            else targetInteractable = hoverTarget.GetComponentInChildren<Interactable>();
        }
        else targetInteractable = null;

        if (targetInteractable && targetInteractable.decoration && targetInteractable.decoration.CheckForItemsOnShelf())  // If it is a shelf and has items on it, ignore it and check again
        {
            hoverTarget = lookCheck.LookCheck(3, layerMask);
            if (hoverTarget != null)
            {
                if (hoverTarget.GetComponent<Decoration>()) targetInteractable = hoverTarget.GetComponent<Decoration>().interactable;
                else if (hoverTarget.GetComponent<Interactable>()) targetInteractable = hoverTarget.GetComponent<Interactable>();
                else targetInteractable = hoverTarget.GetComponentInChildren<Interactable>();
            }
            else targetInteractable = null;
        }

        if (targetInteractable) targetInteractable.Show();

        if (hoverTarget && hoverTarget.GetComponent<ToolTip>()) tooltip.text = hoverTarget.GetComponent<ToolTip>().toolTip;
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
            if (Store.decorateController.decorating)
            {
                Store.decorateController.MouseClick(key.isPressed);
            }
            else
            {
                if (targetInteractable && targetInteractable.interactable) targetInteractable.Action();
            }
        }
    }


    public void OnPlayerRightClick(InputValue key)
    {
        if (radialMenu.menuOpen) return;

        if (key.Get<float>() == 1)
        {
            if (Store.decorateController.decorating)
                Store.decorateController.StopPlacing();
            else if (GetComponent<HeldItem>().GetHeldItem() != null)
                GetComponent<HeldItem>().StopHoldingItem();
            else
            {
                if (!targetInteractable || !targetInteractable.holdInteractable || !targetInteractable.HasHoldActions()) return;

                radialMenu.DisplayMenu(targetInteractable.GetHoldActions(), targetInteractable.decoration.decorationSO.itemName);
            }
        }
    }


    public void SetTankFocus(TankController tankController)
    {
        if (tankController != null && tankController.waterFilled)
        {
            if (_tankView != null) _tankView.GetComponent<TankController>().StopFocusingTank();

            _camera.transform.position = tankController.GetCam().transform.position;
            _camera.transform.rotation = tankController.GetCam().transform.rotation;

            //Debug.Log("GetHere");

            _input.SwitchCurrentActionMap("TankView");

            tankController.FocusTank();
            _tankView = tankController.gameObject;
        }
    }

    /// <summary>
    /// Can only be called from tank view action map
    /// Will switch the focus from one tank to the next in the direction clicked
    /// </summary>
    /// <param name="Key"></param>
    public void OnSwitchTank(InputValue Key)
    {
        TankViewScript view = UIManager.instance.GetScreen()?.GetComponent<TankViewScript>();
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
                        if (nextTank != null && nextTank.waterFilled)
                        {
                            SetTankFocus(nextTank);
                        }
                    }
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
