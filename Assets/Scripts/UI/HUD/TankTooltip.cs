using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankController))]
public class TankTooltip : ToolTip
{
    void Start()
    {
        toolTip = GetComponent<TankController>().tankName;
    }
}
