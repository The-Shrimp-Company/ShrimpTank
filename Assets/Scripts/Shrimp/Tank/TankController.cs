using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using DG.Tweening;
using SaveLoadSystem;
using System.Drawing;

[RequireComponent(typeof(TankUpgradeController))]
public class TankController : Interactable
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
        AmmoniumNitrate,
        ph,
        ShrimpDeath
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
    public int dayLastFed;
    public float starvationTimer;
    public float starvationTime;
    [SerializeField] private GameObject FoodAlertSign;

    [Header("Water")]
    public Transform waterLevel;
    public GameObject waterObject;
    private float waterHeight, waterScale;
    [HideInInspector] public bool waterFilled;
    [SerializeField] private float waterFillAnimLength;
    private bool waterFilling;

    public float waterQuality = 100;
    [SerializeField] float waterQualityDecreaseSpeed = 5;

    public float waterTemperature = 50;
    private float tempuratureRisingTarget;
    private float tempuratureRisingTimer;
    [SerializeField] float tempuratureChangeSpeed = 0.005f;
    [SerializeField][Range(0,50)] float naturalTempuratureVariation = 25;

    [HideInInspector] public float waterSalt = 0;
    [HideInInspector] public float waterAmmonium = 0;
    [HideInInspector] public float waterPh = 7;

    [HideInInspector] public float idealTemp = 0;
    [HideInInspector] public float idealSalt = 0;
    [HideInInspector] public float idealNitr = 0;
    [HideInInspector] public float idealPh = 0;

    public float tempVariance = 10;
    public float saltVariance = 10;
    public float nitrVariance = 10;
    public float pHVariance = 2;


    [Header("Upgrades")]
    [HideInInspector] public TankUpgradeController upgradeController;
    [HideInInspector] public UpgradeState upgradeState = new UpgradeState() { LightsOn = true };
    [SerializeField] private List<Light> lights;

    [Header("Sale Tank")]
    [SerializeField] private GameObject SaleSign;
    public bool openTank { get; private set; } = false;
    [SerializeField] private TextMeshProUGUI label;
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
    public float breedingCooldown = 20;
    [HideInInspector] public float breedingCooldownTimer;
    [HideInInspector] public bool shrimpCanBreed;

    [Header("Alarms")]
    public List<string> AlarmIds = new List<string>();

    [Header("Misc")]
    [HideInInspector] public bool tankLoaded;
    private bool invalidShrimpHover;

    [Header("Optimisation")]
    private LODLevel currentLODLevel;
    [SerializeField] float distanceCheckTime = 1;
    private float distanceCheckTimer;
    public Transform particleParent;

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

        breedingCooldownTimer = breedingCooldown;
        shrimpCanBreed = false;
        
        if (!tankLoaded)
            EmptyWater();

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
        /*
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
        */

        // Deals with shrimp food
        if(shrimpInTank.Count > 0)
        {
            if (dayLastFed <= TimeManager.instance.day - 2)
            {
                if(starvationTimer == 0)
                {
                    starvationTimer = UnityEngine.Random.Range(5, 30);
                    starvationTime = 0;
                }
            }
            else
            {
                starvationTimer = 0;
            }

            if (starvationTimer != 0)
            {
                if (starvationTime > starvationTimer)
                {
                    shrimpInTank[UnityEngine.Random.Range(0, shrimpInTank.Count)].KillShrimp(DeathReason: "of hunger"); 
                    starvationTimer = UnityEngine.Random.Range(5, 30);
                    starvationTime = 0;
                }
                else
                {
                    starvationTime += Time.deltaTime;
                }
            }
        }
        else
        {
            dayLastFed = TimeManager.instance.day-1;
        }

        FoodAlertSign.SetActive(shrimpInTank.Count > 0 && !FedShrimpToday());
        MouseHover();
        label.text = tankName;

        if (focusingTank) PlayerStats.stats.timeSpentFocusingTank += Time.deltaTime;
        interactable = (!waterFilling && waterFilled && tooltip.toolTip != "");
        holdInteractable = (!waterFilling && shrimpInTank.Count <= 0);
    }

    public void FeedShrimp()
    {
        dayLastFed = TimeManager.instance.day;
    }

    public bool FedShrimpToday() { return (dayLastFed == TimeManager.instance.day || shrimpInTank.Count == 0); }
    public bool FedTankToday() { return (dayLastFed == TimeManager.instance.day); }

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
                if (shrimpToRemove[i].saleSlotIndex != -1)
                {
                    CustomerManager.Instance.shrimpSaleSlots[shrimpToRemove[i].saleSlotIndex] = new();
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
        waterTemperature = Mathf.Clamp(waterTemperature - tempuratureChangeSpeed * updateTimer, 20, 80);

        waterSalt = Mathf.Clamp(waterSalt - 0.01f * updateTimer * shrimpInTank.Count, 0, 100);
        
        waterAmmonium = Mathf.Clamp(waterAmmonium - 0.01f * updateTimer * shrimpInTank.Count * UnityEngine.Random.value, 0, 100);



        upgradeController.UpdateUpgrades(updateTimer);

        

        idealTemp = 0;
        foreach(Shrimp shrimp in shrimpInTank)
        {
            idealTemp += shrimp.stats.temperaturePreference;
        }
        idealTemp /= shrimpInTank.Count;

        idealNitr = 0;
        foreach(Shrimp shrimp in shrimpInTank)
        {
            idealNitr += shrimp.stats.ammoniaPreference;
        }
        idealNitr /= shrimpInTank.Count;

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
        CheckStatAlarm(waterTemperature, idealTemp, tempVariance, "Your tank is the wrong temperature", AlarmTypes.Temp);
        CheckStatAlarm(waterSalt, idealSalt, saltVariance, "Your tank has the wrong salt level", AlarmTypes.Salt);
        CheckStatAlarm(waterAmmonium, idealNitr, nitrVariance, "Your tank has the wrong Nitrate level", AlarmTypes.AmmoniumNitrate);
        CheckStatAlarm(waterPh, idealPh, pHVariance, "Your tank has the wrong pH level", AlarmTypes.ph);
        if(shrimpInTank.Count > 0)
        {
            CheckMinValueAlarm(dayLastFed, TimeManager.instance.day-2, "Shrimp Haven't been fed!", AlarmTypes.Food);
        }
    }

    /// <summary>
    /// When given the current value and target value, if the two are further apart than the maximum difference, an alarm of the given type, 
    /// with the given message wll be sent.
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="targetValue"></param>
    /// <param name="maximumDifference">Value is inclusive.</param>
    /// <param name="alarmMessage">Message to send</param>
    /// <param name="alarmType">Defined in the enumerator, used to identify the previous alarms of the same type.</param>
    private void CheckStatAlarm(float currentValue, float targetValue, float maximumDifference, string alarmMessage, AlarmTypes alarmType)
    {
        if (Mathf.Abs(currentValue - targetValue) >= maximumDifference)
        {
            Email email = CreateOrFindAlarm(alarmType);
            email.subjectLine = alarmType.ToString() + " warning in tank " + tankName;
            email.mainText = alarmMessage;
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

    /// <summary>
    /// Alternative of the CheckStatAlarm, used when there is a value which can be arbitrarily high, but not below a certain level. Otherwise identical to Check
    /// StatAlarm.
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="minimumValue">Value is inclusive</param>
    /// <param name="AlarmMessage"></param>
    /// <param name="alarmType">USed to identify previous alarms from this tank of the same type.</param>
    private void CheckMinValueAlarm(float currentValue, float minimumValue, string AlarmMessage, AlarmTypes alarmType)
    {
        if(currentValue <= minimumValue)
        {
            Email email = CreateOrFindAlarm(alarmType);
            email.subjectLine = alarmType.ToString() + " warning in tank " + tankName;
            email.mainText = AlarmMessage;
        }
        else
        {
            Email email = FindAlarm(alarmType);
            if(email != null)
            {
                AlarmIds.Remove(email.ID);
                EmailManager.RemoveEmail(email);
            }
        }
    }

    public void ShrimpDiedAlarm(ShrimpStats shrimp, string EmailMessage = "")
    {
        Email email = FindAlarm(AlarmTypes.ShrimpDeath);
        if(email != null)
        {
            UIManager.instance.PushNotification((shrimp.name + " has died as well"), true);
        }
        else
        {
            email = CreateAlarm(AlarmTypes.ShrimpDeath);
            email.CreateEmailButton("Delete", true);
        }

        email.mainText += "\n" + shrimp.name + " has died";
        if (EmailMessage != "") email.mainText += " " + EmailMessage;
        
        
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
        EmailManager.SendEmailDirect(email, false);
        return email;
    }

    private Email FindAlarm(AlarmTypes type)
    {
        Email[] emails = EmailManager.instance.emails.Where(x => x.tag == Email.EmailTags.Alarms && AlarmIds.Contains(x.ID)).ToArray();
        if (emails.Length >= 1) return Array.Find(emails, (x) => { return x.title == type.ToString(); });
        else return null;
    }

    private void ClearAlarms()
    {
        foreach (string alarmId in AlarmIds)
        {
            EmailManager.RemoveEmail(EmailManager.instance.emails.Find(x => x.ID == alarmId));
        }
        AlarmIds.Clear();
    }



    public void toggleTankOpen()
    {
        if (CustomerManager.Instance == null) return;
        openTank = !openTank;
        if (openTank) CustomerManager.Instance.openTanks.Add(this);
        else CustomerManager.Instance.openTanks.Remove(this);
        SaleSign.SetActive(openTank);
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

                /*
                Email email = EmailTools.CreateEmail();
                email.title = "YourStore@notifSystem.store";
                email.subjectLine = shrimpInTank[r].stats.name + " has died due to overpopulation";
                email.mainText = shrimpInTank[r].stats.name + " was in " + tankName;
                EmailManager.SendEmail(email);
                */

                PlayerStats.stats.shrimpDeathsThroughOverpopulation++;

                shrimpInTank[r].KillShrimp(DeathReason:"of overpopulation");
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


    public override void Action()
    {
        bool stopHolding = true;

        Item item = Store.player.GetComponent<HeldItem>().GetHeldItem();
        ItemSO so = null;
        if (item != null) so = Inventory.GetSOForItem(item);

        if (!so)  // Not holding anything
        {
            Store.player.GetComponent<PlayerInteraction>().SetTankFocus(this);
        }
        else
        {
            if (so.tags.Contains(ItemTags.Shrimp))  // Holding a Shrimp
            {
                ShrimpStats s = (item as ShrimpItem).shrimp;

                if (!invalidShrimpHover)
                {
                    SpawnShrimp((item as ShrimpItem).shrimp);
                    Inventory.RemoveShrimp(s);
                }
                else
                {
                    // Water not suitable
                    stopHolding = false;
                }
            }
            else if (so.tags.Contains(ItemTags.Food) && !FedTankToday())  // Holding Food
            {
                FeedShrimp();
                Store.player.GetComponent<HeldItem>().UseHeldItem();
                Inventory.RemoveItem(item);

                if (Inventory.GetItemQuantity(item) > 0) stopHolding = false;
            }
            else if (so.tags.Contains(ItemTags.Medicine) && shrimpInTank.Count != 0)  // Holding Medicine
            {
                foreach (Shrimp shrimp in shrimpInTank)
                {
                    shrimp.illnessCont.UseMedicine((so as MedicineItemSO));
                }

                Store.player.GetComponent<HeldItem>().UseHeldItem();
                Inventory.RemoveItem(item);
            }
            else if (so.tags.Contains(ItemTags.Salt))  // Holding Salt
            {
                waterSalt = Mathf.Clamp(waterSalt + 10, 0, 100);
                Inventory.RemoveItem(item);

                if (Inventory.GetItemQuantity(item) > 0) stopHolding = false;
            }
        }

        if (stopHolding) Store.player.GetComponent<HeldItem>().StopHoldingItem();
        OnHover();
    }

    public override void OnHover()
    {
        if (tooltip)
        {
            Item item = Store.player.GetComponent<HeldItem>().GetHeldItem();
            invalidShrimpHover = false;
            if (item != null)
            {
                if (Inventory.GetSOForItem(item).tags.Contains(ItemTags.Shrimp))
                {
                    int suitableTank = 0;
                    if (Mathf.Abs(waterTemperature - (item as ShrimpItem).shrimp.temperaturePreference) >= 10) suitableTank++;
                    if (Mathf.Abs(waterSalt - (item as ShrimpItem).shrimp.salineLevel) >= 10) suitableTank++;
                    if (Mathf.Abs(waterAmmonium - (item as ShrimpItem).shrimp.ammoniaPreference) >= 10) suitableTank++;
                    if (Mathf.Abs(waterPh - (item as ShrimpItem).shrimp.PhPreference) >= 2) suitableTank++;

                    if (suitableTank == 4)
                    {
                        tooltip.toolTip = "<color=red>Water is not suitable for this Shrimp</color>";
                        invalidShrimpHover = true;
                    }
                    //else if (!FedTankToday())
                    //{
                    //    tooltip.toolTip = "<color=red>Tank must have food</color>";
                    //    invalidShrimpHover = true;
                    //}
                    else
                    {
                        tooltip.toolTip = "Put shrimp in " + tankName;
                        invalidShrimpHover = false;
                    }
                }
                else if (Inventory.GetSOForItem(item).tags.Contains(ItemTags.Food) && !FedTankToday() && shrimpInTank.Count > 0)
                    tooltip.toolTip = "Put food in " + tankName;
                else if (Inventory.GetSOForItem(item).tags.Contains(ItemTags.Medicine) && shrimpInTank.Count > 0)
                    tooltip.toolTip = "Put medicine in " + tankName;
                else if (Inventory.GetSOForItem(item).tags.Contains(ItemTags.Salt))
                    tooltip.toolTip = "Put salt in " + tankName;
                else tooltip.toolTip = "";
            }
            else
                tooltip.toolTip = tankName;
            
        }
    }

    public override void OnStopHover()
    {

    }


    public void FillWater()
    {
        if (waterFilling) return;

        bool animate = true;
        if (waterHeight == 0)
        {
            waterHeight = waterObject.transform.localPosition.z;
            waterScale = waterObject.transform.localScale.y;
            animate = false;
        }
        waterFilling = true;
        waterFilled = true;
        waterObject.SetActive(true);
        RemoveHoldAction("Fill Water");
        RemoveHoldAction("Empty Water");
        RemoveHoldAction("Move");
        RemoveHoldAction("Remove");

        waterAmmonium = 0;
        waterTemperature = 20;
        waterSalt = 0;
        waterPh = 7;

        if (animate)
        {
            waterObject.transform.localPosition = new Vector3(waterObject.transform.localPosition.x, waterObject.transform.localPosition.y, 0);
            waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, 0, waterObject.transform.localScale.z);

            waterObject.transform.DOLocalMoveZ(waterHeight, waterFillAnimLength).SetEase(Ease.InOutSine).OnComplete(WaterFinishFilling);
            waterObject.transform.DOScaleY(waterScale, waterFillAnimLength).SetEase(Ease.InOutSine);
        }
        else
        {
            waterObject.transform.localPosition = new Vector3(waterObject.transform.localPosition.x, waterObject.transform.localPosition.y, waterHeight);
            waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, waterScale, waterObject.transform.localScale.z);
            WaterFinishFilling();
        }
    }

    public void EmptyWater()
    {
        if (waterFilling) return;

        if (shrimpInTank.Count != 0)
        {
            Debug.Log("Cannot empty tank with shrimp in it");
            return;
        }

        bool animate = true;
        if (waterHeight == 0)
        {
            waterHeight = waterObject.transform.localPosition.z;
            waterScale = waterObject.transform.localScale.y;
            animate = false;
        }

        DecorateTankController.Instance.ClearTank(this);

        waterFilling = true;
        waterFilled = false;
        RemoveHoldAction("Fill Water");
        RemoveHoldAction("Empty Water");
        RemoveHoldAction("Move");
        RemoveHoldAction("Remove");

        if (animate)
        {
            waterObject.transform.localPosition = new Vector3(waterObject.transform.localPosition.x, waterObject.transform.localPosition.y, waterHeight);
            waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, waterScale, waterObject.transform.localScale.z);

            waterObject.transform.DOLocalMoveZ(0, waterFillAnimLength).SetEase(Ease.InOutSine).OnComplete(WaterFinishFilling);
            waterObject.transform.DOScaleY(0, waterFillAnimLength).SetEase(Ease.InOutSine);
        }
        else
        {
            waterObject.transform.localPosition = new Vector3(waterObject.transform.localPosition.x, waterObject.transform.localPosition.y, 0);
            waterObject.transform.localScale = new Vector3(waterObject.transform.localScale.x, 0, waterObject.transform.localScale.z);
            WaterFinishFilling();
        }
    }

    private void WaterFinishFilling() 
    { 
        waterFilling = false;
        waterObject.SetActive(waterFilled);
        if (waterFilled)
        {
            AddHoldAction("Empty Water", EmptyWater);
        }
        else
        {
            Decoration decoration = GetComponent<Decoration>();
            AddHoldAction("Fill Water", FillWater);
            AddHoldAction("Move", decoration.MoveDecoration);
            AddHoldAction("Remove", decoration.RemoveDecoration);
        }
    }


    private void CheckLODDistance()
    {
        if (currentLODLevel != LODLevel.High)
        {
            float dist = Vector3.Distance(Store.player.transform.position, transform.position);


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

    private void OnDestroy()
    {
        ClearAlarms();
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