using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class ShopGrid : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public int gridLength;
    public RoomGridNode[][][] grid;
    public float pointDistance;
    public float pointSize;
    public float invalidPointSize;
    Vector3 startPoint;

    [Header("Room Construction")]
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject ceilingPrefab;
    [SerializeField] GameObject wallPrefab;



    [Header("Debugging")]
    [SerializeField] bool debugGrid;
    [SerializeField] GameObject gridPointPrefab;
    private GameObject gridParent;



    // Generate a grid
    // Set all of the ground tiles
    // Spawn floor on each of those
    // Set all of the wall tiles 
    // Spawn wall on each of those
    // Set all of the ceiling tiles
    // Spawn ceiling on each of those



    private void Awake()
    {
        if (debugGrid)
        {
            gridParent = new GameObject("Grid Debug");
            gridParent.transform.parent = transform;
        }

        InitializeGrid();
        ConstructRoom();
    }


    private void AddNeighbour(GridNode p, Vector3Int neighbour)
    {
        if (neighbour.x > -1 && neighbour.x < gridWidth &&
            neighbour.y > -1 && neighbour.y < gridHeight &&
            neighbour.z > -1 && neighbour.z < gridLength)
        {
            p.neighbours.Add(neighbour);
        }
    }


    public void InitializeGrid()
    {
        //startPoint = new Vector3(-gridWidth, -gridHeight, -gridLength) / 2f * pointDistance + transform.position;  // Start generating from center
        startPoint = transform.position;  // Start generating from corner

        grid = new RoomGridNode[gridWidth][][];
        for (int w = 0; w < gridWidth; w++)
        {
            grid[w] = new RoomGridNode[gridHeight][];
            for (int h = 0; h < gridHeight; h++)
            {
                grid[w][h] = new RoomGridNode[gridLength];
                for (int l = 0; l < gridLength; l++)
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
                                {
                                    continue;
                                }
                                AddNeighbour(grid[w][h][l], new Vector3Int(w + p, h + q, l + g));
                            }
                        }
                    }
                }
            }
        }
    }


    private void CheckNodeValidity(GridNode node)
    {
        node.invalid = false;

        LayerMask layer = LayerMask.GetMask("Decoration");
        if (Physics.CheckBox(node.worldPos, Vector3.one * pointDistance / 2f, Quaternion.identity, layer, QueryTriggerInteraction.Ignore))
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
    }


    private void CheckNodePosition(int w, int h, int l)
    {
        grid[w][h][l].floor = (h == 0);
        grid[w][h][l].ceiling = (h == gridHeight - 1);

        grid[w][h][l].nWall = (l == gridLength - 1);  // North
        grid[w][h][l].eWall = (w == gridWidth - 1);  // East
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
        if (gridParent == null) return;

        // Remove the old room
        while (gridParent.transform.childCount > 0)
        {
            DestroyImmediate(gridParent.transform.GetChild(0).gameObject);
        }

        // Create the new room
        for (int w = 0; w < gridWidth; w++)
        {
            for (int h = 0; h < gridHeight; h++)
            {
                for (int l = 0; l < gridLength; l++)
                {
                    if (grid[w][h][l].floor) GameObject.Instantiate(floorPrefab, grid[w][h][l].worldPos, Quaternion.identity, gridParent.transform);
                    if (grid[w][h][l].ceiling) GameObject.Instantiate(ceilingPrefab, grid[w][h][l].worldPos, Quaternion.identity, gridParent.transform);

                    if (grid[w][h][l].nWall) GameObject.Instantiate(wallPrefab, grid[w][h][l].worldPos, Quaternion.Euler(new Vector3(0, 180, 0)), gridParent.transform);
                    if (grid[w][h][l].eWall) GameObject.Instantiate(wallPrefab, grid[w][h][l].worldPos, Quaternion.Euler(new Vector3(0, 270, 0)), gridParent.transform);
                    if (grid[w][h][l].sWall) GameObject.Instantiate(wallPrefab, grid[w][h][l].worldPos, Quaternion.Euler(new Vector3(0, 0, 0)), gridParent.transform);
                    if (grid[w][h][l].wWall) GameObject.Instantiate(wallPrefab, grid[w][h][l].worldPos, Quaternion.Euler(new Vector3(0, 90, 0)), gridParent.transform);

                }
            }
        }
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


        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                for (int k = 0; k < gridLength; k++)
                {
                    CheckNodeValidity(grid[i][j][k]);
                }
            }
        }
    }


    public List<GridNode> CheckForObjectCollisions(Transform[] objects)
    {
        List<GridNode> collidingNodes = new List<GridNode>();
        LayerMask layer = LayerMask.GetMask("Decoration");
        RaycastHit[] hit;

        // Nodes
        for (int i = 0; i < gridWidth; i++)
        {
            int j = DecorateTankController.Instance.editingLayer;
            for (int k = 0; k < gridLength; k++)
            {
                //if (!grid[i][j][k].invalid) continue;

                hit = Physics.BoxCastAll(grid[i][j][k].worldPos, Vector3.one * pointDistance / 2f, Vector3.up, Quaternion.identity, 0.01f, layer, QueryTriggerInteraction.Ignore);
                foreach (RaycastHit h in hit)
                {
                    if (objects.Contains(h.transform))
                    {
                        collidingNodes.Add(grid[i][j][k]);
                        break;
                    }
                }
            }
        }



        GridNode wall = new GridNode();
        wall.invalid = true;

        // Back
        Vector3 pos = transform.position + new Vector3(0, 0, (gridLength + 1) / 2) * pointDistance;
        Vector3 r = pos - transform.position;  // Get the relative vector from point to pivot
        r = transform.rotation * r;  // Rotate around the point
        pos = transform.position + r;  // Return to world space
        hit = Physics.BoxCastAll(pos, new Vector3(gridWidth * 2, gridHeight * 2, 0.1f) * pointDistance / 2f, transform.up, transform.rotation, 1, layer, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit h in hit)
            if (objects.Contains(h.transform))
                collidingNodes.Add(wall);

        // Front
        pos = transform.position + new Vector3(0, 0, (-gridLength - 2) / 2) * pointDistance;
        r = pos - transform.position;  // Get the relative vector from point to pivot
        r = transform.rotation * r;  // Rotate around the point
        pos = transform.position + r;  // Return to world space
        hit = Physics.BoxCastAll(pos, new Vector3(gridWidth * 2, gridHeight * 2, 0.1f) * pointDistance / 2f, transform.up, transform.rotation, 1, layer, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit h in hit)
            if (objects.Contains(h.transform))
                collidingNodes.Add(wall);

        // Right
        pos = transform.position + new Vector3((gridWidth + 1) / 2, 0, 0) * pointDistance;
        r = pos - transform.position;  // Get the relative vector from point to pivot
        r = transform.rotation * r;  // Rotate around the point
        pos = transform.position + r;  // Return to world space
        hit = Physics.BoxCastAll(pos, new Vector3(0.1f, gridHeight * 2, gridLength * 2) * pointDistance / 2f, transform.up, transform.rotation, 1, layer, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit h in hit)
            if (objects.Contains(h.transform))
                collidingNodes.Add(wall);

        // Left
        pos = transform.position + new Vector3((-gridWidth - 2) / 2, 0, 0) * pointDistance;
        r = pos - transform.position;  // Get the relative vector from point to pivot
        r = transform.rotation * r;  // Rotate around the point
        pos = transform.position + r;  // Return to world space
        hit = Physics.BoxCastAll(pos, new Vector3(0.1f, gridHeight * 2, gridLength * 2) * pointDistance / 2f, transform.up, transform.rotation, 1, layer, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit h in hit)
            if (objects.Contains(h.transform))
                collidingNodes.Add(wall);


        return collidingNodes;
    }


    public List<GameObject> CheckNodeForObject(GridNode node)
    {
        if (node == null) return null;
        List<GameObject> collidingObjects = new List<GameObject>();
        LayerMask layer = LayerMask.GetMask("Decoration");
        RaycastHit[] hit;

        hit = Physics.BoxCastAll(node.worldPos, Vector3.one * pointDistance / 2f, Vector3.up, Quaternion.identity, 0.01f, layer, QueryTriggerInteraction.Ignore);
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
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    for (int k = 0; k < gridLength; k++)
                    {
                        Gizmos.DrawWireCube(grid[i][j][k].worldPos, new Vector3(pointSize, pointSize, pointSize));
                    }
                }
            }
        }
    }


    public GridNode GetClosestNode(Vector3 position)
    {
        float sizeX = pointDistance * gridWidth;
        float sizeY = pointDistance * gridHeight;
        float sizeZ = pointDistance * gridLength;

        Vector3 pos = position - startPoint;
        float percentageX = Mathf.Clamp01(pos.x / sizeX);
        float percentageY = Mathf.Clamp01(pos.y / sizeY);
        float percentageZ = Mathf.Clamp01(pos.z / sizeZ);
        int x = Mathf.Clamp(Mathf.RoundToInt(percentageX * gridWidth), 0, gridWidth - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(percentageY * gridHeight), 0, gridHeight - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt(percentageZ * gridLength), 0, gridLength - 1);
        GridNode result = grid[x][y][z];
        int step = 1;
        while (result.invalid)
        {
            List<GridNode> freePoints = new List<GridNode>();
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
                        if (i > -1 && i < gridWidth &&
                            j > -1 && j < gridHeight &&
                            k > -1 && k < gridLength)
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
                //Debug.Log("Step - " + step);
                step++;
            }
        }
        return result;
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