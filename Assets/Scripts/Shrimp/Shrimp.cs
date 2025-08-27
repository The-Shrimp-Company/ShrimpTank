using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(IllnessController))]
public class Shrimp : MonoBehaviour
{
    [Header("Shrimp")]
    public ShrimpStats stats;
    public TankController tank;
    public bool finishedStarting = false;

    [Header("Activities")]
    public List<ShrimpActivity> shrimpActivities = new List<ShrimpActivity>();
    private int minActivitiesInQueue = 2;

    [Header("Pathfinding")]
    public ShrimpAgent agent;

    [Header("Model")]
    [HideInInspector] public Legs shrimpLegs;

    [Header("Hunger")]
    [SerializeField] private float hungerLossSpeed = 0.1f;
    public Transform camDock;

    [Header("Molt")]
    private float moltTimer;
    private float moltSpeed;

    [Header("Breeding")]
    [HideInInspector] public float breedingTimer;

    [Header("Illness")]
    public GameObject symptomBubbleParticles;
    [HideInInspector] public IllnessController illnessCont;

    [Header("Breeding")]
    public GameObject breedingHeartParticles;

    [Header("Effects")]
    public Transform particleParent;
    public Ease shrimpAppearEase = Ease.OutBack;

    [Header("Misc")]
    public float currentValue;
    [HideInInspector] public bool shrimpNameChanged;
    [HideInInspector] public bool loadedShrimp;  // Whether the shrimp has been loaded from a save file
    private bool focusingShrimp;



    public void Start()
    {
        agent.tankGrid = tank.tankGrid;
        illnessCont = GetComponent<IllnessController>();

        TryAddActivity(3);  // Try to add up to 3 activities

        moltSpeed = ShrimpManager.instance.GetMoltTime(TimeManager.instance.GetShrimpAge(stats.birthTime));
        breedingTimer = ShrimpManager.instance.GetBreedingCooldown(stats, tank);
        stats.canBreed = false;

        agent.shrimpModel.localScale = Vector2.zero;
        agent.shrimpModel.DOScale(ShrimpManager.instance.GetShrimpSize(TimeManager.instance.GetShrimpAge(stats.birthTime), stats.geneticSize), 0.5f).SetEase(shrimpAppearEase);  // Make the shrimp smoothly appear
        finishedStarting = true;
    }


    private void Update()
    {
        if (focusingShrimp) PlayerStats.stats.timeSpentFocusingShrimp += Time.deltaTime;
    }


    public void ConstructShrimp()
    {
        GameObject newShrimp = Instantiate<GameObject>(GeneManager.instance.GetTraitSO(stats.legs.activeGene.ID).part, agent.shrimpModel);
        shrimpLegs = newShrimp.GetComponent<Legs>();
        shrimpLegs.Construct(stats);
    }


    public void UpdateShrimp(float elapsedTime)
    {
        // Activities      
        float timeRemaining = elapsedTime;  // The time since the last update
        do
        {
            if (shrimpActivities.Count > 0 && shrimpActivities[0] != null && shrimpActivities[0].shrimp != null)
            {
                timeRemaining = shrimpActivities[0].Activity(timeRemaining);
                if (timeRemaining != 0)
                {
                    EndActivity();
                }
            }
            else
            {
                timeRemaining = 0;
            }
        }
        while (timeRemaining > 0);


        // Molting
        moltTimer += elapsedTime;
        while (moltTimer >= moltSpeed && moltSpeed != 0)
        {
            moltTimer -= moltSpeed;
            Molt();
        }


        // Breeding
        if (!stats.canBreed && ShrimpManager.instance.IsShrimpAdult(stats))
        {
            breedingTimer -= elapsedTime;
            if (breedingTimer <= 0)
            {
                stats.canBreed = true;
            }
        }


        // Update illness
        if (illnessCont != null)
            illnessCont.UpdateIllness(elapsedTime);


        // Reduce hunger
        //stats.hunger = Mathf.Clamp(stats.hunger - ((hungerLossSpeed * ((stats.metabolism / 50) + 1)) * elapsedTime), 0, 100);
        

        

        // Try to add an activity if we don't have enough
        TryAddActivity();

        // Check if the shrimp should die due to not having it's needs met
        if (Mathf.Abs(tank.waterAmmonium - stats.ammoniaPreference) > 10 &&
            Mathf.Abs(tank.waterSalt - stats.salineLevel) > 10 &&
            Mathf.Abs(tank.waterPh - stats.PhPreference) > 2 &&
            Mathf.Abs(tank.waterTemperature - stats.temperaturePreference) > 10)
        {
            KillShrimp();
        }
        // Check if the shrimp should die due to hunger
        else if(stats.hunger >= 3)
        {
            KillShrimp();
        }
        
    }


