using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ShelfGridNode : MonoBehaviour
{
    [SerializeField] public RoomGridNode roomGridNode;

    private void Start()
    {
        roomGridNode.worldPos = transform.position;
        CheckNodeValidity();
    }

    private void CheckNodeValidity()
    {
        LayerMask layer = LayerMask.GetMask("RoomDecoration");
        RaycastHit[] hit;

        if (Physics.CheckSphere(transform.position + new Vector3(0, 0.1f, 0), 0.1f, layer, QueryTriggerInteraction.Collide))
            roomGridNode.invalid = true;
    }
}
