using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private ShelfSpawn shelves;

    [SerializeField]
    private GameObject inventory;

    private CameraLookCheck lookCheck;
    private Camera _camera;
    private PlayerInput _input;
    private GameObject _tankView;
    private Vector2 press;
    private bool _pressed;

    // Start is called before the first frame update
    void Start()
    {
        lookCheck = GetComponentInChildren<CameraLookCheck>();
        _camera = GetComponentInChildren<Camera>();
        _input = GetComponent<PlayerInput>();
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
        if (key.isPressed)
        {
            if (Store.decorateController.decorating)
            {
                Store.decorateController.MouseClick(key.isPressed);
            }
            else
            {
                LayerMask layer = LayerMask.GetMask("RoomDecoration") | LayerMask.GetMask("Decoration") | LayerMask.GetMask("Tanks");
                GameObject target = lookCheck.LookCheck(3, layer);

                if (target != null)
                {
                    if (target.GetComponent<TankController>() != null)
                    {
                        SetTankFocus(target.GetComponent<TankController>());
                    }
                    else if (target.GetComponent<TankSocket>() != null)
                    {
                        GameObject invenScreen = UIManager.instance.GetCanvas().GetComponent<MainCanvas>().RaiseScreen(inventory);
                        GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
                        invenScreen.GetComponentInChildren<InventoryContent>().TankAssignment(target);

                    }
                    else if (target.GetComponent<Interactable>() != null)
                    {
                        target.GetComponent<Interactable>().Action();
                    }
                }
            }
        }
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
        if (UIManager.instance.GetScreen().GetComponent<TankViewScript>() != null)
        {
            if (Key.Get<Vector2>().normalized != press)
            {
                press = Key.Get<Vector2>().normalized;
                if (press != Vector2.zero || press != null)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(_tankView.GetComponent<Collider>().bounds.center, _tankView.transform.TransformDirection(new Vector3(-press.x, press.y, 0)), out hit, 1f, layerMask: LayerMask.GetMask("Tanks")))
                    {
                        //Debug.Log("YEah");
                        SetTankFocus(hit.transform.GetComponent<TankController>());
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
            Vector3 v3 = _tankView.GetComponent<TankController>().GetCam().transform.position;
            transform.position = new Vector3(v3.x, transform.position.y, v3.z);
            _camera.transform.localPosition = Vector3.up / 2;
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
