using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfGridNode : MonoBehaviour
{
    [SerializeField] public RoomGridNode roomGridNode;

    private void Start()
    {
        roomGridNode.worldPos = transform.position;
        CheckNodeValidity();
    }

    public void CheckNodeValidity()
    {
        roomGridNode.invalid = false;
        LayerMask layer = LayerMask.GetMask("RoomDecoration");
        RaycastHit[] hit;

        if (Physics.CheckSphere(transform.position + new Vector3(0, 0.1f, 0), 0.1f, layer, QueryTriggerInteraction.Collide))
            roomGridNode.invalid = true;
    }
}
