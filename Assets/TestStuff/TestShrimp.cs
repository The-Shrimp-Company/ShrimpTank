using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShrimp : MonoBehaviour
{
    public Transform shrimpModel;

    // Start is called before the first frame update
    void Start()
    {
        ShrimpStats s = ShrimpManager.instance.CreatePureBreedShrimp(TraitSet.Cherry, true);
        GameObject newShrimp = Instantiate(GeneManager.instance.GetTraitSO(s.legs.activeGene.ID).part, shrimpModel);
        newShrimp.GetComponent<Legs>().Construct(s);
    }

    
}
