using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IllnessController))]
public class Shrimp : MonoBehaviour
{
    [Header("Shrimp")]
    public ShrimpStats stats;
    public TankController tank;

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

    [Header("Illness")]
    public GameObject symptomBubbleParticles;
    [HideInInspector] public IllnessController illnessCont;

    [Header("Breeding")]
    public GameObject breedingHeartParticles;

    [Header("Effects")]
    public Transform particleParent;

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
        agent.shrimpModel.localScale = ShrimpManager.instance.GetShrimpSize(TimeManager.instance.GetShrimpAge(stats.birthTime), stats.geneticSize);
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
            if (shrimpActivities.Count > 0 && shrimpActivities[0] != null)
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


        // Update illness
        if (illnessCont != null)
            illnessCont.UpdateIllness(elapsedTime);


        // Reduce hunger
        stats.hunger = Mathf.Clamp(stats.hunger - ((hungerLossSpeed * ((stats.metabolism / 50) + 1)) * elapsedTime), 0, 100);
        

        // Try to add an activity if we don't have enough
        TryAddActivity();  
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


    public void EndActivity()
    {
        shrimpActivities.RemoveAt(0);

        TryAddActivity();
    }


    private void Molt()
    {
        int age = TimeManager.instance.GetShrimpAge(stats.birthTime);

        stats.moltHistory++;
        agent.shrimpModel.localScale = ShrimpManager.instance.GetShrimpSize(age, stats.geneticSize);

        if (age >= ShrimpManager.instance.GetAdultAge())  // If the shrimp is considered an adult
            stats.canBreed = true;  // The shrimp can breed

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

        // Clear all activities
        if (shrimpActivities.Count != 0)
            shrimpActivities[0].EndActivity();
        shrimpActivities.Clear();

        TryAddActivity(3);  // Try to add up to 3 activities
    }


    public void KillShrimp()
    {
        illnessCont.RemoveShrimp();
        tank.shrimpToRemove.Add(this);

        if (focusingShrimp)
        {
            GetComponentInChildren<ShrimpCam>().Deactivate();
            Camera.main.transform.position = tank.GetCam().transform.position;
            Camera.main.transform.rotation = tank.GetCam().transform.rotation;
            UIManager.instance.CloseScreen();
        }


        // Spawn dead body




        Email email = EmailTools.CreateEmail();
        email.title = stats.name + " has died";
        email.subjectLine = "Please check the conditions of the tank";
        email.mainText = stats.name + " was in " + tank.tankName;
        EmailManager.SendEmail(email);

        PlayerStats.stats.shrimpDeaths++;

        Destroy(gameObject);
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
        agent.shrimpModel.gameObject.SetActive(true);
        particleParent.gameObject.SetActive(true);
    }

    public void SwitchToMidLOD()  // Focused on tank
    {
        agent.shrimpModel.gameObject.SetActive(true);
        particleParent.gameObject.SetActive(true);
    }

    public void SwitchToLowLOD()  // Near tank
    {
        agent.shrimpModel.gameObject.SetActive(true);
        particleParent.gameObject.SetActive(false);
    }

    public void SwitchToSuperLowLOD()  // Far from tank
    {
        agent.shrimpModel.gameObject.SetActive(false);
        particleParent.gameObject.SetActive(false);
    }
}