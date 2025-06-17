using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC
{
    protected int reputation;
    protected int reliability;
    protected int completion;

    public virtual void NpcCheck()
    {
        Debug.LogWarning("Empty NPC Check called from:" + this.GetType());
    }
}
