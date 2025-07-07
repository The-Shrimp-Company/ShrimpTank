using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Decorations : TankUpgrade
{
    public List<Modifier> statModifiers;

    public override void CreateUpgrade(UpgradeItemSO u, TankController t)
    {
        base.CreateUpgrade(u, t);
    }


    public override void UpdateUpgrade(float elapsedTime)
    {
        base.UpdateUpgrade(elapsedTime);
    }


    public ShrimpStats ApplyStatModifiers(ShrimpStats s)
    {
        foreach (Modifier m in statModifiers)
        {
            switch (m.type)
            {
                case ModifierEffects.temperament:
                    {
                        s.temperament = Mathf.Clamp(s.temperament + m.effect, 0, 100);
                        break;
                    }

                case ModifierEffects.geneticSize:
                    {
                        s.geneticSize = Mathf.Clamp(s.geneticSize + m.effect, 0, 100);
                        break;
                    }
                case ModifierEffects.salineLevel:
                    {
                        s.salineLevel = Mathf.Clamp(s.salineLevel + m.effect, 0, 100);
                        break;
                    }
                case ModifierEffects.immunity:
                    {
                        s.immunity = Mathf.Clamp(s.immunity + m.effect, 0, 100);
                        break;
                    }
                case ModifierEffects.metabolism:
                    {
                        s.metabolism = Mathf.Clamp(s.metabolism + m.effect, 0, 100);
                        break;
                    }
                case ModifierEffects.filtration:
                    {
                        s.filtration = Mathf.Clamp(s.filtration + m.effect, 0, 100);
                        break;
                    }
                case ModifierEffects.temperature:
                    {
                        s.temperaturePreference = Mathf.Clamp(s.temperaturePreference + m.effect, 0, 100);
                        break;
                    }
            }
        }

        return s;
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
        base.BreakUpgrade();
    }
}
