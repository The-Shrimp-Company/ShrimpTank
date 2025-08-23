using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DecorateShopController))]
public class Store : MonoBehaviour
{
    public static string StoreName;
    public static DecorateShopController decorateController;

    void Awake()
    {
        decorateController = GetComponent<DecorateShopController>();
    }
}
