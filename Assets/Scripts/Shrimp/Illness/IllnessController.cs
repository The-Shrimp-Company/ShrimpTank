using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IllnessController : MonoBehaviour
{
    public IllnessSO[] possibleIllness;
    public List<IllnessSO> currentIllness = new List<IllnessSO>();
    public List<Symptom> currentSymptoms = new List<Symptom>();
    private Shrimp shrimp;
    [SerializeField] float illnessCheckTime = 5f;
    private float illnessCheckTimer;
    [SerializeField] float severityBoostIfSymptomIsAlreadyPresent = 20;
    public AnimationCurve severityCurve;
    [SerializeField] GameObject curingParticles;
    public GameObject gainIllnessParticles;
    private bool loadingIllnesses = false;

    [Header("Illness Unlock Requirement")]
    [SerializeField] int unlockReqTotalShrimp = 50;
    [SerializeField] int unlockReqShrimpInOneTank = 20;

    private void Start()
    {
        shrimp = GetComponent<Shrimp>();
    }

    public void UpdateIllness(float elapsedTime)
    {
        // Illnesses will not start to appear until certain requirements are met
        if (unlockReqTotalShrimp > PlayerStats.stats.totalShrimp &&
            unlockReqShrimpInOneTank > PlayerStats.stats.mostShrimpInOneTank)
            return;


        illnessCheckTimer += Time.deltaTime;
        if (illnessCheckTimer > illnessCheckTime)
        {
            foreach (IllnessSO i in possibleIllness)
            {
                if (!currentIllness.Contains(i))
                {
                    CheckForIllness(i, elapsedTime);
                }
            }

            illnessCheckTimer = 0;
        }

        shrimp.stats.illnessLevel = 0;  // Set the illness level to 0, this will be recalculated in the symptom updates
        foreach (Symptom s in currentSymptoms)
        {
            s.UpdateSymptom(elapsedTime);
        }
    }

    public void CheckForIllness(IllnessSO so, float elapsedTime)
    {
        float triggerChance = 1;

        switch (so.trigger)
        {
            case IllnessTriggers.RandomChance:
                {
                    triggerChance += (Random.Range(1, 100)); 
                    break;
                }
            case IllnessTriggers.WaterQuality:
                {
                    triggerChance += (-shrimp.tank.waterQuality + 100);
                    break;
                }
            case IllnessTriggers.Hunger:
                {
                    triggerChance += (-shrimp.stats.hunger + 100);
                    break;
                }
            case IllnessTriggers.HotWater:
                {
                    triggerChance += (Mathf.Clamp(shrimp.tank.waterTemperature - 50, 0, 50) * 2);
                    break;
                }
            case IllnessTriggers.ColdWater:
                {
                    triggerChance += (-Mathf.Clamp(shrimp.tank.waterTemperature - 50, -50, 0) * 2);
                    break;
                }
        }

        // Trigger chance is now a value from 1-100 based on the trigger
        if (shrimp.tank.currentIllness.ContainsKey(so))
        {
            float tankInfection = Mathf.Clamp(Mathf.InverseLerp(0, shrimp.tank.roughShrimpCapacity, shrimp.tank.currentIllness[so]) * shrimp.tank.illnessSpreadRate, 0, shrimp.tank.illnessSpreadRate) + 1;
            //float spreadRate = (shrimp.tank.illnessSpreadRate / 100) + 1;
            //triggerChance = Mathf.Clamp(triggerChance + (tankInfection * spreadRate), 0, 100);
            triggerChance = Mathf.Clamp(triggerChance * tankInfection, 0, 100);
        }

        if (Random.value < so.triggerChance.Evaluate(triggerChance / 100))
        {
            AddIllness(so);
        }
    }

    public void AddIllness(IllnessSO i)
    {
        if (currentIllness.Contains(i)) return;

        foreach (IllnessSymptoms s in i.symptoms)
        {
            Symptom symptom;

            switch (s)
            {
                case IllnessSymptoms.BodySize:
                {
                    symptom = new SymptomBodySize();
                    if (loadingIllnesses) 
                        symptom.severity = shrimp.stats.symptoms[0];
                    break;
                }

                case IllnessSymptoms.Discolouration:
                {
                    symptom = new SymptomDiscolouration();
                    if (loadingIllnesses)
                        symptom.severity = shrimp.stats.symptoms[1];
                    break;
                }

                case IllnessSymptoms.Bubbles:
                {
                    symptom = new SymptomBubbles();
                    if (loadingIllnesses)
                        symptom.severity = shrimp.stats.symptoms[2];
                    break;
                }

                default:
                {
                    symptom = null;
                    break;
                }
            }

            foreach (Symptom x in currentSymptoms)
            {
                if (x.GetType() == symptom.GetType())
                {
                    symptom = null;
                    x.severity += severityBoostIfSymptomIsAlreadyPresent;
                }
            }

            if (symptom != null && symptom.GetType() != typeof(Symptom))
            {
                currentSymptoms.Add(symptom);
                symptom.shrimp = shrimp;
                symptom.StartSymptom();
            }
        }

        currentIllness.Add(i);
        AddIllnessToTank(shrimp.tank, i);

        if (!loadingIllnesses)
            PlayerStats.stats.illnessesGained++;

        if (gainIllnessParticles != null)
            GameObject.Instantiate(gainIllnessParticles, shrimp.transform.position, shrimp.transform.rotation, shrimp.particleParent); ;
    }



    public void MoveShrimp(TankController oldTank, TankController newTank)
    {
        foreach (IllnessSO i in currentIllness)
        {
            RemoveIllnessFromTank(oldTank, i);

            AddIllnessToTank(newTank, i);
        }
    }


    public void RemoveShrimp()
    {
        foreach(IllnessSO i in currentIllness)
        {
            RemoveIllnessFromTank(shrimp.tank, i);
        }
    }


    public void AddIllnessToTank(TankController t, IllnessSO i)
    {
        if (t.currentIllness.ContainsKey(i))
            t.currentIllness[i]++;
        else t.currentIllness.Add(i, 1);
    }


    private void RemoveIllnessFromTank(TankController t, IllnessSO i)
    {
        t.currentIllness[i]--;
    }


    public void UseMedicine(Medicine m)
    {
        foreach (Symptom s in currentSymptoms)
        {
            foreach (IllnessSymptoms i in m.symptoms)
            {
                if (s.symptom == i)
                {
                    s.severity -= m.strength;
                }
            }
        }

        for (int x = currentIllness.Count - 1; x >= 0; x--)  // For every illness the shrimp has
        {
            List<Symptom> illnessSymptoms = new List<Symptom>();

            foreach (IllnessSymptoms y in currentIllness[x].symptoms)  // Check each symptom of the illness and whether it has been cured
            {
                Symptom symptom = currentSymptoms.Find(i => i.symptom == y);

                if (symptom != null && symptom.severity < 0)
                {
                    illnessSymptoms.Add(symptom);
                }
            }


            if (illnessSymptoms.Count == currentIllness[x].symptoms.Count)  // If all symptoms of the illness have been cured
            {
                CureIllness(currentIllness[x], illnessSymptoms);
            }
        }
    }


    private void CureIllness(IllnessSO illness, List<Symptom> illnessSymptoms)
    {
        for (int i = illnessSymptoms.Count - 1; i >= 0; i--)  // For each symptom in the illness
        {
            bool usedByAnotherIllness = false;
            foreach (IllnessSO v in currentIllness)  // If the symptom is not being used by another illness
            {
                if (v != illness && v.symptoms.Contains(illnessSymptoms[i].symptom))
                    usedByAnotherIllness = true;
            }

            if (!usedByAnotherIllness)  // Remove the symptom
            {
                illnessSymptoms[i].EndSymptom();
                currentSymptoms.RemoveAt(i);
            }
        }

        PlayerStats.stats.illnessesCured++;


        currentIllness.Remove(illness);
        RemoveIllnessFromTank(shrimp.tank, illness);


        if (curingParticles != null)
        {
            GameObject.Instantiate(curingParticles, shrimp.transform.position, shrimp.transform.rotation, shrimp.particleParent); ;
        }
    }


    public void CureAllIllnesses()
    {
        if (currentIllness.Count == 0) return;

        foreach (Symptom symptom in currentSymptoms)
        {
            symptom.EndSymptom();
        }

        foreach (IllnessSO illness in currentIllness)
        { 
            RemoveIllnessFromTank(shrimp.tank, illness);
        }

        currentSymptoms.Clear();
        currentIllness.Clear();

        if (curingParticles != null)
        {
            GameObject.Instantiate(curingParticles, shrimp.transform.position, shrimp.transform.rotation, shrimp.particleParent); ;
        }
    }


    public void SaveIllnesses()
    {
        shrimp.stats.illness = new bool[possibleIllness.Length];
        for(int i = 0; i < possibleIllness.Length; i++)
        {
            if (currentIllness.Contains(possibleIllness[i]))
                shrimp.stats.illness[i] = true;
            else
                shrimp.stats.illness[i] = false;
        }

        shrimp.stats.symptoms = new float[3] { 0, 0, 0 };
        SaveSymptom(typeof(SymptomBodySize), 0);
        SaveSymptom(typeof(SymptomDiscolouration), 1);
        SaveSymptom(typeof(SymptomBubbles), 2);
    }


    private void SaveSymptom(System.Type t, int index)
    {
        Symptom currentSymptom = currentSymptoms.Find(i => i.GetType() == t);
        if (currentSymptom != null) shrimp.stats.symptoms[index] = currentSymptom.severity;
    }


    public void LoadIllnesses(ShrimpStats s)
    {
        loadingIllnesses = true;
        shrimp = GetComponent<Shrimp>();

        for (int i = 0; i < possibleIllness.Length; i++)
        {
            if (s.illness.Length >= i && s.illness[i] == true)
                AddIllness(possibleIllness[i]);
        }

        loadingIllnesses = false;
    }
}