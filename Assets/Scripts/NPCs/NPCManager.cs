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
            NPCs.Add(new Alan());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (count >= NPCs.Count) count = 0;

        NPCs[count].NpcCheck();

        count++;
    }
}
