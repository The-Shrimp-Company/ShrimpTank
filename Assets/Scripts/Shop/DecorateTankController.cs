using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;

public class DecorateTankController : MonoBehaviour
{
    public static DecorateTankController Instance;

    private TankController currentTank;
    private TankGrid currentGrid;

    private Dictionary<GridNode, GameObject> bottomNodes = new Dictionary<GridNode, GameObject>();
    private GridNode hoveredNode, selectedNode;
    private GameObject selectedObject, objectPreview;

    private bool selectionValid;


    [Header("Grid")]
    public GameObject decoratingGridPrefab;
    public Material decoratingGridMat;
    public Material decoratingGridHovered;
    public Material decoratingGridValidMat;
    public Material decoratingGridInvalidMat;
    public Material objectPreviewValidMat;
    public Material objectPreviewInvalidMat;

    [Header("Debug")]
    public bool showNodes = true;
    public GameObject testItem;



    public void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) { Destroy(gameObject); }
    }

    public void StartDecorating(TankController t)
    {
        if (t == null) return;
        if (t.tankViewScript == null) return;
        currentTank = t;
        currentGrid = t.tankGrid;

        GameObject newMenu = GameObject.Instantiate(t.tankViewScript.tankDecorateView, t.transform);
        UIManager.instance.OpenScreen(newMenu.GetComponent<ScreenView>());
        newMenu.GetComponent<TankDecorateViewScript>().UpdateContent();
        newMenu.GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
        newMenu.GetComponent<Canvas>().planeDistance = 1;
        UIManager.instance.SetCursorMasking(false);

        Camera.main.transform.position = currentTank.GetDecorationCam().transform.position;
        Camera.main.transform.rotation = currentTank.GetDecorationCam().transform.rotation;

        currentTank.waterObject.SetActive(false);



        int h = 0;  // Height
        for (int w = 0; w < currentGrid.gridWidth; w++)
        {
            for (int l = 0; l < currentGrid.gridLength; l++)
            {
                GameObject node = GameObject.Instantiate(decoratingGridPrefab, currentGrid.grid[w][h][l].worldPos + new Vector3(0, -(currentGrid.pointSize / 2), 0), Quaternion.identity);
                node.transform.parent = currentTank.transform;
                node.transform.localScale = new Vector3(currentGrid.pointSize, currentGrid.pointSize / 10, currentGrid.pointSize);
                node.GetComponent<MeshRenderer>().enabled = showNodes;
                bottomNodes.Add(currentGrid.grid[w][h][l], node);
            }
        }

        StartPlacing(testItem);
    }


    public void StopDecorating()
    {
        currentTank.waterObject.SetActive(true);

        currentTank = null;
        currentGrid = null;

        hoveredNode = null;
        selectedNode = null;

        if (objectPreview)
            Destroy(objectPreview);


        foreach(GameObject n in bottomNodes.Values)
        {
            GameObject.Destroy(n);
        }
        bottomNodes.Clear();
    }





    public void Update()
    {
        if (currentTank == null) return;
        if (currentGrid == null) return;

        MouseDetection();
    }


    public void MouseDetection()
    {
        RaycastHit ray;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray originMouse = Camera.main.ScreenPointToRay(mousePos);
        int layerMask = LayerMask.GetMask("GridNode");
        if (Physics.Raycast(originMouse, out ray, 3f, layerMask))
        {
            if (hoveredNode != null && bottomNodes[hoveredNode] == ray.collider.gameObject) return;


            foreach (GridNode node in bottomNodes.Keys)
            {
                if (bottomNodes[node] == ray.collider.gameObject)
                {
                    ChangeHoveredNode(node);
                    break;
                }
            }
        }
        else
        {
            if (hoveredNode != null) ChangeHoveredNode(null);
        }
    }


    private void ChangeHoveredNode(GridNode node)
    {
        foreach (GridNode n in bottomNodes.Keys)
        {
            if (n.invalid) bottomNodes[n].GetComponent<MeshRenderer>().material = decoratingGridInvalidMat;
            else bottomNodes[n].GetComponent<MeshRenderer>().material = decoratingGridMat;
        }

        hoveredNode = node;
        if (hoveredNode != null)
        {
            bottomNodes[hoveredNode].GetComponent<MeshRenderer>().material = decoratingGridHovered;

            objectPreview.transform.position = hoveredNode.worldPos;
        }
        else
        {
            objectPreview.transform.position = new Vector3(0, 100000, 0);
        }


        CheckPlacementValidity();
    }


    public void MouseClick(Vector3 point, bool pressed)
    {
        if (hoveredNode == null) return;
        if (selectedObject == null) return;

        if (selectionValid)
        {
            GameObject t = GameObject.Instantiate(selectedObject, hoveredNode.worldPos, Quaternion.identity);
            t.transform.localScale = new Vector3(currentGrid.pointSize * 2f, currentGrid.pointSize * 2f, currentGrid.pointSize * 2f);
            currentGrid.RebakeGrid();
        }
    }


    private void StartPlacing(GameObject d)
    {
        selectedObject = d;

        objectPreview = GameObject.Instantiate(selectedObject);
        objectPreview.name = "Object Preview";
        objectPreview.transform.localScale = new Vector3(currentGrid.pointSize * 2f, currentGrid.pointSize * 2f, currentGrid.pointSize * 2f);
        SetObjectMaterials(objectPreview, objectPreviewValidMat);
    }


    private void StopPlacing()
    {
        if (objectPreview) Destroy(objectPreview);
    }


    private void SetObjectMaterials(GameObject obj, Material mat)
    {
        if (obj == null) return;
        if (mat == null) return;

        MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();

        if (meshes[0] == null || meshes[0].material == mat) return;

        foreach (MeshRenderer me in meshes)
        {
            var materials = me.materials;

            for (var i = 0; i < materials.Length; i++)
            {
                materials[i] = mat;
            }

            me.materials = materials;
        }
    }


    public void CheckPlacementValidity()
    {
        if (objectPreview == null) return;
        if (currentGrid == null) return;

        // Sphere check with all nodes to see if it overlaps

        List<GridNode> nodes = currentGrid.CheckForObjectCollisions(objectPreview);
        if (nodes.Count == 0)
        {
            selectionValid = true;
        }
        else
        {
            selectionValid = false;
        }

        SetObjectMaterials(objectPreview, selectionValid ? objectPreviewValidMat : objectPreviewInvalidMat);


        // Box check sides of tank


        // Overlapped nodes added to list


        // Valid ones are blue, invalid are red


        // If no invalid then it can be placed
    }
}
