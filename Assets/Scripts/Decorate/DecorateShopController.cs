using Bitgem.VFX.StylisedWater;
using DG.Tweening;
using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public enum Surface
{
    Floor,
    Wall,
    Ceiling
}


public class DecorateShopController : MonoBehaviour
{
    public static DecorateShopController Instance;

    [SerializeField] ShopGrid currentGrid;
    [SerializeField] GameObject decorateViewPrefab;
    private ShopDecorateViewScript decorateView;

    private List<Decoration> decorationsInStore = new List<Decoration>();

    [SerializeField] Transform decorationParent;

    private Dictionary<GridNode, GameObject> nodes = new Dictionary<GridNode, GameObject>();
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
    private bool movingObject;

    public bool deselectOnPlace;


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



    public void StartDecorating()
    {
        if (currentGrid == null) return;

        decorating = true;

        GameObject newMenu = GameObject.Instantiate(decorateViewPrefab, transform);
        UIManager.instance.OpenScreen(newMenu.GetComponent<ScreenView>());
        newMenu.GetComponent<ShopDecorateViewScript>().UpdateContent();
        newMenu.GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
        newMenu.GetComponent<Canvas>().planeDistance = 1;
        decorateView = newMenu.GetComponent<ScreenView>() as ShopDecorateViewScript;
        UIManager.instance.SetCursorMasking(false);

        transparentDecorations = false;


        int b = 0;  // Bottom Layer
        int t = currentGrid.gridHeight - 1;  // Top Layer
        for (int w = 0; w < currentGrid.gridWidth; w++)
        {
            for (int l = 0; l < currentGrid.gridLength; l++)
            {
                // Bottom Layer
                GameObject node = GameObject.Instantiate(decoratingGridPrefab, currentGrid.grid[w][b][l].worldPos + new Vector3(0, -(currentGrid.pointSize / 2), 0), Quaternion.identity);
                node.transform.parent = currentGrid.transform;
                node.transform.localScale = new Vector3(currentGrid.pointSize / 1.025f, currentGrid.pointSize / 10, currentGrid.pointSize / 1.025f);
                node.GetComponent<MeshRenderer>().enabled = showNodes;
                nodes.Add(currentGrid.grid[w][b][l], node);
            }
        }

        ChangeEditSurface(Surface.Floor);
    }



    public void StopDecorating()
    {
        decorating = false;
        editingTop = false;
        SetTransparentDecorations(false);
        Camera.main.nearClipPlane = 0.01f;

        if (objectPreview)
            Destroy(objectPreview);


        foreach(GameObject n in nodes.Values)
        {
            GameObject.Destroy(n);
        }
        nodes.Clear();


        currentGrid = null;
        hoveredNode = null;
    }



    public void Update()
    {
        if (currentGrid == null) return;

        MouseDetection();
    }



