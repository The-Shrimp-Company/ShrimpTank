using System.Collections.Generic;
using UnityEngine;

public static class ShrimpActivityManager
{


    public static void AddActivity(Shrimp shrimp, ShrimpActivity activity = null)
    {
        if (shrimp == null)
        {
            return;
        }

        List<ShrimpActivity> options = new List<ShrimpActivity>();

        if (activity == null)
        {
            options.Add(new ShrimpMovement());
            options.Add(new ShrimpSleeping());
            options.Add(new ShrimpBreeding());
            options.Add(new ShrimpEating());
        }
        else
        {
            options.Add(activity);
        }
        while (true)
        {
            activity = options[Random.Range(0, options.Count)];
            options.Remove(activity);


            if (activity is ShrimpMovement)
            {
                ShrimpMovement movement = (ShrimpMovement)activity;  // Casts the activity to the derrived shrimpMovement activity
                movement.randomDestination = true;
            }


            else if (activity is ShrimpSleeping)
            {
                ShrimpSleeping sleeping = (ShrimpSleeping)activity;

                sleeping.taskTime = Random.Range(4, 8);
            }


            else if (activity is ShrimpBreeding)
            {
                // If the tank cooldown has not ended
                if (!shrimp.tank.shrimpCanBreed)
                {
                    continue;
                }

                // If there are too many shrimp in the tank
                if (shrimp.tank.shrimpInTank.Count > ShrimpManager.instance.shrimpInTankBreedingLimit)
                {
                    continue;
                }

                // Find other shrimp
                List<Shrimp> validShrimp = new List<Shrimp>();
                foreach (Shrimp s in shrimp.tank.shrimpInTank)
                {
                    if (s.stats.gender != shrimp.stats.gender)  // Get all shrimp of the opposite gender, also excludes this shrimp
                    {
                        // Other logic for who can breed here

                        if (s.stats.canBreed &&
                            shrimp.stats.canBreed)
                        {
                            validShrimp.Add(s);
                        }
                    }
                }

                if (validShrimp.Count == 0)  // If there are no valid shrimp
                {
                    continue;
                }

                // Pick other shrimp
                int i = Random.Range(0, validShrimp.Count);
                Shrimp otherShrimp = validShrimp[i];

                // Setup other shrimp activity
                ShrimpBreeding otherBreeding = new ShrimpBreeding();
                otherBreeding.instigator = false;
                otherBreeding.shrimp = otherShrimp;
                otherBreeding.otherShrimp = shrimp;
                otherShrimp.shrimpActivities.Add(otherBreeding);

                // Setup this shrimp's activity
                ShrimpBreeding breeding = (ShrimpBreeding)activity;
                breeding.instigator = true;
                breeding.otherShrimp = otherShrimp;

                shrimp.tank.shrimpCanBreed = false;
                shrimp.tank.breedingCooldownTimer = shrimp.tank.breedingCooldown;
                shrimp.stats.canBreed = false;
                otherShrimp.stats.canBreed = false;
                shrimp.breedingTimer = 300;
                otherShrimp.breedingTimer = 300;
            }


            else if (activity is ShrimpEating)
            {
                // Check if there is food in the tank or if the shrimp is "full"
                // If I ever add shrimp happiness, the shrimp should try and eat to prevent hunger.
                if (shrimp.tank.foodInTank.Count == 0 || shrimp.stats.hunger == 0)
                {
                    shrimp.stats.hunger += 1;
                    continue;
                }

                // Create a list of all of the food in the tank that is available to be eaten right now
                List<ShrimpFood> possibleFood = new List<ShrimpFood>();
                foreach (ShrimpFood f in shrimp.tank.foodInTank)
                {
                    bool foodValid = true;



                    if (f.shrimpEating != null)  // If a shrimp is already eating it
                        foodValid = false;

                    if (foodValid)
                        possibleFood.Add(f);
                }

                // Check if there is valid food in the tank
                if (possibleFood.Count == 0)
                {
                    shrimp.stats.hunger += 1;
                    continue;
                }

                // Pick a random piece of food
                int i = Random.Range(0, possibleFood.Count);
                ShrimpFood food = possibleFood[i];

                food.shrimpEating = shrimp;

                ShrimpEating eating = (ShrimpEating)activity;
                eating.food = food;
            }


            else
            {
                Debug.Log(activity + " Activity logic is missing");
                return;
            }

            activity.shrimp = shrimp;
            activity.CreateActivity();
            shrimp.shrimpActivities.Add(activity);
            return;
        }
        
    }
}
