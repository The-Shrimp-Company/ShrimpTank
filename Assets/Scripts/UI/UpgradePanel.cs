using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using System;

public class UpgradePanel : MonoBehaviour
{
    [Header("Heater")]
    [SerializeField] private GameObject heaterPanel;
    [SerializeField] private TMP_Text heaterName;
    [SerializeField] private Slider heaterSlider;
    [SerializeField] private Button heaterRepair;
    [SerializeField] private Button heaterRemove;
    [SerializeField] private TextMeshProUGUI currentTemp;
    [SerializeField] private RectTransform tempArrow;
    private float prevTemp;
    private int arrowTimer;

    [Header("Filter")]
    [SerializeField] private GameObject filterPanel;
    [SerializeField] private TMP_Text filterName;
    [SerializeField] private Button filterRepair;
    [SerializeField] private Button filterRemove;

    [Header("Label")]
    [SerializeField] private GameObject labelPanel;
    [SerializeField] private TMP_Text labelName;
    [SerializeField] private Button labelRemove;

    [Header("Salt")]
    [SerializeField] private TextMeshProUGUI saltLabel;

    [Header("Ammonium")]
    [SerializeField] private TextMeshProUGUI ammoniumLabel;

    [Header("pH")]
    [SerializeField] private TextMeshProUGUI phLabel;

    [Header("Decor")]
    [SerializeField] private Button DecorButton;

    [HideInInspector] private TankController tank;
    [HideInInspector] public TankController Tank
    { 
        set
        {
            tank = value;

            UpdatePanel();
            /*
            if(!(Tutorial.instance.flags.Contains("AccountActivated") || Tutorial.instance.flags.Contains("UpgradeStoreOpen")))
            {
                gameObject.SetActive(false);
            }
            */
        }
        private get { return tank; }
    }


    public void Update()
    {
        if (tank != null)
        {
            currentTemp.text = Math.Round(tank.waterTemperature, 1).ToString() + "°C";
            if(arrowTimer > 20)
            {
                arrowTimer = 0;
                if (MathF.Abs(tank.waterTemperature - prevTemp) < 0.0001f)
                {
                    tempArrow.localScale = new Vector3(1, 0, 1);
                }
                else
                {
                    tempArrow.localScale = new Vector3(1, Mathf.Sign(tank.waterTemperature - prevTemp), 1);
                }
                prevTemp = tank.waterTemperature;
            }
            else
            {
                arrowTimer++;
            }
            saltLabel.text = "Salt Level: " + ((int)tank.waterSalt).ToString();
            phLabel.text = "pH Level: " + ((int)tank.waterPh).ToString();
            ammoniumLabel.text = "Nitrate Level: " + ((int)tank.waterAmmonium).ToString();
        }
    }

    private void UpdatePanel()
    {
        if (!tank) return;

        if (tank.GetComponent<TankUpgradeController>().CheckForUpgrade(UpgradeTypes.Heater))
        {
            heaterName.text = tank.GetComponent<TankUpgradeController>().GetUpgrade(UpgradeTypes.Heater).upgrade.itemName;
            heaterRemove.interactable = true;
            if (tank.GetComponent<TankUpgradeController>().GetUpgrade(UpgradeTypes.Heater).upgrade.thermometer != Thermometer.AutomaticThermometer)
            {
                heaterSlider.gameObject.SetActive(false);
            }
            else
            {
                heaterSlider.gameObject.SetActive(true);
            }
            currentTemp.enabled = true;
            prevTemp = tank.waterTemperature;
        }
        else
        {
            heaterPanel.SetActive(false);
            heaterRemove.interactable = false;
            heaterRepair.interactable = false;
            heaterSlider.enabled = false;
            currentTemp.enabled = false;
        }

        if (tank.GetComponent<TankUpgradeController>().CheckForUpgrade(UpgradeTypes.Filter))
        {
            filterName.text = tank.GetComponent<TankUpgradeController>().GetUpgrade(UpgradeTypes.Filter).upgrade.itemName;
            filterRemove.interactable = true;
        }
        else
        {
            filterPanel.SetActive(false);
            filterRemove.interactable = false;
            filterRepair.interactable = false;
        }

        if (tank.GetComponent<TankUpgradeController>().CheckForUpgrade(UpgradeTypes.Label))
        {
            labelName.text = tank.GetComponent<TankUpgradeController>().GetUpgrade(UpgradeTypes.Label).upgrade.itemName;
            labelRemove.interactable = true;
        }
        else
        {
            labelPanel.SetActive(false);
            labelRemove.interactable = false;
        }
    }

    public void ChangeTemp()
    {
        ((Heater)tank.GetComponent<TankUpgradeController>().GetUpgrade(UpgradeTypes.Heater)).SetTargetTemperature(heaterSlider.value);
    }

    public void RemoveHeater() { RemoveUpgrade(UpgradeTypes.Heater); }
    public void RemoveFilter() { RemoveUpgrade(UpgradeTypes.Filter); }
    public void RemoveLabel() { RemoveUpgrade(UpgradeTypes.Label); }
    private void RemoveUpgrade(UpgradeTypes type)
    {
        TankUpgradeController controller = tank.GetComponent<TankUpgradeController>();
        if (controller && controller.CheckForUpgrade(type))
        {
            controller.RemoveUpgrade(type);
            UpdatePanel();
        }
    }
}
