using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraLookCheck : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI toolTip;
    private CrossHairSwitch crosshair;

    private void Start()
    {
        crosshair = toolTip.transform.parent.GetComponent<CrossHairSwitch>();
        if (crosshair == null) Debug.LogWarning("CameraLookCheck cannot find crosshair");
    }

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

    public void Update()
    {
        if (toolTip.enabled)
        {
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            LayerMask layer = LayerMask.GetMask("RoomDecoration") | LayerMask.GetMask("Decoration") | LayerMask.GetMask("Tanks");

            if (Physics.Raycast(transform.position, fwd, out hit, 3f, layer))
            {
                if (hit.collider)
                {
                    if (hit.collider.GetComponent<Interactable>())
                    {
                        hit.collider.GetComponent<Interactable>().Show();
                    }
                    if (hit.collider.GetComponent<ToolTip>())
                    {
                        toolTip.text = hit.collider.GetComponent<ToolTip>().toolTip;
                        crosshair.hovering = true;
                        return;
                    }
                }
            }

            //toolTip.text = "";
            crosshair.hovering = false;
        }
    }
}
