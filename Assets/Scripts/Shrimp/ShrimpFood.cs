using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShrimpFood : MonoBehaviour
{
    private TankController tank;

    public Item thisItem;

    [Header("Eating")]
    public int hungerFillAmount = 10;
    public float eatingTime = 2;
    [HideInInspector] public Shrimp shrimpEating; 

    [Header("Sinking")]
    [SerializeField] float sinkSpeed = 1;  // How quickly it sinks, 0 will float
    private float sinkTimer = 0;
    [SerializeField] LayerMask decorationLayer;

    [Header("Landing")]
    [HideInInspector] public bool settled = false;  // If it has landed
    private float surfacePosition;
    [HideInInspector] public float landingPosition;
    [SerializeField] Vector3 landingPositionOffset;  // How high off the ground it will sit

    [Header("Despawning")]
    [SerializeField] float despawnTime = 120;
    public float despawnTimer = 0;

    [Header("Particles")]
    public GameObject eatingParticles;
    public GameObject finishedParticles;


    public void CreateFood(TankController t, Item thisItem)
    {
        tank = t;
        tank.foodToAdd.Add(this);
        transform.parent = tank.foodParent;
        surfacePosition = transform.position.y;
        this.thisItem = thisItem;
        FindLandingPosition();
    }

    public void CreateFood(TankController t, FoodSaveData saveData)
    {
        tank = t;
        tank.foodToAdd.Add(this);
        transform.parent = tank.foodParent;
        surfacePosition = tank.GetRandomSurfacePosition().y;
        if (!saveData.settled)
        {
            FindLandingPosition();
        }
        else
        {
            settled = saveData.settled;
            landingPosition = transform.position.y;
        }
        despawnTimer = saveData.despawnTimer;
        thisItem = saveData.thisItem;
    }


    public void UpdateFood(float elapsedTime)
    {
        // Sinking
        if (!settled && landingPosition != 0)
        {
            sinkTimer += sinkSpeed * elapsedTime * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(surfacePosition, landingPosition, sinkTimer), transform.position.z);
            if (transform.position.y <= landingPosition)
            {
                settled = true;
                transform.position = new Vector3(transform.position.x, landingPosition, transform.position.z);
            }
        }

        // Despawning
        despawnTimer += elapsedTime;
        if (shrimpEating == null && despawnTimer >= despawnTime)
        {
            tank.foodToRemove.Add(this);
        }
    }

    private void FindLandingPosition()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 10, decorationLayer))
        {
            landingPosition = hit.point.y + landingPositionOffset.y;
            //transform.position = landingPosition;

            float dist = surfacePosition - landingPosition;
            sinkSpeed /= dist * 10;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal);
        }
    }
}