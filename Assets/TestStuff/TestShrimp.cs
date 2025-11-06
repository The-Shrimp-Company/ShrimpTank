using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShrimp : MonoBehaviour
{
    public Transform shrimpModel;
    public TraitSet fullSet;
    public PartScript.AnimNames anim;

    // Start is called before the first frame update
    void Start()
    {
        ShrimpStats s;
        if(fullSet == TraitSet.None)
        {
            s = ShrimpManager.instance.CreateRandomShrimp(true);
        }
        else
        {
            s = ShrimpManager.instance.CreatePureBreedShrimp(fullSet, true);
        }
        GameObject newShrimp = Instantiate(GeneManager.instance.GetTraitSO(s.legs.activeGene.ID).part, shrimpModel);
        newShrimp.GetComponent<Legs>().Construct(s);
        newShrimp.GetComponent<Legs>().SetAnimation(anim);
    }

    
}
