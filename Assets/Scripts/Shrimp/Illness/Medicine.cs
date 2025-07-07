using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medicine : Item
{
    public IllnessSymptoms[] symptoms;
    [Range(0, 100)] public float strength = 100;  // 100 Will allways fully cure


}
