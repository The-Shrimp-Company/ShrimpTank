using System.Collections.Generic;
using UnityEngine;

public static class ShrimpActivityManager
{
    public static ShrimpActivity GetRandomActivity(Shrimp shrimp)
    {
        int i = Random.Range(0, 4);
        if (i == 0) return new ShrimpMovement();
        if (i == 1) return new ShrimpSleeping();
        if (i == 2) return new ShrimpBreeding();
        if (i == 3) return new ShrimpEating();
        return (new ShrimpActivity());
    }


    public static void AddActivity(Shrimp shrimp, ShrimpActivity activity = null)
    {
        if (shrimp == null)
            return;

        if (activity == null)  // If they did not request a specific activity
            activity = GetRandomActivity(shrimp);  // Pick a random one


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
                AddActivity(shrimp, GetRandomActivity(shrimp));
                return;
            }

            // If there are too many shrimp in the tank
            if (shrimp.tank.shrimpInTank.Count > ShrimpManager.instance.shrimpInTankBreedingLimit)  
            {
                AddActivity(shrimp, GetRandomActivity(shrimp));
                return;
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
                AddActivity(shrimp, GetRandomActivity(shrimp));
                return;
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
            // Check if there is food in the tank
            if (shrimp.tank.foodInTank.Count == 0)
            {
                AddActivity(shrimp, GetRandomActivity(shrimp));
                shrimp.stats.hunger += 1;
                return;  // Cancel this and find a different activity
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
                AddActivity(shrimp, GetRandomActivity(shrimp));
                shrimp.stats.hunger += 1;
                return;  // Cancel this and find a different activity
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
    }
}
