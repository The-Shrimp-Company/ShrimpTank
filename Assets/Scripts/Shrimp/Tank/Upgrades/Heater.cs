using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heater : TankUpgrade
{
    [Header("Heater")]
    public Slider thermometer;
    private float targetTemperature = 50;
    [Range(0, 100)] public float minTemperature = 25;
    [Range(0, 100)] public float maxTemperature = 75;

    public override void CreateUpgrade(UpgradeItemSO u, TankController t)
    {
        base.CreateUpgrade(u, t);
        t.upgradeState.HeaterOn = true;
        t.upgradeState.HeaterTargetTemp = targetTemperature;
    }


    public override void UpdateUpgrade(float elapsedTime)
    {
        if (tank.upgradeState.HeaterOn)
        {
            if (working)
            {
                if(upgrade.thermometer == Thermometer.AutomaticThermometer)
                {
                    if (tank.waterTemperature > targetTemperature)
                        tank.waterTemperature = Mathf.Clamp(tank.waterTemperature - ((upgrade.heaterOutput / 10) * elapsedTime), targetTemperature, 100);
                    else if (tank.waterTemperature < targetTemperature)
                        tank.waterTemperature = Mathf.Clamp(tank.waterTemperature + ((upgrade.heaterOutput / 10) * elapsedTime), 0, targetTemperature);
                }
                else if(upgrade.thermometer == Thermometer.NoThermometer)
                {
                    tank.waterTemperature += upgrade.heaterOutput * elapsedTime;
                }
            }
            if (/*upgrade.thermometer != Thermometer.NoThermometer &&*/ thermometer != null) thermometer.value = tank.waterTemperature;

            base.UpdateUpgrade(elapsedTime);
        }
    }


    public void SetTargetTemperature(float t)
    {
        targetTemperature = Mathf.Clamp(t, minTemperature, maxTemperature);
        tank.upgradeState.HeaterTargetTemp = targetTemperature;
    }


    public float GetTargetTemperature()
    {
        return targetTemperature;
    }


    public override void RemoveUpgrade()
    {
        base.RemoveUpgrade();
    }


    public override void FixUpgrade()
    {
        base.FixUpgrade();
    }


    public override void BreakUpgrade()
    {
        if (tank.shrimpInTank.Count != 0)
        {
            Email email = EmailTools.CreateEmail();
            email.title = "YourStore@notifSystem.store";
            email.subjectLine = "Heater on " + tank.tankName + " has broken down";
            email.mainText = "The shrimp could die if the temperature is incorrect";
            EmailManager.SendEmail(email);
        }

        base.BreakUpgrade();
    }
}
