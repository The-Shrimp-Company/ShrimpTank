using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SellContentBlock : ContentBlock
{
    private Shrimp _shrimp;
    [SerializeField]
    private TextMeshProUGUI _salePrice;

    public void SetShrimp(Shrimp shrimp)
    {
        _shrimp = shrimp;
    }

    public void SellShrimp()
    {
        PlayerStats.stats.shrimpSoldThroughOpenTank++;
        CustomerManager.Instance.PurchaseShrimp(_shrimp);
        Destroy(gameObject);
    }

}