    public void TryAddActivity(int activities = 1)
    {
        if (activities <= 0)  // If we don't need any more activities
            return;

        if (shrimpActivities.Count > minActivitiesInQueue)  // If the shrimp doesn't have enough activities
            return;

        ShrimpActivityManager.AddActivity(this, null);  // Add an activity

        activities--;
        TryAddActivity(activities);  // Recursively add activities
    }

    public void ClearActivities()
    {
        for (int i = shrimpActivities.Count - 1; i >= 0; i--)
        {
            if (shrimpActivities[i] == null) break;

            shrimpActivities[i].EndActivity();
            shrimpActivities.RemoveAt(i);
        }
        shrimpActivities.Clear();
    }

    public void EndActivity()
    {
        shrimpActivities.RemoveAt(0);

        TryAddActivity();
    }


    private void Molt()
    {
        if (agent.shrimpModel == null) return;

        int age = TimeManager.instance.GetShrimpAge(stats.birthTime);

        stats.moltHistory++;
        agent.shrimpModel.localScale = ShrimpManager.instance.GetShrimpSize(age, stats.geneticSize);

        if (ShrimpManager.instance.CheckForMoltFail(age, stats, tank))
            KillShrimp();  // Molt has failed, the shrimp will now die

        moltSpeed = ShrimpManager.instance.GetMoltTime(age);
    }


    public void ChangeTank(TankController t)
    {
        if (tank != null)
            illnessCont.MoveShrimp(tank, t);

        tank = t;
        agent.tankGrid = tank.tankGrid;

        ClearActivities();  // Remove old activities
        TryAddActivity(3);  // Try to add up to 3 activities

        breedingTimer = ShrimpManager.instance.GetBreedingCooldown(stats, tank);  // Reset breeding cooldown
        stats.canBreed = false;
    }


    public void KillShrimp()
    {
        illnessCont.RemoveShrimp();
        tank.shrimpToRemove.Add(this);

        if (focusingShrimp)
        {
            GetComponentInChildren<ShrimpCam>().Deactivate();
            Camera.main.transform.SetPositionAndRotation(tank.GetCam().transform.position, tank.GetCam().transform.rotation);
            UIManager.instance.CloseScreen();
        }


        // Spawn dead body




        Email email = EmailTools.CreateEmail();
        email.title = "YourStore@notifSystem.store";
        email.subjectLine = stats.name + " has died";
        email.mainText = stats.name + " was in " + tank.tankName;
        EmailManager.SendEmail(email);

        PlayerStats.stats.shrimpDeaths++;

        //Destroy(gameObject);
    }


    public void SellShrimp()
    {
        illnessCont.RemoveShrimp();
        CustomerManager.Instance.PurchaseShrimp(this, currentValue);
    }

    public void HardSellShrimp()
    {
        illnessCont.RemoveShrimp();
        CustomerManager.Instance.HardPurchaseShrimp(this, currentValue);
    }

    public float GetValue()
    {
        return (EconomyManager.instance.GetShrimpValue(stats));
    }


    public void FocusShrimp()
    {
        focusingShrimp = true;
        tank.SwitchLODLevel(LODLevel.High);
        shrimpNameChanged = false;
    }


    public void StopFocusingShrimp()
    {
        focusingShrimp = false;
        tank.SwitchLODLevel(LODLevel.Mid);

        if (shrimpNameChanged)
        {
            PlayerStats.stats.shrimpNamed++;
            shrimpNameChanged = false;
        }
    }





    public void SwitchLODLevel(LODLevel level)  // Focused on shrimp
    {
        if (particleParent == null) return;

        switch(level)
        {
            case LODLevel.High:
            {
                SwitchToHighLOD();
                break;
            }
            case LODLevel.Mid:
            {
                SwitchToMidLOD();
                break;
            }
            case LODLevel.Low:
            {
                SwitchToLowLOD();
                break;
            }
            case LODLevel.SuperLow:
            {
                SwitchToSuperLowLOD();
                break;
            }
        }
    }

    public void SwitchToHighLOD()  // Focused on Shrimp
    {
        shrimpLegs.ForceLod(0);
        particleParent.gameObject.SetActive(true);
    }

    public void SwitchToMidLOD()  // Focused on tank
    {
        shrimpLegs.ForceLod(1);
        particleParent.gameObject.SetActive(true);
    }

    public void SwitchToLowLOD()  // Near tank
    {
        shrimpLegs.ForceLod(2);
        particleParent.gameObject.SetActive(false);
    }

    public void SwitchToSuperLowLOD()  // Far from tank
    {
        shrimpLegs.ForceLod(3);
        particleParent.gameObject.SetActive(false);
    }
}