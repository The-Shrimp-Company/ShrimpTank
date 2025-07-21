using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankGrid : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public int gridLength;
    public GridNode[][][] grid;
    public float pointDistance;
    public float pointSize;
    public float invalidPointSize;
    Vector3 startPoint;

    [Header("Decorating")]
    public GameObject decoratingGridPrefab;
    public Material decoratingGridMat;
    public Material decoratingGridValidMat;
    public Material decoratingGridInvalidMat;

    [Header("Debugging")]
    [SerializeField] bool debugGrid;
    [SerializeField] GameObject gridPointPrefab;


    private void Awake()
    {
        InitializeGrid();
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
        startPoint = new Vector3(-gridWidth, -gridHeight, -gridLength) / 2f * pointDistance + transform.position;

        GameObject gridParent = null;
        if (debugGrid)
        {
            gridParent = new GameObject("Grid Debug");
            gridParent.transform.parent = transform;
        }

        grid = new GridNode[gridWidth][][];
        for (int i = 0; i < gridWidth; i++)
        {
            grid[i] = new GridNode[gridHeight][];
            for (int j = 0; j < gridHeight; j++)
            {
                grid[i][j] = new GridNode[gridLength];
                for (int k = 0; k < gridLength; k++)
                {
                    Vector3 pos = startPoint + new Vector3(i, j, k) * pointDistance;
                    grid[i][j][k] = new GridNode();
                    grid[i][j][k].coords = new Vector3Int(i, j, k);
                    grid[i][j][k].worldPos = pos;
                    LayerMask layer = LayerMask.GetMask("Decoration");
                    if (Physics.CheckBox(grid[i][j][k].worldPos, Vector3.one * pointDistance / 2f, Quaternion.identity, layer, QueryTriggerInteraction.Ignore))
                    {
                        grid[i][j][k].invalid = true;

                        if (debugGrid)
                        {
                            GameObject invalidCube = Instantiate(gridPointPrefab, grid[i][j][k].worldPos, Quaternion.identity);
                            invalidCube.transform.parent = gridParent.transform;
                            invalidCube.transform.localScale = new Vector3(pointSize, pointSize, pointSize);
                        }
                    }
                    for (int p = -1; p <= 1; p++)
                    {
                        for (int q = -1; q <= 1; q++)
                        {
                            for (int g = -1; g <= 1; g++)
                            {
                                if (i == p && g == q && k == g)
                                {
                                    continue;
                                }
                                AddNeighbour(grid[i][j][k], new Vector3Int(i + p, j + q, k + g));
                            }
                        }
                    }
                }
            }
        }
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