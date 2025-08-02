using Bitgem.VFX.StylisedWater;
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
    private Dictionary<GridNode, GameObject> topNodes = new Dictionary<GridNode, GameObject>();
    private Dictionary<GridNode, GameObject> allNodes = new Dictionary<GridNode, GameObject>();
    private GridNode hoveredNode;
    [HideInInspector] public GameObject selectedObject;
    private GameObject objectPreview;

    [HideInInspector] public bool decorating;
    [HideInInspector] public bool placementMode;
    [HideInInspector] public bool editingTop;
    [HideInInspector] public int editingLayer;
    private bool selectionValid;
    public float rotationSnap = 90;
    private bool transparentDecorations;
    private bool transparentShrimp;
    private int camAngle = 0;


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



    public void StartDecorating(TankController tank)
    {
        if (tank == null) return;
        if (tank.tankViewScript == null) return;
        currentTank = tank;
        currentGrid = tank.tankGrid;
        decorating = true;

        GameObject newMenu = GameObject.Instantiate(tank.tankViewScript.tankDecorateView, tank.transform);
        UIManager.instance.OpenScreen(newMenu.GetComponent<ScreenView>());
        newMenu.GetComponent<TankDecorateViewScript>().UpdateContent();
        newMenu.GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
        newMenu.GetComponent<Canvas>().planeDistance = 1;
        UIManager.instance.SetCursorMasking(false);

        camAngle = 0;
        ChangeCam(0);

        currentTank.waterObject.SetActive(false);
        transparentDecorations = false;
        transparentShrimp = false;


        int b = 0;  // Bottom Layer
        int t = currentGrid.gridHeight - 1;  // Top Layer
        for (int w = 0; w < currentGrid.gridWidth; w++)
        {
            for (int l = 0; l < currentGrid.gridLength; l++)
            {
                // Bottom Layer
                GameObject node = GameObject.Instantiate(decoratingGridPrefab, currentGrid.grid[w][b][l].worldPos + new Vector3(0, -(currentGrid.pointSize / 2), 0), Quaternion.identity);
                node.transform.parent = currentTank.transform;
                node.transform.localScale = new Vector3(currentGrid.pointSize / 1.025f, currentGrid.pointSize / 10, currentGrid.pointSize / 1.025f);
                node.GetComponent<MeshRenderer>().enabled = showNodes;
                bottomNodes.Add(currentGrid.grid[w][b][l], node);
                allNodes.Add(currentGrid.grid[w][b][l], node);

                // Top Layer
                node = GameObject.Instantiate(decoratingGridPrefab, currentGrid.grid[w][t][l].worldPos + new Vector3(0, -(currentGrid.pointSize / 2), 0), Quaternion.identity);
                node.transform.parent = currentTank.transform;
                node.transform.localScale = new Vector3(currentGrid.pointSize, currentGrid.pointSize / 10, currentGrid.pointSize);
                node.GetComponent<MeshRenderer>().enabled = showNodes;
                topNodes.Add(currentGrid.grid[w][t][l], node);
                allNodes.Add(currentGrid.grid[w][t][l], node);
            }
        }

        ChangeEditLayer(false);

        foreach (Shrimp shrimp in currentTank.shrimpInTank)
        {
            shrimp.gameObject.AddComponent<Decoration>();
        }
    }



    public void StopDecorating()
    {
        currentTank.waterObject.SetActive(true);
        SetTransparentDecorations(false);
        SetTransparentShrimp(false);

        Decoration d;
        foreach (Shrimp shrimp in currentTank.shrimpInTank)
        {
            shrimp.gameObject.TryGetComponent<Decoration>(out d);
            if (d != null) Destroy(d);
        }


        if (objectPreview)
            Destroy(objectPreview);


        foreach(GameObject n in allNodes.Values)
        {
            GameObject.Destroy(n);
        }
        bottomNodes.Clear();
        topNodes.Clear();
        allNodes.Clear();


        currentTank = null;
        currentGrid = null;
        hoveredNode = null;
        decorating = false;
    }



    public void Update()
    {
        if (currentTank == null) return;
        if (currentGrid == null) return;

        MouseDetection();
    }



    public void MouseDetection()
    {
        if (allNodes == null || allNodes.Count == 0) return;

        RaycastHit ray;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray originMouse = Camera.main.ScreenPointToRay(mousePos);
        int layerMask = LayerMask.GetMask("GridNode");
        if (Physics.Raycast(originMouse, out ray, 3f, layerMask))
        {
            if (hoveredNode != null && allNodes.ContainsKey(hoveredNode) && allNodes[hoveredNode] == ray.collider.gameObject) return;


            foreach (GridNode node in allNodes.Keys)
            {
                if (allNodes[node] == ray.collider.gameObject)
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

        foreach (GridNode n in allNodes.Keys)
        {
            if (n.invalid)
                allNodes[n].GetComponent<MeshRenderer>().material = decoratingGridTakenMat;
        }

        if (!placementMode)
        {
            if (hoveredNode != null)
            {
                foreach (GameObject obj in currentGrid.CheckNodeForObject(hoveredNode))
                {
                    Transform[] transforms = obj.GetComponentsInChildren<Transform>();
                    foreach (GridNode n in currentGrid.CheckForObjectCollisions(transforms))
                        if (n.worldPos != null && n.worldPos != Vector3.zero)  // If it is not a wall
                            allNodes[n].GetComponent<MeshRenderer>().material = decoratingGridHovered;
                }
            }

            if (selectedObject != null)
            {
                Transform[] transforms = selectedObject.GetComponentsInChildren<Transform>();
                foreach (GridNode n in currentGrid.CheckForObjectCollisions(transforms))
                    if (n.worldPos != null && n.worldPos != Vector3.zero)  // If it is not a wall
                        allNodes[n].GetComponent<MeshRenderer>().material = decoratingGridValidMat;
            }


            //if (hoveredNode != null) bottomNodes[hoveredNode].GetComponent<MeshRenderer>().material = decoratingGridHovered;
        }

        else  // Placement Mode
        {
            if (objectPreview != null)
            {
                if (hoveredNode != null) objectPreview.transform.position = hoveredNode.worldPos + decorateView.selectedItemType.gridSnapOffset;
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

        // Disable the floater if it has one
        WateverVolumeFloater floater;
        d.TryGetComponent(out floater);
        if (floater != null) floater.enabled = false;

    }


    public void StopPlacing()
    {
        placementMode = false;
        selectedObject = null;
        if (objectPreview) Destroy(objectPreview);
        objectPreview = null;

        if (decorateView != null)
        {
            decorateView.ChangeSelectedItem(null, null);
            decorateView.UpdateContent();
        }
    }


    private void PlaceDecoration()
    {
        GameObject d = GameObject.Instantiate(selectedObject, hoveredNode.worldPos + decorateView.selectedItemType.gridSnapOffset, Quaternion.identity, currentTank.decorationParent);
        d.transform.localScale = objectPreview.transform.localScale;
        d.transform.rotation = objectPreview.transform.rotation;
        currentTank.decorationsInTank.Add(d);

        // Set up decoration
        Decoration decoration;
        d.TryGetComponent(out decoration);
        if (decoration != null) decoration.floating = editingTop;

        // Set up floater
        WateverVolumeFloater floater;
        d.TryGetComponent(out floater);
        if (floater != null) floater.WaterVolumeHelper = currentTank.waterObject.GetComponent<WaterVolumeHelper>();

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
                    allNodes[node].GetComponent<MeshRenderer>().material = decoratingGridInvalidMat;
                }
            }

            else
            {
                allNodes[node].GetComponent<MeshRenderer>().material = decoratingGridValidMat;
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
        foreach (GridNode n in allNodes.Keys) allNodes[n].GetComponent<MeshRenderer>().material = decoratingGridMat;
    }

    public void ToggleTransparentDecorarions()
    {
        SetTransparentDecorations(!transparentDecorations);
    }

    private void SetTransparentDecorations(bool s)
    {
        transparentDecorations = s;

        if (transparentDecorations)  // All decorations are transparent
        {
            foreach (GameObject obj in currentTank.decorationsInTank)
            {
                SetObjectMaterials(obj, objectTransparentMat);
            }
        }

        else  // Decorations on the level you aren't editing are transparent
        {
            foreach (GameObject obj in currentTank.decorationsInTank)
            {
                if ((editingTop && !obj.GetComponent<Decoration>().floating) ||  // If it isn't on the level you are editing
                    (!editingTop && obj.GetComponent<Decoration>().floating))
                    SetObjectMaterials(obj, objectTransparentMat);

                else
                    obj.GetComponent<Decoration>().ResetMaterials();
            }
        }
    }

    public void ToggleTransparentShrimp()
    {
        SetTransparentShrimp(!transparentShrimp);
    }

    private void SetTransparentShrimp(bool s)
    {
        if (currentTank == null) return;
        if (currentTank.shrimpInTank == null || currentTank.shrimpInTank.Count == 0) return;

        transparentShrimp = s;

        if (transparentShrimp)
        {
            Decoration d;
            foreach (Shrimp shrimp in currentTank.shrimpInTank)
            {
                shrimp.gameObject.TryGetComponent<Decoration>(out d);
                if (d != null) SetObjectMaterials(shrimp.gameObject, objectTransparentMat);
            }
        }

        else
        {
            Decoration d;
            foreach (Shrimp shrimp in currentTank.shrimpInTank)
            {
                shrimp.gameObject.TryGetComponent<Decoration>(out d);
                if (d != null) d.ResetMaterials();
            }
        }
    }

    public void ChangeEditLayer(bool top)
    {
        selectedObject = null;
        StopPlacing();
        editingTop = top;
        ChangeCam(0);
        SetTransparentDecorations(transparentDecorations);

        foreach(GridNode n in bottomNodes.Keys)
        {
            bottomNodes[n].gameObject.SetActive(!top);
        }

        foreach (GridNode n in topNodes.Keys)
        {
            topNodes[n].gameObject.SetActive(top);
        }

        if (!top)
            editingLayer = 0;
        else
            editingLayer = currentGrid.gridHeight - 1;
    }

    public void OnChangeCam(InputValue value)
    {
        if (value.isPressed)
        {
            ChangeCam();
        }
    }

    public void ChangeCam(int c = 1)
    {
        if (currentTank == null) return;

        int limit = currentTank.decorationCamDock.Length - 1;

        camAngle += c;

        if (camAngle > limit) camAngle = 0;
        if (camAngle < 0) camAngle = limit;

        Vector3 pos = currentTank.decorationCamDock[camAngle].transform.position;
        if (editingTop) pos.y += currentTank.decorateSurfaceCamHeight;

        Camera.main.transform.position = pos;
        if (camAngle <= 3) Camera.main.transform.LookAt(currentTank.decorationCamLookPoint.transform, Vector3.up);
        else Camera.main.transform.rotation = currentTank.decorationCamDock[camAngle].transform.rotation;
    }
}
