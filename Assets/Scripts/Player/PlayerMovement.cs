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


    private Vector2 move = new Vector2(0, 0);
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
            move = Vector2.zero;


        CC.Move(transform.TransformVector(move.x, 0, move.y) * Time.deltaTime);  // Move the player


        if (move != Vector2.zero) PlayerStats.stats.timeSpentMoving += Time.deltaTime;  // Update the TimeSpentMoving tracker



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
        move = Move.Get<Vector2>();
        move *= Speed;
    }

    public void OnSlowMove(InputValue Move)
    {
        move = Move.Get<Vector2>() / 2;
        move *= Speed;
    }
}
