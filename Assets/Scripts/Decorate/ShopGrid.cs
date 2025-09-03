using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopGrid : MonoBehaviour
{
    public static ShopGrid Instance;

    public Vector3 startingRoomSize;
    [HideInInspector] public Vector3 roomSize;
    private Vector3 minRoomSize = new Vector3(3, 3, 3);

    public RoomGridNode[][][] grid;
    public float pointDistance;
    public float pointSize;
    public float invalidPointSize;
    Vector3 startPoint;
    private Transform player;

    [Header("Room Construction")]
    [SerializeField] GameObject roomParent;
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject ceilingPrefab;
    [SerializeField] GameObject wallPrefab;



    [Header("Debugging")]
    [SerializeField] bool debugGrid;
    [SerializeField] GameObject gridPointPrefab;
    private GameObject gridParent;



    // Could change wall detection to tiles without a neighbour on that side?



    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) { Destroy(gameObject); }

        if (debugGrid)
        {
            gridParent = new GameObject("Grid Debug");
            gridParent.transform.parent = transform;
        }

        player = GameObject.Find("Player").transform;

        roomSize = startingRoomSize;

        InitializeGrid();
        ConstructRoom();
    }


    private void AddNeighbour(GridNode p, Vector3Int neighbour)
    {
        if (neighbour.x > -1 && neighbour.x < roomSize.x &&
            neighbour.y > -1 && neighbour.y < roomSize.y &&
            neighbour.z > -1 && neighbour.z < roomSize.z)
        {
            p.neighbours.Add(neighbour);
        }
    }


    public void InitializeGrid()
    {
        //startPoint = new Vector3(-gridWidth, -gridHeight, -gridLength) / 2f * pointDistance + transform.position;  // Start generating from center
        startPoint = transform.position;  // Start generating from corner

        grid = new RoomGridNode[(int)roomSize.x][][];
        for (int w = 0; w < roomSize.x; w++)
        {
            grid[w] = new RoomGridNode[(int)roomSize.y][];
            for (int h = 0; h < roomSize.y; h++)
            {
                grid[w][h] = new RoomGridNode[(int)roomSize.z];
                for (int l = 0; l < roomSize.z; l++)
                {
                    Vector3 pos = startPoint + new Vector3(w, h, l) * pointDistance;
                    Vector3 r = pos - transform.position;  // Get the relative vector from point to pivot
                    r = transform.rotation * r;  // Rotate around the point
                    pos = transform.position + r;  // Return to world space

                    grid[w][h][l] = new RoomGridNode();
                    grid[w][h][l].coords = new Vector3Int(w, h, l);
                    grid[w][h][l].worldPos = pos;

                    CheckNodeValidity(grid[w][h][l]);

                    CheckNodePosition(w, h, l);

                    for (int p = -1; p <= 1; p++)
                    {
                        for (int q = -1; q <= 1; q++)
                        {
                            for (int g = -1; g <= 1; g++)
                            {
                                if (w == p && g == q && l == g)
                                    continue;
                                if (p == 0 && q == -1 && g == 0 && h != 0)
                                    grid[w][h][l].nodeBelow = grid[w][h - 1][l];
                                AddNeighbour(grid[w][h][l], new Vector3Int(w + p, h + q, l + g));
                            }
                        }
                    }
                }
            }
        }
    }


    private void CheckNodeValidity(RoomGridNode node)
    {
        node.invalid = false;
        node.shelf = false;

        LayerMask layer = LayerMask.GetMask("RoomDecoration") | LayerMask.GetMask("Shelf");
        RaycastHit[] hit;

        hit = Physics.BoxCastAll(node.worldPos, Vector3.one * pointDistance / 2f, Vector3.up, Quaternion.identity, 0.01f, layer, QueryTriggerInteraction.Collide);

        foreach (RaycastHit h in hit)
        {
            if (h.transform.gameObject.layer == LayerMask.NameToLayer("RoomDecoration"))
            {
                node.invalid = true;

                if (debugGrid && gridParent != null)
                {
                    GameObject invalidCube = Instantiate(gridPointPrefab, node.worldPos, Quaternion.identity);
                    invalidCube.transform.parent = gridParent.transform;
                    invalidCube.transform.rotation = transform.rotation;
                    invalidCube.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
                }
            }
            else if (h.transform.gameObject.layer == LayerMask.NameToLayer("Shelf"))
            {
                node.shelf = true;
                foreach(ShelfGridNode sgn in h.transform.gameObject.GetComponentsInChildren<ShelfGridNode>())
                {
                    sgn.CheckNodeValidity();
                }

                if (debugGrid && gridParent != null)
                {
                    GameObject invalidCube = Instantiate(gridPointPrefab, node.worldPos, Quaternion.identity);
                    invalidCube.transform.parent = gridParent.transform;
                    invalidCube.transform.rotation = transform.rotation;
                    invalidCube.transform.localScale = new Vector3(pointSize / 2, pointSize / 2, pointSize / 2);
                }
            }
        }
    }


    private void CheckNodePosition(int w, int h, int l)
    {
        if (grid[w][h][l] == null) return;

        grid[w][h][l].floor = (h == 0);
        grid[w][h][l].ceiling = (h == roomSize.y - 1);

        grid[w][h][l].nWall = (l == roomSize.z - 1);  // North
        grid[w][h][l].eWall = (w == roomSize.x - 1);  // East
        grid[w][h][l].sWall = (l == 0);  // South
        grid[w][h][l].wWall = (w == 0);  // West


        grid[w][h][l].wall = (
            grid[w][h][l].nWall ||
            grid[w][h][l].eWall ||
            grid[w][h][l].sWall ||
            grid[w][h][l].wWall);

    }


    private void ConstructRoom()
    {
        if (roomParent == null) return;

        // Remove the old room
        while (roomParent.transform.childCount > 0)
        {
            DestroyImmediate(roomParent.transform.GetChild(0).gameObject);
        }

        // Create the new room
        for (int w = 0; w < roomSize.x; w++)
        {
            for (int h = 0; h < roomSize.y; h++)
            {
                for (int l = 0; l < roomSize.z; l++)
                {
                    if (grid[w][h][l].floor) GameObject.Instantiate(floorPrefab, grid[w][h][l].worldPos, Quaternion.identity, roomParent.transform);
                    if (grid[w][h][l].ceiling) GameObject.Instantiate(ceilingPrefab, grid[w][h][l].worldPos, Quaternion.identity, roomParent.transform);

                    if (grid[w][h][l].nWall) GameObject.Instantiate(wallPrefab, grid[w][h][l].worldPos, Quaternion.Euler(new Vector3(0, 180, 0)), roomParent.transform);
                    if (grid[w][h][l].eWall) GameObject.Instantiate(wallPrefab, grid[w][h][l].worldPos, Quaternion.Euler(new Vector3(0, 270, 0)), roomParent.transform);
                    if (grid[w][h][l].sWall) GameObject.Instantiate(wallPrefab, grid[w][h][l].worldPos, Quaternion.Euler(new Vector3(0, 0, 0)), roomParent.transform);
                    if (grid[w][h][l].wWall) GameObject.Instantiate(wallPrefab, grid[w][h][l].worldPos, Quaternion.Euler(new Vector3(0, 90, 0)), roomParent.transform);

                }
            }
        }
    }


    public void IncreaseRoomSize(Vector3 increase)
    {
        roomSize.x = Mathf.Clamp(roomSize.x + increase.x, minRoomSize.x, Mathf.Infinity);
        roomSize.y = Mathf.Clamp(roomSize.y + increase.y, minRoomSize.y, Mathf.Infinity);
        roomSize.z = Mathf.Clamp(roomSize.z + increase.z, minRoomSize.z, Mathf.Infinity);

        InitializeGrid();

        ConstructRoom();
    }


    public void RebakeGrid()
    {
        if (debugGrid && gridParent != null)
        {
            foreach (MeshRenderer m in gridParent.GetComponentsInChildren<MeshRenderer>())
            {
                Destroy(m.gameObject);
            }
        }


        for (int i = 0; i < roomSize.x; i++)
        {
            for (int j = 0; j < roomSize.y; j++)
            {
                for (int k = 0; k < roomSize.z; k++)
                {
                    CheckNodeValidity(grid[i][j][k]);
                }
            }
        }
    }


    public List<RoomGridNode> CheckForObjectCollisions(Transform[] objects, bool ignoreShelves)
    {
        List<RoomGridNode> collidingNodes = new List<RoomGridNode>();
        LayerMask layer;
        if (ignoreShelves) layer = LayerMask.GetMask("RoomDecoration");
        else layer = LayerMask.GetMask("RoomDecoration") | LayerMask.GetMask("Shelf");
        RaycastHit[] hit;

        // Nodes
        for (int w = 0; w < roomSize.x; w++)
        {
            for (int h = 0; h < roomSize.y; h++)
            {
                for (int l = 0; l < roomSize.z; l++)
                {
                    hit = Physics.BoxCastAll(grid[w][h][l].worldPos, Vector3.one * pointDistance / 2f, Vector3.up, Quaternion.identity, 0.01f, layer, QueryTriggerInteraction.Collide);
                    foreach (RaycastHit node in hit)
                    {
                        if (objects.Contains(node.transform))
                        {
                            collidingNodes.Add(grid[w][h][l]);
                            break;
                        }
                    }
                }
            }
        }



        RoomGridNode wall = new RoomGridNode();
        wall.invalid = true;

        // Back
        Vector3 pos = transform.position + new Vector3(roomSize.x / 2, 0, -1) * pointDistance;
        hit = Physics.BoxCastAll(pos, new Vector3(roomSize.x * 2, roomSize.y, 0.1f) * pointDistance / 2f, transform.up, transform.rotation, 1, layer, QueryTriggerInteraction.Collide);
        foreach (RaycastHit h in hit)
            if (objects.Contains(h.transform))
                collidingNodes.Add(wall);


        // Front
        pos = transform.position + new Vector3(roomSize.x / 2, 0, roomSize.z) * pointDistance;
        hit = Physics.BoxCastAll(pos, new Vector3(roomSize.x * 2, roomSize.y, 0.1f) * pointDistance / 2f, transform.up, transform.rotation, 1, layer, QueryTriggerInteraction.Collide);
        foreach (RaycastHit h in hit)
            if (objects.Contains(h.transform))
                collidingNodes.Add(wall);

        // Right
        pos = transform.position + new Vector3(roomSize.x, 0, roomSize.z / 2) * pointDistance;
        hit = Physics.BoxCastAll(pos, new Vector3(0.1f, roomSize.y, roomSize.z * 2) * pointDistance / 2f, transform.up, transform.rotation, 1, layer, QueryTriggerInteraction.Collide);
        foreach (RaycastHit h in hit)
            if (objects.Contains(h.transform))
                collidingNodes.Add(wall);

        // Left
        pos = transform.position + new Vector3(-1, 0, roomSize.z / 2) * pointDistance;
        hit = Physics.BoxCastAll(pos, new Vector3(0.1f, roomSize.y, roomSize.z * 2) * pointDistance / 2f, transform.up, transform.rotation, 1, layer, QueryTriggerInteraction.Collide);
        foreach (RaycastHit h in hit)
            if (objects.Contains(h.transform))
                collidingNodes.Add(wall);


        return collidingNodes;
    }


    public List<GameObject> CheckNodeForObject(GridNode node)
    {
        if (node == null) return null;
        List<GameObject> collidingObjects = new List<GameObject>();
        LayerMask layer = LayerMask.GetMask("RoomDecoration") | LayerMask.GetMask("Shelf");
        RaycastHit[] hit;

        hit = Physics.BoxCastAll(node.worldPos, Vector3.one * pointDistance / 2f, Vector3.up, Quaternion.identity, 0.01f, layer, QueryTriggerInteraction.Collide);
        foreach (RaycastHit h in hit)
        {
            GameObject selection = h.transform.gameObject;
            while (selection.transform.parent != null && (layer & 1 << selection.transform.parent.gameObject.layer) == 1 << selection.transform.parent.gameObject.layer)  // Iterate up through parents to find the root of the decoration
            {
                selection = selection.transform.parent.gameObject;
            }

            if (!collidingObjects.Contains(selection))
                collidingObjects.Add(selection);
        }

        return collidingObjects;
    }


    public List<GameObject> CheckPlayerForObject()
    {
        if (player == null) return null;
        List<GameObject> collidingObjects = new List<GameObject>();
        LayerMask layer = LayerMask.GetMask("RoomDecoration") | LayerMask.GetMask("Shelf");
        RaycastHit[] hit;

        hit = Physics.BoxCastAll(player.position, Vector3.one * pointDistance / 2f, Vector3.up, Quaternion.identity, 0.01f, layer, QueryTriggerInteraction.Collide);
        foreach (RaycastHit h in hit)
        {
            GameObject selection = h.transform.gameObject;
            while (selection.transform.parent != null && (layer & 1 << selection.transform.parent.gameObject.layer) == 1 << selection.transform.parent.gameObject.layer)  // Iterate up through parents to find the root of the decoration
            {
                selection = selection.transform.parent.gameObject;
            }

            if (!collidingObjects.Contains(selection))
                collidingObjects.Add(selection);
        }

        return collidingObjects;
    }


    private void OnDrawGizmos()
    {
        if (grid != null && debugGrid)
        {
            for (int i = 0; i < roomSize.x; i++)
            {
                for (int j = 0; j < roomSize.y; j++)
                {
                    for (int k = 0; k < roomSize.z; k++)
                    {
                        Gizmos.DrawWireCube(grid[i][j][k].worldPos, new Vector3(pointSize, pointSize, pointSize));
                    }
                }
            }
        }
    }


    public RoomGridNode GetClosestNode(Vector3 position)
    {
        float sizeX = pointDistance * roomSize.x;
        float sizeY = pointDistance * roomSize.y;
        float sizeZ = pointDistance * roomSize.z;

        Vector3 pos = position - startPoint;
        float percentageX = Mathf.Clamp01(pos.x / sizeX);
        float percentageY = Mathf.Clamp01(pos.y / sizeY);
        float percentageZ = Mathf.Clamp01(pos.z / sizeZ);
        int x = Mathf.Clamp(Mathf.RoundToInt(percentageX * roomSize.x), 0, (int)roomSize.x - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(percentageY * roomSize.y), 0, (int)roomSize.y - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt(percentageZ * roomSize.z), 0, (int)roomSize.z - 1);
        RoomGridNode result = grid[x][y][z];
        int step = 1;
        while (result.invalid)
        {
            List<RoomGridNode> freePoints = new List<RoomGridNode>();
            for (int p = -step; p <= step; p++)
            {
                for (int q = -step; q <= step; q++)
                {
                    for (int g = -step; g <= step; g++)
                    {
                        if (x == p && y == q && z == g)
                        {
                            continue;
                        }
                        int i = x + p;
                        int j = y + q;
                        int k = z + g;
                        if (i > -1 && i < roomSize.x &&
                            j > -1 && j < roomSize.y &&
                            k > -1 && k < roomSize.z)
                        {
                            if (!grid[x + p][y + q][z + g].invalid)
                            {
                                freePoints.Add(grid[x + p][y + q][z + g]);
                            }
                        }
                    }
                }
            }

            float distance = Mathf.Infinity;
            for (int i = 0; i < freePoints.Count; i++)
            {
                float dist = (freePoints[i].worldPos - position).sqrMagnitude;
                if (dist < distance)
                {
                    result = freePoints[i];
                    dist = distance;
                }
            }

            if (freePoints.Count == 0)
            {
                step++;
            }
        }
        return result;
    }


    public RoomGridNode GetTankTeleportPosition(Vector3 position, int maxDistance)
    {
        float sizeX = pointDistance * roomSize.x;
        float sizeZ = pointDistance * roomSize.z;

        Vector3 pos = position - startPoint;
        float percentageX = Mathf.Clamp01(pos.x / sizeX);
        float percentageZ = Mathf.Clamp01(pos.z / sizeZ);
        int x = Mathf.Clamp(Mathf.RoundToInt(percentageX * roomSize.x), 0, (int)roomSize.x - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt(percentageZ * roomSize.z), 0, (int)roomSize.z - 1);
        RoomGridNode result = grid[x][0][z];
        int step = 1;
        while (result.invalid)
        {
            List<RoomGridNode> freePoints = new List<RoomGridNode>();
            for (int p = -step; p <= step; p++)
            {
                for (int g = -step; g <= step; g++)
                {
                    if (x == p && z == g)
                    {
                        continue;
                    }
                    int i = x + p;
                    int k = z + g;
                    if (i > -1 && i < roomSize.x &&
                        k > -1 && k < roomSize.z)
                    {
                        if (!grid[x + p][0][z + g].invalid)
                        {
                            freePoints.Add(grid[x + p][0][z + g]);
                        }
                    }
                }
            }

            float distance = Mathf.Infinity;
            for (int i = 0; i < freePoints.Count; i++)
            {
                float dist = (freePoints[i].worldPos - position).sqrMagnitude;
                if (dist < distance)
                {
                    result = freePoints[i];
                    dist = distance;
                }
            }

            if (freePoints.Count == 0)
            {
                if (step == maxDistance)
                    return null;

                step++;
            }
        }
        return result;
    }


    public RoomGridNode GetLowestNode(RoomGridNode node)
    {
        if (node == null) return null;

        while (node.nodeBelow != null)
        {
            node = node.nodeBelow;
        }

        return node;
    }


    public List<GridNode> GetPoints()  // Returns a list of all of the points
    {
        List<GridNode> points = new List<GridNode>();
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                for (int k = 0; k < grid[i][j].Length; k++)
                {
                    points.Add(grid[i][j][k]);
                }
            }
        }
        return points;
    }

    public List<GridNode> GetFreePoints()  // Returns a list of all of the free points
    {
        List<GridNode> freePoints = new List<GridNode>();
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                for (int k = 0; k < grid[i][j].Length; k++)
                {
                    if (!grid[i][j][k].invalid)
                    {
                        freePoints.Add(grid[i][j][k]);
                    }
                }
            }
        }
        return freePoints;
    }

    public List<GridNode> GetSurfacePoints()  // Returns a list of all of the free surface points
    {
        int topLayer = grid[0].Length - 1;
        List<GridNode> points = new List<GridNode>();
        for (int i = 0; i < grid.Length; i++)
        {
            for (int k = 0; k < grid[i][topLayer].Length; k++)
            {
                points.Add(grid[i][topLayer][k]);
            }
        }
        return points;
    }
}