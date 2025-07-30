using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Body : PartScript
{
    public Transform headNode, tailNode;
    //[SerializeField] private bool debug = false;

    

    

    public Body Construct(ShrimpStats s, ref TFan tFan, ref Tail tail, ref Head head, ref Eyes eyes)
    {
        this.s = s;

        SetMaterials(GeneManager.instance.GetTraitSO(s.body.activeGene.ID).set);

        head = Instantiate(GeneManager.instance.GetTraitSO(s.head.activeGene.ID).part, headNode).GetComponent<Head>().Construct(s, ref eyes);
        tail = Instantiate(GeneManager.instance.GetTraitSO(s.tail.activeGene.ID).part, tailNode).GetComponent<Tail>().Construct(s, ref tFan);

        


        return this;
    }

    public void ChangeColours(ColourTypes colour)
    {
        

        SetColour(colour);
    }

    
}
