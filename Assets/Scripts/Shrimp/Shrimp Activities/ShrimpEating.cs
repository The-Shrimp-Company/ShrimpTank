using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrimpEating : ShrimpActivity
{
    private float eatDistance = 0.02f;

    public ShrimpFood food;
    private bool settled;

    private bool eating;
    private ShrimpMovement movement = null;
    private GameObject particles;
    private bool debugEating = false;


    public override void CreateActivity()
    {
        activityName = "Eating";
    }

    protected override void StartActivity()
    {
        if(food == null)
        {
            EndActivity();
            return;
        }

        eating = false;
        taskTime = 30;  // The task time will not properly start until they start breeding
        movement = new ShrimpMovement();
        movement.SetDestination(shrimp.tank.tankGrid.GetClosestNode(new Vector3(food.transform.position.x, food.landingPosition, food.transform.position.z)));
        movement.shrimp = shrimp;
        movement.Activity(0);
        
        base.StartActivity();
    }


    protected override void UpdateActivity()
    {
        if(food == null)
        {
            EndActivity();
            return;
        }

        if (!eating)
        {
            float dist = Vector3.Distance(shrimp.transform.position, food.transform.position);
            //Debug.Log("Eating - " + eating + " Settled - " + food.settled + " Dist - " + dist);
            if (!settled && food.settled)  // If the food is ready
            {
                settled = true;

                if (debugEating) Debug.Log("Food is ready");
            }

            if (dist > eatDistance)
            {
                MoveToFood();  // Move over and wait for food to land
            }
            else if(settled)  // If the shrimp is close enough and the food has landed
            {
                StartEating();
            }
        }

        else  // If they are eating
        {
            // Whatever happens while they are eating

            if (food.transform.position - shrimp.transform.position != Vector3.zero)
                shrimp.agent.shrimpModel.rotation = Quaternion.RotateTowards(shrimp.agent.shrimpModel.rotation, Quaternion.LookRotation((food.transform.position - shrimp.transform.position), Vector3.up), shrimp.agent.simpleTurnSpeed * elapsedTimeThisFrame);
        }
    }


    private void MoveToFood()
    {
        if (shrimp.agent.Status != AgentStatus.Finished)
            movement.Activity(elapsedTimeThisFrame);

        else
        {
            shrimp.transform.position = Vector3.MoveTowards(shrimp.transform.position, food.transform.position, shrimp.agent.speed * elapsedTimeThisFrame);
            shrimp.agent.shrimpModel.rotation = Quaternion.RotateTowards(shrimp.agent.shrimpModel.rotation, Quaternion.LookRotation((food.transform.position - shrimp.transform.position), Vector3.up), shrimp.agent.simpleTurnSpeed * elapsedTimeThisFrame);
        }
    }



    public void StartEating()
    {
        if (debugEating) Debug.Log("Start Eating");

        // Delete the movement class
        movement.EndActivity();
        movement = null;

        // Spawn eating particles
        if (food.eatingParticles != null)
        {
            particles = GameObject.Instantiate(food.eatingParticles, food.transform.position, food.transform.rotation);
            particles.transform.parent = shrimp.transform;
        }

        eating = true;
        taskTime = food.eatingTime;
        taskRemainingTime = taskTime;
    }



    public override void EndActivity()
    {
        if (eating)
        {
            if (particles != null) Object.Destroy(particles);
            if (food.finishedParticles != null)
            {
                particles = GameObject.Instantiate(food.finishedParticles, food.transform.position, food.transform.rotation);
                particles.transform.parent = shrimp.transform;
            }

            //shrimp.stats.hunger = Mathf.Clamp(shrimp.stats.hunger + food.hungerFillAmount, 0, 100);
            shrimp.stats.hunger -= food.hungerFillAmount;
            if(shrimp.stats.hunger < 0) shrimp.stats.hunger = 0;
            shrimp.tank.foodToRemove.Add(food);
            if (debugEating) Debug.Log("Finished Eating");
        }

        base.EndActivity();
    }
}