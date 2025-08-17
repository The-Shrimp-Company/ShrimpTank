using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridNode
{
    public Vector3Int coords;
    public Vector3 worldPos;
    public List<Vector3Int> neighbours;
    public bool invalid;
    public float distanceFactor = 0.5f;

    public GridNode()
    {
        neighbours = new List<Vector3Int>();
    }
}


[System.Serializable]
public class RoomGridNode : GridNode
{
    public bool floor;
    public bool ceiling;
    public bool wall;
    public bool nWall;
    public bool eWall;
    public bool sWall;
    public bool wWall;
}