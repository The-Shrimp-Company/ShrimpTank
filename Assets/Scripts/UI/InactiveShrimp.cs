using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class InactiveShrimp : MonoBehaviour
{
    [SerializeField] private Transform shrimpModel;

    public void Construct(ShrimpStats s)
    {
        GameObject newShrimp = Instantiate(GeneManager.instance.GetTraitSO(s.legs.activeGene.ID).part, shrimpModel);
        newShrimp.GetComponent<Legs>().Construct(s);
        //newShrimp.transform.SetLayerRecursively(LayerMask.NameToLayer("ShrimpUI"));
    }
}
