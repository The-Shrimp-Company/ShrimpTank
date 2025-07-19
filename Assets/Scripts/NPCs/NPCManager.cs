using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    public List<NPC> NPCs = new List<NPC>();

    private int count = 0;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            NPCs.Add(new Admin());
            NPCs.Add(new Alan());
            NPCs.Add(new CollectorTom());
            NPCs.Add(new SleazyJoe());
            NPCs.Add(new Rival());
            //NPCs.Add(new DebugNPC());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Tutorial.instance.flags.activeAccount)
        {
            NPCs[0].NpcCheck();
            Debug.Log("Doing This");
        }
        else
        {
            if (count >= NPCs.Count) count = 0;

            NPCs[count].NpcCheck();

            count++;
        }
    }
}