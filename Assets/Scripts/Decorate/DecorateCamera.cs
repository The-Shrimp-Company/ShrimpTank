using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorateCamera : MonoBehaviour
{
    public Transform lookAt;  // Leave empty to use camera rotation

    public float topCamHeight;
    public float topCamClippingPlane = 0.01f;
    public Transform topCamLookAt;  // Leave empty to use camera rotation
}
