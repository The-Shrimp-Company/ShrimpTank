using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class PlayerMovement : MonoBehaviour
{
    CharacterController CC;


    [Header("Movement Modifier")]
    public float Speed;


    private Vector3 move = new Vector3(0, 0, 0);
    private Vector3 startingPosition;



    void Start()
    {
        CC = GetComponent<CharacterController>();
        startingPosition = transform.position;
    }


    private void Update()
    {
        if (CC.enabled == false) CC.enabled = true;  // Teleporting requires the character controller to be disabled between updates

        if (UIManager.instance.GetScreen())  // Player cannot move while a menu is open
            move = Vector3.zero;

        if (transform.position.y > startingPosition.y)
        {
            move.y = -5;
        }

        CC.Move(transform.TransformVector(move.x, move.y, move.z) * Time.deltaTime);  // Move the player

        if (move != Vector3.zero) PlayerStats.stats.timeSpentMoving += Time.deltaTime;  // Update the TimeSpentMoving tracker

        move.y = 0;

        // Check in case the player leaves the map
        if (Vector3.Distance(startingPosition, transform.position) > 100)
        {
            Debug.LogWarning("Player has fallen out of map, teleporting back");
            CC.enabled = false;
            transform.position = startingPosition;
        }
    }

    

    public void OnMove(InputValue Move)
    {
        move.x = Move.Get<Vector2>().x;
        move.z = Move.Get<Vector2>().y;
        move *= Speed;
    }

    public void OnSlowMove(InputValue Move)
    {
        move.x = Move.Get<Vector2>().x;
        move.z = Move.Get<Vector2>().y;
        move /= 2;
        move *= Speed;
    }
}
