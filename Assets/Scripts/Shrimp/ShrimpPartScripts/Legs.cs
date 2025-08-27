using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Legs : PartScript
{
    public Transform bodyNode;
    [SerializeField] private bool debug = false;

    [HideInInspector]
    public Body body;
    private Tail tail;
    private TFan tFan;
    private Head head;
    private Eyes eyes;


    private void Start()
    {
        if (debug)
        {
            s = ShrimpManager.instance.CreateRandomShrimp(false);

            SetMaterials(GeneManager.instance.GetTraitSO(s.legs.activeGene.ID).set);

            body = Instantiate(GeneManager.instance.GetTraitSO(s.body.activeGene.ID).part, bodyNode).GetComponent<Body>().Construct(s, ref tFan, ref tail, ref head, ref eyes);

            SetAnimation(AnimNames.swimming);

            body.GetComponent<LODGroup>().localReferencePoint = body.transform.InverseTransformPoint(transform.TransformPoint(GetComponent<LODGroup>().localReferencePoint));
            body.GetComponent<LODGroup>().size = GetComponent<LODGroup>().size;
            tail.GetComponent<LODGroup>().localReferencePoint = tail.transform.InverseTransformPoint(transform.TransformPoint(GetComponent<LODGroup>().localReferencePoint));
            tail.GetComponent<LODGroup>().size = GetComponent<LODGroup>().size;
            tFan.GetComponent<LODGroup>().localReferencePoint = tFan.transform.InverseTransformPoint(transform.TransformPoint(GetComponent<LODGroup>().localReferencePoint));
            tFan.GetComponent<LODGroup>().size = GetComponent<LODGroup>().size;
            head.GetComponent<LODGroup>().localReferencePoint = head.transform.InverseTransformPoint(transform.TransformPoint(GetComponent<LODGroup>().localReferencePoint));
            head.GetComponent<LODGroup>().size = GetComponent<LODGroup>().size;
            eyes.GetComponent<LODGroup>().localReferencePoint = eyes.transform.InverseTransformPoint(transform.TransformPoint(GetComponent<LODGroup>().localReferencePoint));
            eyes.GetComponent<LODGroup>().size = GetComponent<LODGroup>().size;
        }
    }

    

    public void Construct(ShrimpStats s)
    {
        this.s = s;

        SetMaterials(GeneManager.instance.GetTraitSO(s.legs.activeGene.ID).set);

        body = Instantiate(GeneManager.instance.GetTraitSO(s.body.activeGene.ID).part, bodyNode).GetComponent<Body>().Construct(s, ref tFan, ref tail, ref head, ref eyes);

        SetAnimation(AnimNames.swimming);

        SetLodLevels(transform.TransformPoint(GetComponent<LODGroup>().localReferencePoint), GetComponent<LODGroup>().size);
    }

    public void SetLodLevels(Vector3 point, float size = 1)
    {
        body.GetComponent<LODGroup>().localReferencePoint = body.transform.InverseTransformPoint(point);
        body.GetComponent<LODGroup>().size = size;
        tail.GetComponent<LODGroup>().localReferencePoint = tail.transform.InverseTransformPoint(point);
        tail.GetComponent<LODGroup>().size = size;
        tFan.GetComponent<LODGroup>().localReferencePoint = tFan.transform.InverseTransformPoint(point);
        tFan.GetComponent<LODGroup>().size = size;
        head.GetComponent<LODGroup>().localReferencePoint = head.transform.InverseTransformPoint(point);
        head.GetComponent<LODGroup>().size = size;
        eyes.GetComponent<LODGroup>().localReferencePoint = eyes.transform.InverseTransformPoint(point);
        eyes.GetComponent<LODGroup>().size = size;
    }

    public void ForceLod(int index)
    {
        if (!body || !tail || !tFan || !head || !eyes) return;
        body.GetComponent<LODGroup>().ForceLOD(index);
        tail.GetComponent<LODGroup>().ForceLOD(index);
        tFan.GetComponent<LODGroup>().ForceLOD(index);
        head.GetComponent<LODGroup>().ForceLOD(index);
        eyes.GetComponent<LODGroup>().ForceLOD(index);
        GetComponent<LODGroup>().ForceLOD(index);
    }

    public void ChangeColours(ColourTypes colour)
    {
        head.ChangeColours(colour);
        body.ChangeColours(colour);
        tail.ChangeColours(colour);

        SetColour(colour);
    }

    public void SetAnimation(AnimNames anim)
    {
        /*
        head.StartAnimation(anim);
        StartAnimation(anim);
        tail.StartAnimation(anim);
        //tFan.StartAnimation(anim);
        */
    }
}
