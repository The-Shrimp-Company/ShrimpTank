using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    public Transform cameraTransform;
    public float lookSenstivity;
    [SerializeField][Range(50f, 250f)] private float startingSensitivity = 100f;
    [SerializeField] private float cameraHeight = 2.3f;

    private PlayerInput _playerInput;

    private Vector2 _look;
    private float _rotY;
    private float _rotX;


    private void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerInput = GetComponent<PlayerInput>();

        LoadSensitivity();
    }


    private void Update()
    {
        // If the player is in a menu that stops their movement
        if (UIManager.instance.GetScreen())
            _look = Vector2.zero;

        // If the player is not using the correct action map
        if (_playerInput.currentActionMap.name != "Move")
            return;

        _look *= Time.deltaTime;

        _rotY += _look.y * lookSenstivity;
        _rotY = Mathf.Clamp(_rotY, -75, 45);
        _rotX += _look.x * lookSenstivity;
        cameraTransform.localRotation = Quaternion.Euler(-_rotY, 0, 0);

        transform.rotation = Quaternion.Euler(0, _rotX, 0);

        

        cameraTransform.position = new Vector3(cameraTransform.position.x, cameraHeight, cameraTransform.position.z);
        _look = Vector2.zero;
        if(_rotX > 360)
        {
            _rotX -= 360;
        }
        else if(_rotX < -360)
        {
            _rotX += 360;
        }
    }

    

    public void OnLook(InputValue Mouse)
    {
        _look += Mouse.Get<Vector2>();
    }


    public void LoadSensitivity()
    {
        if (!PlayerPrefs.HasKey("Sensitivity"))
            PlayerPrefs.SetFloat("Sensitivity", startingSensitivity);

        lookSenstivity = PlayerPrefs.GetFloat("Sensitivity");
    }

    public void SetRotationX(float rot) { _rotX = rot; }
    public float GetRotationX() { return _rotX; }
}
