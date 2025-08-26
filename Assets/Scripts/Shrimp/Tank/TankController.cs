using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

[RequireComponent(typeof(TankUpgradeController))]
public class TankController : MonoBehaviour
{
    public delegate void shrimpChanged(Shrimp shrimp);
    public event shrimpChanged OnShrimpRemoved;
    public event shrimpChanged OnShrimpAdded;

    public enum AlarmTypes
    {
        Temp,
        Quality,
        Food,
        Salt,
        Hnc,
        ph
    }


    [Header("Shrimp")]
    public List<Shrimp> shrimpInTank = new List<Shrimp>();
    [HideInInspector] public List<Shrimp> shrimpToAdd = new List<Shrimp>();
    [HideInInspector] public List<Shrimp> shrimpToRemove = new List<Shrimp>();
    public Transform shrimpParent;

    [Header("Tank")]
    public string tankName = null;
    public string tankId = "";

    [Header("Updates")]
    private float updateTimer;
    public float updateTime;  // The time between each shrimp update, 0 will be every frame

    [Header("Decorations")]
    public List<GameObject> decorationsInTank = new List<GameObject>();
    public GameObject[] decorationCamDock;
    public Transform decorationParent;

    [Header("Food")]
    public List<ShrimpFood> foodInTank = new List<ShrimpFood>();
    public int FoodStore = 0;
    [HideInInspector] public List<ShrimpFood> foodToAdd = new List<ShrimpFood>();
    [HideInInspector] public List<ShrimpFood> foodToRemove = new List<ShrimpFood>();
    public Transform foodParent;

    [Header("Water")]
    public Transform waterLevel;
    public GameObject waterObject;

    public float waterQuality = 100;
    [SerializeField] float waterQualityDecreaseSpeed = 5;

    public float waterTemperature = 50;
    private float tempuratureRisingTarget;
    private float tempuratureRisingTimer;
    [SerializeField] float tempuratureChangeSpeed = 5;
    [SerializeField][Range(0,50)] float naturalTempuratureVariation = 25;

    [HideInInspector] public float waterSalt = 50;
    [HideInInspector] public float waterAmmonium = 50;
    [HideInInspector] public float waterPh = 7;

    [HideInInspector] public float idealTemp = 0;
    [HideInInspector] public float idealSalt = 0;
    [HideInInspector] public float idealHnc = 0;
    [HideInInspector] public float idealPh = 0;


    [Header("Upgrades")]
    [HideInInspector] public TankUpgradeController upgradeController;
    [HideInInspector] public UpgradeState upgradeState = new UpgradeState() { LightsOn = true };
    [SerializeField] private List<Light> lights;

    [Header("Sale Tank")]
    [SerializeField] private GameObject sign;
    public bool destinationTank { get; private set; } = false;
    [SerializeField] private GameObject SaleSign;
    public bool openTank { get; private set; } = false;
    [SerializeField] private TextMeshProUGUI label;
    public float openTankPrice = 5;
    [SerializeField] private TextMeshProUGUI openTankLabel;

    [Header("Pathfinding")]
    public TankGrid tankGrid;  // The grid used for pathfinding

    [Header("Tank Focus")]
    private bool focusingTank;
    [SerializeField] private GameObject camDock;
    [SerializeField] private GameObject tankViewPrefab;
    [HideInInspector] public TankViewScript tankViewScript;
    [HideInInspector] public TankDecorateViewScript tankDecorateViewScript;
    [HideInInspector] public bool tankNameChanged;

    [Header("Capacity")]
    public int roughShrimpCapacity;
    [SerializeField] AnimationCurve chanceToKillAShrimpOverCapacity;
    [SerializeField] float capacityCheckTime = 0.5f;
    private float capacityCheckTimer;

    [Header("Illness")]
    [Range(0, 100)] public float illnessSpreadRate = 30;
    [HideInInspector] public Dictionary<IllnessSO, int> currentIllness = new Dictionary<IllnessSO, int>();

    [Header("Breeding")]
    public float breedingCooldown = 90;
    [HideInInspector] public float breedingCooldownTimer;
    [HideInInspector] public bool shrimpCanBreed;

    [Header("Alarms")]
    public List<string> AlarmIds = new List<string>();

