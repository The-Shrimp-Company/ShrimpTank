using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrimpBreeding : ShrimpActivity
{
    private float breedDistance = 0.0375f;
    private float waitDistance = 0.15f;
    private float breedTime = 10;

    // While waiting for instigator
    private float bobSpeed = 0.5f;
    private float bobMagnitude = 0.1f;

    public Shrimp otherShrimp;
    private ShrimpBreeding otherBreeding;  // The breeding script for the other shrimp
    private bool otherIsReady;  // If the other shimp has finished other tasks and is ready to breed

    public bool instigator;  // If this is the shrimp that started it, they will move to the other shrimp
    private bool female;  // Whether this is the female shrimp or the male shrimp
    private bool breeding;  // If they are close enough and have started breeding
    private ShrimpMovement movement = null;
    private GameObject particles;
    private bool debugBreeding = false;


    public override void CreateActivity()
    {
        activityName = "Breeding";
    }


    protected override void StartActivity()
    {
        if(otherShrimp == null)
        {
            EndActivity();
            return;
        }
        if (shrimp.stats.gender == false)  // Is female
            female = true;

        breeding = false;
        taskTime = 90;  // The task time will not properly start until they start breeding

        if (instigator)
        {
            movement = new ShrimpMovement();
            movement.SetDestination(shrimp.tank.tankGrid.GetClosestNode(otherShrimp.transform.position));
            movement.shrimp = shrimp;
            movement.endWhenDestinationReached = false;
            movement.taskTime = Mathf.Infinity;
            movement.Activity(0);
        }

        if (debugBreeding) Debug.Log("Breeding Start - " + shrimp.name + "-" + instigator + " - " + otherShrimp.name + "-" + !instigator);
        
        base.StartActivity();
    }


    protected override void UpdateActivity()
    {
        if(otherShrimp == null)
        {
            EndActivity();
            return;
        }
        if (!breeding && instigator)
        {
            float dist = Vector3.Distance(shrimp.transform.position, otherShrimp.transform.position);

            if (!otherIsReady)
            {
                if (dist > waitDistance)
                {
                    MoveToOther();  // Move over and wait for other
                }

                if (otherShrimp.shrimpActivities[0].GetType() == typeof(ShrimpBreeding))  // If the other shrimp's current task is breeding
                {
                    if (((ShrimpBreeding)otherShrimp.shrimpActivities[0]).otherShrimp == shrimp && ((ShrimpBreeding)otherShrimp.shrimpActivities[0]).instigator == false)
                    {
                        otherIsReady = true;
                        otherBreeding = (ShrimpBreeding)otherShrimp.shrimpActivities[0];
                        if (movement != null)
                        {
                            movement.SetDestination(shrimp.tank.tankGrid.GetClosestNode(otherShrimp.transform.position));  // Set the destination again incase they have moved
                            movement.SwitchToAdvanced();
                        }
                        if (debugBreeding) Debug.Log("Other is ready");
                    }
                }
            }

            else  // If the other shimp has finished other tasks and is ready
            {
                if (dist > breedDistance)
                {
                    MoveToOther();
                }

                else
                {
                    StartBreeding();
                    otherBreeding.StartBreeding();

                    if (debugBreeding) Debug.Log("Start Breeding");
                }
            }
        }

        else if (!breeding && !instigator)  // Other shimp is waiting for the instigator
        {
            shrimp.transform.position = shrimp.transform.position + shrimp.transform.up * Mathf.Sin(Time.time * bobSpeed) * (bobMagnitude / 5000);
        }

        else  // If they are breeding
        {
            // Whatever happens while they are breeding

            if (otherShrimp.transform.position - shrimp.transform.position != Vector3.zero)
                shrimp.agent.shrimpModel.rotation = Quaternion.RotateTowards(shrimp.agent.shrimpModel.rotation, Quaternion.LookRotation((otherShrimp.transform.position - shrimp.transform.position), Vector3.up), shrimp.agent.simpleTurnSpeed * elapsedTimeThisFrame);
        }
    }


    private void MoveToOther()
    {
        if (movement != null)
            movement.Activity(elapsedTimeThisFrame);
    }


    public override void EndActivity()
    {
        if (breeding)
        {
            if (female && otherShrimp != null) LayEggs();
            if (particles != null) Object.Destroy(particles);

            if (debugBreeding) Debug.Log("Finished Breeding");
        }

        shrimp.breedingTimer = ShrimpManager.instance.GetBreedingCooldown(shrimp.stats, shrimp.tank);   

        base.EndActivity();
    }


    public void StartBreeding()
    {
        if (instigator)
        {
            // Delete the movement class
            if (movement != null)
            {
                movement.EndActivity();
                movement = null;
            }

            // Spawn breeding particles
            particles = GameObject.Instantiate(shrimp.breedingHeartParticles, shrimp.transform.position, Quaternion.identity);
            particles.transform.parent = shrimp.transform;
            particles.transform.position = (shrimp.transform.position + otherShrimp.transform.position) / 2;
        }

        breeding = true;
        taskTime = breedTime;
        taskRemainingTime = taskTime;
    }


    private void LayEggs()
    {
        int children = Random.Range(ShrimpManager.instance.minChildrenToGiveBirthTo, ShrimpManager.instance.maxChildrenToGiveBirthTo + 1);

        for (int i = 0; i < children; i++)
        {
            GameObject newShrimp = GameObject.Instantiate(ShrimpManager.instance.shrimpPrefab, shrimp.tank.GetRandomTankPosition(), Quaternion.identity);
            Shrimp s = newShrimp.GetComponent<Shrimp>();

            s.stats = ShrimpManager.instance.CreateShrimpThroughBreeding(shrimp.stats, otherShrimp.stats);
            s.ChangeTank(shrimp.tank);
            newShrimp.name = s.stats.name;
            newShrimp.transform.parent = shrimp.tank.shrimpParent;
            newShrimp.transform.position = (shrimp.transform.position + otherShrimp.transform.position) / 2;  // Spawn inbetween the two shrimp
            s.ConstructShrimp();

            shrimp.tank.shrimpToAdd.Add(s);

            if (debugBreeding) Debug.Log(shrimp.name + " has had a shrimp");

            Email email = EmailTools.CreateEmail();
            email.title = "A new shrimp has been born";
            email.subjectLine = "Wow!";
            email.mainText = "The shrimp is in " + shrimp.tank.tankName + ", the parents are " + shrimp.stats.name + " and " + otherShrimp.stats.name;
            EmailManager.SendEmail(email);

            PlayerStats.stats.shrimpBorn++;
        }

        PlayerStats.stats.shrimpBred++;
    }
}