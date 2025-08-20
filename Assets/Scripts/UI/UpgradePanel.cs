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
    [SerializeField] private Button heaterButton;
    [SerializeField] private Slider heaterSlider;
    [SerializeField] private Button heaterRepair;
    [SerializeField] private TextMeshProUGUI currentTemp;
    [SerializeField] private RectTransform tempArrow;
    private float prevTemp;
    private int arrowTimer;

    [Header("Filter")]
    [SerializeField] private Button filterButton;
    [SerializeField] private Button filterRepair;

    [Header("Decor")]
    [SerializeField] private Button DecorButton;

    [HideInInspector] private TankController tank;
    [HideInInspector] public TankController Tank
    { 
        set
        {
            tank = value;

            if (tank.GetComponent<TankUpgradeController>().CheckForUpgrade(UpgradeTypes.Heater))
            {
                heaterButton.GetComponentInChildren<TextMeshProUGUI>().text = tank.GetComponent<TankUpgradeController>().GetUpgrade(UpgradeTypes.Heater).upgrade.itemName;
                if(tank.GetComponent<TankUpgradeController>().GetUpgrade(UpgradeTypes.Heater).upgrade.thermometer != Thermometer.AutomaticThermometer)
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
                heaterRepair.interactable = false;
                heaterSlider.enabled = false;
                currentTemp.enabled = false;
            }

            if (tank.GetComponent<TankUpgradeController>().CheckForUpgrade(UpgradeTypes.Filter))
            {
                filterButton.GetComponentInChildren<TextMeshProUGUI>().text = tank.GetComponent<TankUpgradeController>().GetUpgrade(UpgradeTypes.Filter).upgrade.itemName;

            }
            else
            {
                filterRepair.interactable = false;
            }
            if(!(Tutorial.instance.flags.Contains("AccountActivated") || Tutorial.instance.flags.Contains("UpgradeStoreOpen")))
            {
                gameObject.SetActive(false);
            }
        }
        private get { return tank; }
    }


    public void Update()
    {
        if (tank != null)
        {
            currentTemp.text = Math.Round(tank.waterTemperature, 1).ToString();
            if(arrowTimer > 10)
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
        }
    }

    public void ChangeTemp()
    {
        ((Heater)tank.GetComponent<TankUpgradeController>().GetUpgrade(UpgradeTypes.Heater)).SetTargetTemperature(heaterSlider.value);
    }


}