    [Header("Optimisation")]
    private LODLevel currentLODLevel;
    [SerializeField] float distanceCheckTime = 1;
    private float distanceCheckTimer;
    public Transform particleParent;
    private Transform player;

    [Header("Tooltip")]
    private ToolTip tooltip;

    [Header("Debugging")]
    [SerializeField] bool autoSpawnTestShrimp;
    [SerializeField] float autoSpawnFoodTime;
    private float autoSpawnFoodTimer;
    [SerializeField] GameObject autoSpawnFoodPrefab;


    void Start()
    {
        if (tankGrid == null) Debug.LogError("Pathfinding grid is missing");
        if (shrimpParent == null) Debug.LogError("Shrimp Parent is missing");

        if (string.IsNullOrEmpty(tankName)) tankName = "Tank";

        upgradeController = GetComponent<TankUpgradeController>();
        tooltip = GetComponent<ToolTip>();

        sign.SetActive(destinationTank);

        breedingCooldownTimer = breedingCooldown;
        shrimpCanBreed = false;


        if (autoSpawnTestShrimp)
        {
            for (int i = 0; i < 10; i++)
            {
                SpawnShrimp();
            }
        }

        SwitchLODLevel(LODLevel.Low);
    }

    void Update()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer >= updateTime)
        {
            AddToTank();
            RemoveFromTank();
            UpdateTank();

            updateTimer = 0;
        }


        foreach (Light light in lights) light.enabled = upgradeState.LightsOn;

        // LOD Distance
        distanceCheckTimer += Time.deltaTime;
        if (distanceCheckTimer >= distanceCheckTime)
        {
            CheckLODDistance();
            distanceCheckTimer = 0;
        }


        // Shrimp Capacity
        capacityCheckTimer += Time.deltaTime;
        if (capacityCheckTimer >= capacityCheckTime)
        {
            CheckShrimpCapacity();
            capacityCheckTimer = 0;
        }


        // Breeding
        if (!shrimpCanBreed)
        {
            breedingCooldownTimer -= Time.deltaTime;
            if (breedingCooldownTimer <= 0)
            {
                breedingCooldownTimer = breedingCooldown;
                shrimpCanBreed = true;
            }
        }

        

        // Food Spawning
        autoSpawnFoodTimer += Time.deltaTime;
        if (FoodStore > 0)
        {
            if (autoSpawnFoodTimer >= autoSpawnFoodTime && autoSpawnFoodTime != 0)
            {
                FoodStore -= 1;
                GameObject newFood = GameObject.Instantiate(autoSpawnFoodPrefab, GetRandomSurfacePosition(), Quaternion.identity);
                //newFood.GetComponent<ShrimpFood>().CreateFood(this);

                autoSpawnFoodTimer = 0;
            }
        }

        label.text = tankName;
        if (tooltip) tooltip.toolTip = tankName;

        if (focusingTank) PlayerStats.stats.timeSpentFocusingTank += Time.deltaTime;
    }


    private void AddToTank()
    {
        if (foodToAdd.Count > 0)  // Add food to the tank
        {
            for (int i = foodToAdd.Count - 1; i >= 0; i--)
            {
                foodInTank.Add(foodToAdd[i]);
                foodToAdd.RemoveAt(i);
            }
        }

        if (shrimpToAdd.Count > 0)  // Add shrimp to the tank
        {
            for (int i = shrimpToAdd.Count - 1; i >= 0; i--)
            {
                shrimpInTank.Add(shrimpToAdd[i]);
                if(OnShrimpAdded != null)
                {
                    OnShrimpAdded(shrimpToAdd[i]);
                }
                ShrimpManager.instance.allShrimp.Add(shrimpToAdd[i]);
                shrimpToAdd[i].SwitchLODLevel(currentLODLevel);
                shrimpToAdd.RemoveAt(i);

            }
        }
    }

    private void RemoveFromTank()
    {
        if (foodToRemove.Count > 0)  // Remove food from the tank
        {
            for (int i = foodToRemove.Count - 1; i >= 0; i--)
            {
                if (foodInTank.Contains(foodToRemove[i]))
                {
                    foodInTank.Remove(foodToRemove[i]);

                    Destroy(foodToRemove[i].gameObject);
                }

                foodToRemove.RemoveAt(i);
            }
        }

        if (shrimpToRemove.Count > 0)  // Remove shrimp from the tank
        {
            for (int i = shrimpToRemove.Count - 1; i >= 0; i--)
            {
                if (shrimpInTank.Contains(shrimpToRemove[i]))
                {
                    if(OnShrimpRemoved != null)
                    {
                        OnShrimpRemoved(shrimpToRemove[i]);
                    }
                    shrimpInTank.Remove(shrimpToRemove[i]);
                    ShrimpManager.instance.RemoveShrimpFromStore(shrimpToRemove[i]);
                }
                Destroy(shrimpToRemove[i].gameObject);
                shrimpToRemove.RemoveAt(i);
            }
        }
    }

    private void UpdateTank()
    {
        foreach (ShrimpFood food in foodInTank)  // Update the food in the tank
        {
            food.UpdateFood(updateTimer);
        }

        foreach (Shrimp shrimp in shrimpInTank)  // Update the shrimp in the tank
        {
            if(shrimp.finishedStarting) shrimp.UpdateShrimp(updateTimer);
        }



        // Water Quality
        if (!upgradeController.CheckForUpgrade(UpgradeTypes.Filter) ||
            upgradeController.GetUpgrade(UpgradeTypes.Filter).upgrade.filterCapacity < shrimpInTank.Count)
        {
            waterQuality = Mathf.Clamp(waterQuality - (((waterQualityDecreaseSpeed * ((shrimpInTank.Count + 1) / 2)) / 50) * updateTimer), 0, 100);
        }


        // Water Temperature
        waterTemperature -= 0.01f * updateTimer;

        waterSalt -= 0.01f * updateTimer * shrimpInTank.Count;
        
        waterAmmonium -= 0.01f * updateTimer * shrimpInTank.Count * UnityEngine.Random.value;



        upgradeController.UpdateUpgrades(updateTimer);

        

        idealTemp = 0;
        foreach(Shrimp shrimp in shrimpInTank)
        {
            idealTemp += shrimp.stats.temperaturePreference;
        }
        idealTemp /= shrimpInTank.Count;

        idealHnc = 0;
        foreach(Shrimp shrimp in shrimpInTank)
        {
            idealHnc += shrimp.stats.ammoniaPreference;
        }
        idealHnc /= shrimpInTank.Count;

        idealPh = 0;
        foreach(Shrimp shrimp in shrimpInTank)
        {
            idealPh += shrimp.stats.PhPreference;
        }
        idealPh /= shrimpInTank.Count;

        idealSalt = 0;
        foreach(Shrimp shrimp in shrimpInTank)
        {
            idealSalt += shrimp.stats.salineLevel;
        }
        idealSalt /= shrimpInTank.Count;

        // Sending stat alarms
        CheckStatAlarm(waterTemperature, idealTemp, 10, "Your tank is the wrong temperature", AlarmTypes.Temp);
        CheckStatAlarm(waterSalt, idealSalt, 10, "Your tank has the wrong salt level", AlarmTypes.Salt);
        CheckStatAlarm(waterAmmonium, idealHnc, 10, "Your tank has the wrong Ammonium Nitrate level", AlarmTypes.Hnc);
        CheckStatAlarm(waterPh, idealPh, 2, "Your tank has the wrong pH level", AlarmTypes.ph);
    }

    private void CheckStatAlarm(float currentValue, float targetValue, float maximumDifference, string AlarmMessage, AlarmTypes alarmType)
    {
        if (Mathf.Abs(currentValue - targetValue) >= maximumDifference)
        {
            Email email = CreateOrFindAlarm(alarmType);
            email.subjectLine = alarmType.ToString() + " warning in tank " + tankName;
            email.mainText = AlarmMessage;
        }
        else
        {
            Email email = FindAlarm(alarmType);
            if (email != null)
            {
                AlarmIds.Remove(email.ID);
                EmailManager.RemoveEmail(email);
            }
        }
    }

    private Email CreateOrFindAlarm(AlarmTypes type)
    {
        Email email = null;
        if(AlarmIds.Count == 0)
        {
            email = CreateAlarm(type);
        }
        else
        {
            email = FindAlarm(type);
            if (email == null)
            {
                email = CreateAlarm(type);
            }
        }
        return email;
    }

    private Email CreateAlarm(AlarmTypes type)
    {
        Email email = EmailTools.CreateEmail();
        AlarmIds.Add(email.ID);
        email.tag = Email.EmailTags.Alarms;
        email.title = type.ToString();
        email.subjectLine = type.ToString() + " warning in tank: " + tankName;
        email.CreateEmailButton("Go to tank")
            .SetFunc(EmailFunctions.FunctionIndexes.FocusTargetTank, tankId);
        EmailManager.SendEmail(email);
        return email;
    }

    private Email FindAlarm(AlarmTypes type)
    {
        Email[] emails = EmailManager.instance.emails.Where(x => x.tag == Email.EmailTags.Alarms && AlarmIds.Contains(x.ID)).ToArray();
        if (emails.Length >= 1) return Array.Find(emails, (x) => { return x.title == type.ToString(); });
        else return null;
    }

    public void ToggleDestinationTank()
    {
        destinationTank = !destinationTank;
        sign.SetActive(destinationTank);
    }


    public void SetTankPrice(float price)
    {
        openTankPrice = price;
        openTankLabel.text = "All Shrimp\n" + "£" + price.ToString();
    }


    public void toggleTankOpen()
    {
        if (CustomerManager.Instance == null) return;
        openTank = !openTank;
        if (openTank) CustomerManager.Instance.openTanks.Add(this);
        else CustomerManager.Instance.openTanks.Remove(this);
        SaleSign.SetActive(openTank);
        openTankLabel.text = "All Shrimp\n" + "£" + openTankPrice.ToString();
    }


    public void SpawnShrimp(TraitSet set = TraitSet.None)  // No trait set will spawn a random shrimp
    {
        GameObject newShrimp = Instantiate(ShrimpManager.instance.shrimpPrefab, GetRandomTankPosition(), Quaternion.identity);
        Shrimp s = newShrimp.GetComponent<Shrimp>();

        if (set == TraitSet.None) s.stats = ShrimpManager.instance.CreateRandomShrimp(false);
        else s.stats = ShrimpManager.instance.CreatePureBreedShrimp(set, false);
        newShrimp.name = s.stats.name;
        newShrimp.transform.parent = shrimpParent;
        newShrimp.transform.position = GetRandomTankPosition();
        s.ConstructShrimp();
        s.ChangeTank(this);

        ShrimpManager.instance.AddShrimpToStore(s);
        shrimpToAdd.Add(s);

        CheckMostShrimpInTank();
    }


    public void SpawnShrimp(ShrimpStats s, bool gameLoading = false)
    {
        upgradeController = GetComponent<TankUpgradeController>();
        
        if (upgradeController.CheckForUpgrade(UpgradeTypes.Decorations))
        {
            Decorations d = (Decorations) upgradeController.GetUpgrade(UpgradeTypes.Decorations);
            s = d.ApplyStatModifiers(s);
        }

        GameObject newShrimp = Instantiate(ShrimpManager.instance.shrimpPrefab, GetRandomTankPosition(), Quaternion.identity);
        Shrimp shrimp = newShrimp.GetComponent<Shrimp>();
        shrimp.stats = s;

        shrimp.loadedShrimp = gameLoading;
        newShrimp.name = shrimp.stats.name;
        newShrimp.transform.parent = shrimpParent;
        newShrimp.transform.position = GetRandomTankPosition();
        shrimp.ConstructShrimp();
        shrimp.ChangeTank(this);

        if (gameLoading)
            newShrimp.GetComponent<IllnessController>().LoadIllnesses(s);

        ShrimpManager.instance.AddShrimpToStore(shrimp);
        shrimpToAdd.Add(shrimp);

        CheckMostShrimpInTank();
    }
 
    public void MoveShrimp(Shrimp shrimp)
    {
        shrimp.tank.shrimpToRemove.Add(shrimp);
        Shrimp newShrimp = Instantiate(ShrimpManager.instance.shrimpPrefab).GetComponent<Shrimp>();
        newShrimp.stats = shrimp.stats;
        newShrimp.gameObject.name = shrimp.stats.name;
        newShrimp.ConstructShrimp();
        newShrimp.transform.parent = shrimpParent;
        newShrimp.transform.position = GetRandomTankPosition();
        shrimpToAdd.Add(newShrimp);
        newShrimp.ChangeTank(this);

        CheckMostShrimpInTank();
        PlayerStats.stats.shrimpMoved++;
    }

    public Vector3 GetRandomTankPosition()
    {
        List<GridNode> freePoints = tankGrid.GetFreePoints();
        return freePoints[UnityEngine.Random.Range(0, freePoints.Count)].worldPos;
    }


    public Vector3 GetRandomSurfacePosition()
    {
        List<GridNode> freePoints = tankGrid.GetSurfacePoints();
        return freePoints[UnityEngine.Random.Range(0, freePoints.Count)].worldPos;
    }


    public GridNode GetRandomTankNode()
    {
        List<GridNode> freePoints = tankGrid.GetFreePoints();
        return freePoints[UnityEngine.Random.Range(0, freePoints.Count)];
    }


    private void CheckShrimpCapacity()
    {
        if (shrimpInTank.Count != 0)
        {
            float c = chanceToKillAShrimpOverCapacity.Evaluate(shrimpInTank.Count / roughShrimpCapacity);
            if (UnityEngine.Random.value < c)
            {
                int r = UnityEngine.Random.Range(0, shrimpInTank.Count);

                Email email = EmailTools.CreateEmail();
                email.title = "YourStore@notifSystem.store";
                email.subjectLine = shrimpInTank[r].stats.name + " has died due to overpopulation";
                email.mainText = shrimpInTank[r].stats.name + " was in " + tankName;
                EmailManager.SendEmail(email);

                PlayerStats.stats.shrimpDeathsThroughOverpopulation++;

                shrimpInTank[r].KillShrimp();
            }
        }
    }


    private void CheckMostShrimpInTank()
    {
        if (shrimpInTank.Count > PlayerStats.stats.mostShrimpInOneTank)
            PlayerStats.stats.mostShrimpInOneTank = shrimpInTank.Count;
    }


    public GameObject GetCam() { return camDock; }


    public void FocusTank()
    {
        focusingTank = true;
        GameObject newView = Instantiate(tankViewPrefab, transform);
        UIManager.instance.SwitchScreen(newView.GetComponent<ScreenView>());
        newView.GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
        newView.GetComponent<Canvas>().planeDistance = 1;
        UIManager.instance.SetCursorMasking(false);
        SwitchLODLevel(LODLevel.Mid);
        tankNameChanged = false;
    }

    public void StopFocusingTank()
    {
        focusingTank = false;
        SwitchLODLevel(LODLevel.Low);
        CheckLODDistance();

        if (tankNameChanged)
            PlayerStats.stats.tanksNamed++;
        tankNameChanged = false;
    }


    private void CheckLODDistance()
    {
        if (currentLODLevel != LODLevel.High)
        {
            if (player == null) player = GameObject.Find("Player").transform;

            float dist = Vector3.Distance(player.position, transform.position);


            if (dist < 4)
                SwitchLODLevel(LODLevel.Mid);

            else if (dist < 10)
                SwitchLODLevel(LODLevel.Low);

            else
                SwitchLODLevel(LODLevel.SuperLow);
        }
    }

    public void SwitchLODLevel(LODLevel level)  // Focused on shrimp
    {
        if (currentLODLevel == level) return;

        currentLODLevel = level;

        foreach (Shrimp s in shrimpInTank)
        {
            s.SwitchLODLevel(level);
        }

        switch (level)
        {
            case LODLevel.High:
                {
                    updateTime = 0;
                    particleParent.gameObject.SetActive(true);
                    break;
                }
            case LODLevel.Mid:
                {
                    updateTime = 0.01f;
                    updateTime *= ((ShrimpManager.instance.allShrimp.Count / 200) + 1);
                    particleParent.gameObject.SetActive(true);
                    break;
                }
            case LODLevel.Low:
                {
                    updateTime = 0.1f;
                    updateTime *= ((ShrimpManager.instance.allShrimp.Count / 200) + 1);
                    particleParent.gameObject.SetActive(true);
                    break;
                }
            case LODLevel.SuperLow:
                {
                    updateTime = 1.0f;
                    updateTime *= ((ShrimpManager.instance.allShrimp.Count / 200) + 1);
                    particleParent.gameObject.SetActive(false);
                    break;
                }
        }
    }
}

public enum LODLevel
{
    High,
    Mid,
    Low,
    SuperLow
}

public struct UpgradeState
{
    public bool HeaterOn;
    public float HeaterTargetTemp;
    public bool FilterOn;
    public bool LightsOn;
}