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

    public override void CreateUpgrade(UpgradeSO u, TankController t)
    {
        base.CreateUpgrade(u, t);
        item = (Upgrade)Items.UpHeat0;
    }


    public override void UpdateUpgrade(float elapsedTime)
    {
        if (working)
        {
            if (tank.waterTemperature > targetTemperature)
                tank.waterTemperature = Mathf.Clamp(tank.waterTemperature - ((upgrade.heaterOutput / 10) * elapsedTime), targetTemperature, 100);
            else if (tank.waterTemperature < targetTemperature)
                tank.waterTemperature = Mathf.Clamp(tank.waterTemperature + ((upgrade.heaterOutput / 10) * elapsedTime), 0, targetTemperature);

            if (upgrade.thermometer != Thermometer.NoThermometer && thermometer != null) thermometer.value = tank.waterTemperature;
        }

        base.UpdateUpgrade(elapsedTime);
    }


    public void SetTargetTemperature(float t)
    {
        targetTemperature = Mathf.Clamp(t, minTemperature, maxTemperature);
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
            email.title = "Heater on " + tank.tankName + " has broken down";
            email.subjectLine = "Please fix it";
            email.mainText = "The shrimp could die if the temperature is incorrect";
            EmailManager.SendEmail(email);
        }

        base.BreakUpgrade();
    }
}