    public void MouseDetection()
    {


        // Use raycastall to get a lisr
        // Go from the back of the list till you find a valid node
        // If not, Go from the back of the list till you find a free node


        if (nodes == null || nodes.Count == 0) return;

        RaycastHit ray;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray originMouse = Camera.main.ScreenPointToRay(mousePos);
        int layerMask = LayerMask.GetMask("GridNode");
        if (Physics.Raycast(originMouse, out ray, 3f, layerMask))
        {
            if (hoveredNode != null && nodes.ContainsKey(hoveredNode) && nodes[hoveredNode] == ray.collider.gameObject) return;


            foreach (GridNode node in nodes.Keys)
            {
                if (nodes[node] == ray.collider.gameObject)
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

        foreach (GridNode n in nodes.Keys)
        {
            if (n.invalid)
                nodes[n].GetComponent<MeshRenderer>().material = decoratingGridTakenMat;
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
                            nodes[n].GetComponent<MeshRenderer>().material = decoratingGridHovered;
                }
            }

            if (selectedObject != null)
            {
                Transform[] transforms = selectedObject.GetComponentsInChildren<Transform>();
                foreach (GridNode n in currentGrid.CheckForObjectCollisions(transforms))
                    if (n.worldPos != null && n.worldPos != Vector3.zero)  // If it is not a wall
                        nodes[n].GetComponent<MeshRenderer>().material = decoratingGridValidMat;
            }


            //if (hoveredNode != null) bottomNodes[hoveredNode].GetComponent<MeshRenderer>().material = decoratingGridHovered;
        }

        else  // Placement Mode
        {
            if (objectPreview != null && decorateView.selectedItemType != null)
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
            if (selectionValid && selectedObject != null)  // Clicking on a valid space
            {
                if (Inventory.HasItem(decorateView.selectedItemType.itemName))
                {
                    Inventory.RemoveItem(decorateView.selectedItemType.itemName);
                    PlaceDecoration(decorateView.selectedItemType);
                }
                else if (Money.instance.WithdrawMoney(decorateView.selectedItemType.purchaseValue))
                {
                    PlaceDecoration(decorateView.selectedItemType);
                }
            }
            else  // Clicking on a different object whilst placing
            {
                List<GameObject> objects = currentGrid.CheckNodeForObject(hoveredNode);
                if (objects.Contains(objectPreview)) objects.Remove(objectPreview);
                if (objects.Count != 0 && selectedObject != objects[0])
                {
                    StopPlacing();
                    selectedObject = objects[0];
                    if (selectedObject.GetComponent<Decoration>() && selectedObject.GetComponent<Decoration>().decorationSO != null)
                        decorateView.ChangeSelectedItem(selectedObject.GetComponent<Decoration>().decorationSO, selectedObject);
                    else Debug.LogWarning(selectedObject + " is missing the decoration script on it's root");
                    UpdateGridMaterials();
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
        objectPreview.TryGetComponent(out floater);
        if (floater != null) floater.enabled = false;

    }


    public void StopPlacing()
    {
        placementMode = false;
        selectedObject = null;
        movingObject = false;

        if (objectPreview)
        {
            objectPreview.SetActive(false);
            Destroy(objectPreview);
            objectPreview = null;
        }

        if (decorateView)
        {
            decorateView.ChangeSelectedItem(null, null);
            decorateView.UpdateContent();
        }
    }


    private void PlaceDecoration(DecorationItemSO so)
    {
        GameObject d = GameObject.Instantiate(selectedObject, hoveredNode.worldPos + so.gridSnapOffset, Quaternion.identity, decorationParent);

        Sequence sequence = DOTween.Sequence(d);
        d.transform.localScale = objectPreview.transform.localScale;
        sequence.Join(d.transform.DOScale(objectPreview.transform.localScale / 2, 0.1f).SetEase(Ease.InOutSine));
        sequence.Join(d.transform.DOScale(objectPreview.transform.localScale, 0.5f).SetEase(Ease.OutBounce));

        d.transform.rotation = objectPreview.transform.rotation;

        // Set up decoration
        Decoration decoration;
        d.TryGetComponent(out decoration);
        if (decoration != null)
        {
            decoration.floating = editingTop;
            decorationsInStore.Add(decoration);
        }

        // Set up floater
        //WateverVolumeFloater floater;
        //d.TryGetComponent(out floater);
        //if (floater != null) floater.WaterVolumeHelper = currentTank.waterObject.GetComponent<WaterVolumeHelper>();


        SetTransparentDecorations(transparentDecorations);
        currentGrid.RebakeGrid();

        if (deselectOnPlace || movingObject)
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
        List<GridNode> hitNodes = currentGrid.CheckForObjectCollisions(transforms);

        foreach (GridNode node in hitNodes)
        {
            if (node.invalid)
            {
                selectionValid = false;

                if (node.worldPos != null && node.worldPos != Vector3.zero)  // If it is not a wall
                {
                    nodes[node].GetComponent<MeshRenderer>().material = decoratingGridInvalidMat;
                }
            }

            else
            {
                nodes[node].GetComponent<MeshRenderer>().material = decoratingGridValidMat;
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
        if (hoveredNode == null) return;

        float r = value.Get<float>();
        if (r > 0.1)
            r = 1;
        else if (r < -0.1)
            r = -1;
        RotateObject(r);
    }

    public void RotateObject(float dir)
    {
        if (objectPreview == null) return;
        if (!placementMode) return;
        if (decorateView == null) return;

        objectPreview.transform.Rotate(Vector3.Scale(decorateView.selectedItemType.rotationAxis, (new Vector3(rotationSnap, rotationSnap, rotationSnap) * dir)));

        UpdateGridMaterials();
    }

    public void MoveDecoration()
    {
        if (selectedObject == null) return;
        if (decorateView.selectedItemType == null) return;
        DecorationItemSO so = decorateView.selectedItemType;
        Quaternion rot = selectedObject.transform.rotation;
        PutDecorationAway();
        decorateView.ChangeSelectedItem(so, so.decorationPrefab);
        movingObject = true;
        StartPlacing(so.decorationPrefab);
        if (objectPreview != null)
            objectPreview.transform.rotation = rot;
    }

    public void PutDecorationAway()
    {
        if (selectedObject == null) return;
        if (currentGrid == null) return;
        if (decorateView == null) return;
        if (decorateView.selectedItemType == null) return;

        Inventory.AddItem(decorateView.selectedItemType.itemName);

        decorationsInStore.Remove(selectedObject.GetComponent<Decoration>());

        GameObject obj = selectedObject;
        selectedObject.gameObject.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(()=>DestroyDecoration(obj));

        selectedObject = null;
        decorateView.ChangeSelectedItem(null, null);
        decorateView.UpdateContent();
    }

    private void DestroyDecoration(GameObject obj)
    {
        if (obj == null) return;
        obj.SetActive(false);
        Destroy(obj.gameObject);
        UpdateGridMaterials();
        currentGrid.RebakeGrid();
    }

    private void GreyOutGrid()
    {
        foreach (GridNode n in nodes.Keys) nodes[n].GetComponent<MeshRenderer>().material = decoratingGridMat;
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
            foreach (Decoration d in decorationsInStore)
            {
                SetObjectMaterials(d.gameObject, objectTransparentMat);
            }
        }

        else  // Decorations on the level you aren't editing are transparent
        {
            foreach (Decoration d in decorationsInStore)
            {
                if (((editingTop && !d.floating) ||  // If it isn't on the level you are editing
                    (!editingTop && d.floating)) && decorating)
                    SetObjectMaterials(d.gameObject, objectTransparentMat);

                else
                    d.ResetMaterials();
            }
        }
    }



    public void ChangeEditSurface(Surface s)
    {
        //selectedObject = null;
        //StopPlacing();
        //editingTop = top;
        //ChangeCam(0);
        //SetTransparentDecorations(transparentDecorations);

        //foreach(GridNode n in bottomNodes.Keys)
        //{
        //    bottomNodes[n].gameObject.SetActive(!top);
        //}

        //foreach (GridNode n in topNodes.Keys)
        //{
        //    topNodes[n].gameObject.SetActive(top);
        //}

        //if (!top)
        //    editingLayer = 0;
        //else
        //    editingLayer = currentGrid.gridHeight - 1;
    }

    //public void OnChangeCam(InputValue value)
    //{
    //    if (value.isPressed)
    //    {
    //        ChangeCam();
    //    }
    //}

    //public void ChangeCam(int c = 1)
    //{
    //    if (currentTank == null) return;

    //    int limit = currentTank.decorationCamDock.Length - 1;

    //    camAngle += c;

    //    if (camAngle > limit) camAngle = 0;
    //    if (camAngle < 0) camAngle = limit;

    //    DecorateCamera cam;
    //    currentTank.decorationCamDock[camAngle].TryGetComponent(out cam);
    //    if (cam != null)
    //    {
    //        Vector3 pos = cam.transform.position;
    //        if (editingTop)
    //        {
    //            pos.y += cam.topCamHeight;
    //            Camera.main.nearClipPlane = cam.topCamClippingPlane;
    //        }
    //        else
    //            Camera.main.nearClipPlane = 0.01f;

    //        Camera.main.transform.position = pos;
    //        if (cam.lookAt != null)
    //        {
    //            Camera.main.transform.LookAt(editingTop ? cam.lookAt : cam.topCamLookAt, Vector3.up);
    //        }
    //        else
    //            Camera.main.transform.rotation = cam.transform.rotation;
    //    }
    //}

    public void LoadDecorations(TankController tank, TankSaveData data)
    {
        if (tank == null) return;
        if (data == null || data.decorations == null || data.decorations.Length == 0) return;

        foreach (TankDecorationSaveData decorationSave in data.decorations)
        {
            GameObject d = GameObject.Instantiate(((DecorationItemSO)Inventory.GetSOForItem(Inventory.GetItemUsingName(decorationSave.name))).decorationPrefab, 
                new Vector3(decorationSave.position.X, decorationSave.position.Y, decorationSave.position.Z), 
                Quaternion.Euler(new Vector3(decorationSave.rotation.X, decorationSave.rotation.Y, decorationSave.rotation.Z)), 
                decorationParent);

            decorationsInStore.Add(d.GetComponent<Decoration>());

            // Set up decoration
            Decoration decoration;
            d.TryGetComponent(out decoration);
            if (decoration != null) decoration.floating = decorationSave.floating;

            // Set up floater
            //WateverVolumeFloater floater;
            //d.TryGetComponent(out floater);
            //if (floater != null) floater.WaterVolumeHelper = currentTank.waterObject.GetComponent<WaterVolumeHelper>();
        }

        currentGrid.RebakeGrid();
        StopDecorating();
    }
}
