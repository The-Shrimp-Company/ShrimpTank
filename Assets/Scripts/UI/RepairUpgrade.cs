using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;
using TMPro;

public class RepairUpgrade : MonoBehaviour
{
    [SerializeField] private UpgradeTypes upgradeType;
    private Button button;
    private TankUpgradeController controller;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        controller = GetComponentInParent<TankUpgradeController>();
        if (controller == null) return;
        button.onClick.AddListener(() =>
        {
            if (Money.instance.WithdrawMoney(controller.GetUpgrade(upgradeType).upgrade.repairCost))
            {
                controller.GetUpgrade(upgradeType).FixUpgrade();
            }
        });
        if (controller.CheckForUpgrade(upgradeType))
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Repair for £" + controller.GetUpgrade(upgradeType).upgrade.repairCost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null) controller = GetComponentInParent<TankUpgradeController>();
        if (controller == null) return;

        if (controller.CheckForUpgrade(upgradeType) && controller.GetUpgrade(upgradeType).IsBroken()) button.interactable = true;
        else button.interactable = false;
    }
}
