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
    private GridNode hoveredNode;
    [HideInInspector] public GameObject selectedObject;
    private GameObject objectPreview;

    [HideInInspector] public bool placementMode;
    private bool selectionValid;
    public float rotationSnap = 90;
    private bool transparentDecorations;


    [Header("Grid")]
    public GameObject decoratingGridPrefab;
    public Material decoratingGridMat;
    public Material decoratingGridTakenMat;
    public Material decoratingGridHovered;
    public Material decoratingGridValidMat;
    public Material decoratingGridInvalidMat;
    public Material objectPreviewValidMat;
    public Material objectPreviewInvalidMat;
    public Material objectTransparentMat;

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
        transparentDecorations = false;



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
        SetTransparentDecorations(false);

        currentTank = null;
        currentGrid = null;
        hoveredNode = null;


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
        GreyOutGrid();

        foreach (GridNode n in bottomNodes.Keys)
        {
            if (n.invalid)
                bottomNodes[n].GetComponent<MeshRenderer>().material = decoratingGridTakenMat;
        }


        if (!placementMode)
        {
            // Raycast for objects first?


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
            if (objectPreview != null)
            {
                if (hoveredNode != null) objectPreview.transform.position = hoveredNode.worldPos;
                else objectPreview.transform.position = new Vector3(0, 100000, 0);
            }

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
                    if (selectedObject.GetComponent<Decoration>() && selectedObject.GetComponent<Decoration>().decorationSO != null)
                        decorateView.ChangeSelectedItem(selectedObject.GetComponent<Decoration>().decorationSO, selectedObject);
                    else Debug.LogWarning(selectedObject + " is missing the decoration script on it's root");
                }
                else  // Click on the same object again
                {
                    selectedObject = null;
                    decorateView.ChangeSelectedItem(null, null);
                }
            }
            else  // Click on an empty node
            {
                selectedObject = null;
                decorateView.ChangeSelectedItem(null, null);
            }
        }

        UpdateGridMaterials();
    }


    public void StartPlacing(GameObject d)
    {
        selectedObject = d;
        placementMode = true;

        objectPreview = GameObject.Instantiate(selectedObject);
        objectPreview.name = "Object Preview";
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
        GameObject d = GameObject.Instantiate(selectedObject, hoveredNode.worldPos, Quaternion.identity);
        d.transform.localScale = objectPreview.transform.localScale;
        d.transform.rotation = objectPreview.transform.rotation;
        currentTank.decorationsInTank.Add(d);
        SetTransparentDecorations(transparentDecorations);
        currentGrid.RebakeGrid();
        StopPlacing();
    }


    private void SetObjectMaterials(GameObject obj, Material mat)
    {
        if (obj == null) return;
        if (mat == null) return;

        Decoration decoration = obj.GetComponent<Decoration>();
        MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
        if (decoration == null) return;

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
        if (decorateView.selectedItemType == null) return;


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
        if (!Inventory.HasItem(decorateView.selectedItemType.itemName))
            if (decorateView.selectedItemType.purchaseValue > Money.instance.money)
                selectionValid = false;  // Cannot afford



        SetObjectMaterials(objectPreview, selectionValid ? objectPreviewValidMat : objectPreviewInvalidMat);    
    }



    public void OnRotate(InputValue value)
    {
        if (value.isPressed)
        {
            RotateObject();
        }
    }

    public void RotateObject()
    {
        if (objectPreview == null) return;
        if (!placementMode) return;
        if (decorateView == null) return;

        objectPreview.transform.Rotate(Vector3.Scale(decorateView.selectedItemType.rotationAxis, new Vector3(rotationSnap, rotationSnap, rotationSnap)));

        UpdateGridMaterials();
    }

    public void MoveDecoration()
    {
        DecorationItemSO so = decorateView.selectedItemType;
        PutDecorationAway();
        decorateView.ChangeSelectedItem(so, so.decorationPrefab);
        StartPlacing(so.decorationPrefab);
    }

    public void PutDecorationAway()
    {
        if (selectedObject == null) return;
        if (currentGrid == null) return;
        if (decorateView == null) return;
        if (decorateView.selectedItemType == null) return;

        Inventory.AddItem(decorateView.selectedItemType.itemName);

        currentTank.decorationsInTank.Remove(selectedObject);
        selectedObject.gameObject.SetActive(false);
        Destroy(selectedObject.gameObject);

        selectedObject = null;
        decorateView.ChangeSelectedItem(null, null);
        UpdateGridMaterials();
        currentGrid.RebakeGrid();
        decorateView.UpdateContent();
    }

    private void GreyOutGrid()
    {
        foreach (GridNode n in bottomNodes.Keys) bottomNodes[n].GetComponent<MeshRenderer>().material = decoratingGridMat;
    }

    public void ToggleTransparentDecorarions()
    {
        SetTransparentDecorations(!transparentDecorations);
    }

    private void SetTransparentDecorations(bool s)
    {
        transparentDecorations = s;

        if (transparentDecorations)
        {
            // Iterate through decorations and set mat
            foreach (GameObject obj in currentTank.decorationsInTank)
            {
                SetObjectMaterials(obj, objectTransparentMat);
            }
        }

        else
        {
            foreach (GameObject obj in currentTank.decorationsInTank)
            {
                obj.GetComponent<Decoration>().ResetMaterials();
            }
        }
    }
}
