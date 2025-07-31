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
    public TankDecorateViewScript decorateView;

    private Dictionary<GridNode, GameObject> bottomNodes = new Dictionary<GridNode, GameObject>();
    private GridNode hoveredNode, selectedNode;
    private GameObject selectedObject, objectPreview;

    private bool placementMode;
    private bool selectionValid;
    private float currentRotation;
    public float rotationSnap = 90;


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
        if (bottomNodes == null || bottomNodes.Count == 0) return;

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
                    hoveredNode = node;
                    break;
                }
            }
        }
        else
        {
            if (hoveredNode != null) hoveredNode = null;
        }

        UpdateGridMaterials();
    }


    private void UpdateGridMaterials()
    {
        if (!placementMode)
        {
            GreyOutGrid();

            // Raycast for objects first?

            foreach (GridNode n in bottomNodes.Keys)
            {
                if (n.invalid)
                    bottomNodes[n].GetComponent<MeshRenderer>().material = decoratingGridInvalidMat;
            }

            if (hoveredNode != null)
            {
                foreach (GameObject obj in currentGrid.CheckNodeForObject(hoveredNode))
                {
                    Transform[] transforms = obj.GetComponentsInChildren<Transform>();
                    foreach (GridNode n in currentGrid.CheckForObjectCollisions(transforms))
                        if (n.worldPos != null && n.worldPos != Vector3.zero)  // If it is not a wall
                            bottomNodes[n].GetComponent<MeshRenderer>().material = decoratingGridHovered;
                }
            }

            if (selectedObject != null)
            {
                Transform[] transforms = selectedObject.GetComponentsInChildren<Transform>();
                foreach (GridNode n in currentGrid.CheckForObjectCollisions(transforms))
                    if (n.worldPos != null && n.worldPos != Vector3.zero)  // If it is not a wall
                        bottomNodes[n].GetComponent<MeshRenderer>().material = decoratingGridValidMat;
            }


            //if (hoveredNode != null) bottomNodes[hoveredNode].GetComponent<MeshRenderer>().material = decoratingGridHovered;
        }

        else  // Placement Mode
        {
            if (hoveredNode != null) objectPreview.transform.position = hoveredNode.worldPos;
            else objectPreview.transform.position = new Vector3(0, 100000, 0);

            CheckPlacementValidity();
        }
    }


    public void MouseClick(Vector3 point, bool pressed)
    {
        if (hoveredNode == null) return;
        if (decorateView == null) return;

        if (placementMode)
        {
            if (selectionValid && selectedObject != null)
            {
                if (Inventory.HasItem(decorateView.selectedItemType.itemName))
                {
                    Inventory.RemoveItem(decorateView.selectedItemType.itemName);
                    PlaceDecoration();
                }
                else if (Money.instance.WithdrawMoney(decorateView.selectedItemType.purchaseValue))
                {
                    PlaceDecoration();
                }
            }
        }
        else  // Selection mode
        {
            List<GameObject> objects = currentGrid.CheckNodeForObject(hoveredNode);
            if (objects.Count != 0)
            {
                if (selectedObject != objects[0])
                {
                    selectedObject = objects[0];
                    if (selectedObject.GetComponent<Decoration>())
                        decorateView.ChangeSelectedItem(selectedObject.GetComponent<Decoration>().decorationSO, selectedObject);  // Add item type here
                }
                else  // Click on the same object again
                {
                    selectedObject = null;
                    decorateView.ChangeSelectedItem(null, null);
                }
            }
        }

        UpdateGridMaterials();
    }


    public void StartPlacing(GameObject d)
    {
        selectedObject = d;
        placementMode = true;

        currentRotation = 0;

        objectPreview = GameObject.Instantiate(selectedObject);
        objectPreview.name = "Object Preview";
        //objectPreview.transform.localScale = new Vector3(currentGrid.pointSize * 2f, currentGrid.pointSize * 2f, currentGrid.pointSize * 2f);
        SetObjectMaterials(objectPreview, objectPreviewValidMat);
    }


    public void StopPlacing()
    {
        placementMode = false;
        selectedObject = null;
        if (objectPreview) Destroy(objectPreview);

        if (decorateView != null)
        {
            decorateView.ChangeSelectedItem(null, null);
            decorateView.UpdateContent();
        }
    }


    private void PlaceDecoration()
    {
        GameObject t = GameObject.Instantiate(selectedObject, hoveredNode.worldPos, Quaternion.identity);
        t.transform.localScale = objectPreview.transform.localScale;
        t.transform.rotation = objectPreview.transform.rotation;
        currentGrid.RebakeGrid();
        StopPlacing();
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
        if (decorateView == null) return;

        GreyOutGrid();

        selectionValid = true;


        //Check for colliding nodes and walls, if they are invalid then invalidate the placement
        Transform[] transforms = objectPreview.GetComponentsInChildren<Transform>();
        List<GridNode> nodes = currentGrid.CheckForObjectCollisions(transforms);

        foreach (GridNode node in nodes)
        {
            if (node.invalid)
            {
                selectionValid = false;

                if (node.worldPos != null && node.worldPos != Vector3.zero)  // If it is not a wall
                {
                    bottomNodes[node].GetComponent<MeshRenderer>().material = decoratingGridInvalidMat;
                }
            }

            else
            {
                bottomNodes[node].GetComponent<MeshRenderer>().material = decoratingGridValidMat;
            }
        }


        // Check Money
        if (decorateView.selectedItemType.purchaseValue > Money.instance.money)
        {
            selectionValid = false;  // Cannot afford
        }



        SetObjectMaterials(objectPreview, selectionValid ? objectPreviewValidMat : objectPreviewInvalidMat);    
    }



    // Selecting when not placing

    // Raycast for objects

    // If not, check for objects on the highlighted node


    public void OnRotate(InputValue value)
    {
        if (value.isPressed)
        {
            RotateObject();
        }
    }

    private void RotateObject()
    {
        if (objectPreview == null) return;
        if (!placementMode) return;
        if (decorateView == null) return;

        currentRotation += rotationSnap;

        objectPreview.transform.Rotate(Vector3.Scale(decorateView.selectedItemType.rotationAxis, new Vector3(rotationSnap, rotationSnap, rotationSnap)));

        CheckPlacementValidity();
    }

    public void MoveDecoration()
    {
        if (selectedObject == null) return;

        // Put decoration away
        // Start placing that decoration
    }

    public void PutDecorationAway()
    {
        if (selectedObject == null) return;

        // Add to inventory
        // Destroy the decoration
    }

    private void GreyOutGrid()
    {
        foreach (GridNode n in bottomNodes.Keys) bottomNodes[n].GetComponent<MeshRenderer>().material = decoratingGridMat;
    }
}
