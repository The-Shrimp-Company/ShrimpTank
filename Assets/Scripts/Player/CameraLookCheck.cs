using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraLookCheck : MonoBehaviour
{
    public GameObject LookCheck(float Distance, LayerMask layer)
    {
        RaycastHit hit;

        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if(layer != -1)
        {
            if (Physics.Raycast(transform.position, fwd, out hit, Distance, layer))
            {
                return hit.collider.gameObject;
            }
        }
        else
        {
            if(Physics.Raycast(transform.position, fwd, out hit, Distance))
            {
                return hit.collider.gameObject;
            }
        }

        return null;
    }
}
