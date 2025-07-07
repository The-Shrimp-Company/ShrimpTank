using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUpgrade : MonoBehaviour
{
    [HideInInspector] public UpgradeItemSO upgrade;
    protected TankController tank;
    public bool working = true;
    public GameObject brokenParticlesPrefab;
    private GameObject brokenParticles;

    [Header("Illness Unlock Requirement")]
    [SerializeField] int unlockReqTotalShrimp = 50;
    [SerializeField] int unlockReqShrimpInOneTank = 20;


    public virtual void CreateUpgrade(UpgradeItemSO u, TankController t)
    {
        tank = t;
        upgrade = u;
        working = true;
    }


    public virtual void UpdateUpgrade(float elapsedTime)
    {
        // Upgrades will not start to break until certain requirements are met
        if (unlockReqTotalShrimp <= PlayerStats.stats.totalShrimp ||
            unlockReqShrimpInOneTank <= PlayerStats.stats.mostShrimpInOneTank)
        {
            if ((Random.value * 100) < (upgrade.breakRate / 1000) * elapsedTime)
                BreakUpgrade();
        }
    }


    public virtual void RemoveUpgrade()
    {

    }


    public virtual void FixUpgrade()
    {
        if (working == false)
        {
            working = true;

            if (brokenParticles != null)
            {
                brokenParticles.GetComponentInChildren<ParticleSystem>().Stop();
                brokenParticles.GetComponent<DestroyAfter>().enabled = true;
                brokenParticles = null;
            }
        }
    }


    public virtual void BreakUpgrade()
    {
        if (working == true)
        {
            working = false;

            if (brokenParticlesPrefab != null)
            {
                brokenParticles = GameObject.Instantiate(brokenParticlesPrefab, transform.position, transform.rotation, tank.particleParent);
            }
        }
    }

    public bool IsBroken()
    {
        if (working) return false;
        else return true;
    }
}
