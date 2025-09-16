using Bitgem.VFX.StylisedWater;
using DG.Tweening;
using SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UIElements;


public enum Surface
{
    Floor,
    Wall,
    Ceiling
}


public class DecorateShopController : MonoBehaviour
{
    [SerializeField] ShopGrid currentGrid;
    [SerializeField] GameObject shopInventoryPrefab;
    private ShopInventory inventoryScreen;

    [HideInInspector] public List<Decoration> decorationsInStore = new List<Decoration>();
    [HideInInspector] public List<TankController> tanksInStore { get; private set; } = new List<TankController>();


    [SerializeField] Transform decorationParent;

    private Dictionary<RoomGridNode, GameObject> nodes = new Dictionary<RoomGridNode, GameObject>();
    private RoomGridNode hoveredNode, previousHoveredNode, previousPreviousHoveredNode;
    [HideInInspector] public GameObject selectedObject;
    [HideInInspector] public GameObject selectedMovingObject;
    [HideInInspector] public DecorationItemSO selectedItemType;
    private GameObject objectPreview;

    [HideInInspector] public bool decorating;
    [HideInInspector] public bool placementMode;
    [HideInInspector] public bool ignoreShelves;
    [HideInInspector] public int editingLayer;
    private bool selectionValid;
    public float rotationSnap = 90;
    public float placementRange = 10;
    private int rotationInput;
    private bool movingObject;
    private bool hoveringShelf;

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
    public DecorationItemSO testItemType;



    public void Awake()
    {
        if (currentGrid == null) currentGrid = GetComponent<ShopGrid>();
    }


    private void Start()
    {
        if (!SaveManager.loadingGameFromFile)
        {
            foreach (Decoration d in decorationParent.transform.GetComponentsInChildren<Decoration>())
            {
                decorationsInStore.Add(d);

                // Set up tank
                TankController tank;
                d.TryGetComponent(out tank);
                if (tank != null)
                {
                    tanksInStore.Add(tank);
                    tank.tankName = "Tank " + tanksInStore.Count;
                }

                //SetTransparentDecorations(transparentDecorations);
                PlayerStats.stats.roomDecorationCount = decorationsInStore.Count;
                PlayerStats.stats.tankCount = tanksInStore.Count;
            }

            currentGrid.RebakeGrid();
        }
    }



    public void OpenShopInventory(List<InventoryTabs> tabs)
    {
        if (decorating || inventoryScreen != null) return;

        GameObject newMenu = GameObject.Instantiate(shopInventoryPrefab, transform);
        UIManager.instance.OpenScreen(newMenu.GetComponent<ScreenView>());
        newMenu.GetComponent<ShopInventory>().OpenInventory(tabs);
        newMenu.GetComponent<ShopInventory>().UpdateContent();
        newMenu.GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
        newMenu.GetComponent<Canvas>().planeDistance = 1;
        inventoryScreen = newMenu.GetComponent<ScreenView>() as ShopInventory;
        UIManager.instance.SetCursorMasking(false);
    }



    public void Update()
    {
        if (currentGrid == null) return;

        MouseDetection();
        SetObjectRotation();
    }



    public void MouseDetection()
    {

        // Use raycastall to get a list
        // Go from the back of the list till you find a valid node
        // If not, Go from the back of the list till you find a free node
        // Keep going if you do find one, if an invalid node is encountered then look for the next valid node

        if (nodes == null || nodes.Count == 0) return;

        previousPreviousHoveredNode = previousHoveredNode;
        previousHoveredNode = hoveredNode;


        RaycastHit[] hits;
        LayerMask layerMask = LayerMask.GetMask("ShelfGridNode");
        hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), placementRange, layerMask, QueryTriggerInteraction.Collide);
        foreach (RaycastHit h in hits)
        {
            ShelfGridNode shelfNode = h.transform.GetComponent<ShelfGridNode>();
            if (shelfNode)
            {
                if (!nodes.ContainsKey(shelfNode.roomGridNode))
                    nodes.Add(shelfNode.roomGridNode, shelfNode.gameObject);
            }
        }


        layerMask = LayerMask.GetMask("GridNode") | LayerMask.GetMask("ShelfGridNode");
        hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), placementRange, layerMask, QueryTriggerInteraction.Collide);
        bool shelfItem = selectedItemType.placementSurfaces.Contains(PlacementSurfaces.Shelf);
        hoveredNode = null;
        if (hits.Length == 0) return;
        for (int i = hits.Length - 1; i >= 0; i--)
        {
            RaycastHit hit = hits[i];

            foreach (RoomGridNode node in nodes.Keys)
            {
                if (node == null || nodes[node] == null) nodes.Remove(node);

                if (nodes[node] == hit.collider.gameObject)
                {
                    if (node.invalid)  // If the node is taken
                    {
                        hoveredNode = null;
                        break;
                    }

                    if (!shelfItem && node.shelf)  // If it is not a shelf item and hits a shelf
                    {
                        hoveredNode = null;
                        break;
                    }

                    if (shelfItem && node.shelf && !nodes[node].GetComponent<ShelfGridNode>())  // If it is a shelf item and hits a shelf
                    {
                        break;
                    }

                    if (shelfItem && node.shelf && nodes[node].GetComponent<ShelfGridNode>())  // If it is a shelf item and hits a shelf slot
                    {
                        hoveredNode = node;
                        hoveringShelf = true;
                        break;
                    }

                    if (hoveredNode != null || shelfItem)  // If we want to get a new node
                    {
                        break;
                    }

                    hoveredNode = node;
                    hoveringShelf = false;
                    break;
                }

            }
        }

        if (hoveredNode != null)  // If a valid node has been found
        {
            while(!selectedItemType.placementSurfaces.Contains(GetCurrentSurface()))
            {
                if (hoveredNode.nodeBelow == null) break;
                
                hoveredNode = hoveredNode.nodeBelow;
            }

            UpdateGridMaterials();
            return;
        }
    }


    private void UpdateGridMaterials()
    {
        GreyOutGrid();

        foreach (RoomGridNode n in nodes.Keys)
        {
            if (n.invalid)
            {
                MeshRenderer m;
                nodes[n].TryGetComponent(out m);
                if (m) m.material = decoratingGridTakenMat;
            }
        }

        if (!placementMode)
        {
            if (hoveredNode != null)
            {
                foreach (GameObject obj in currentGrid.CheckNodeForObject(hoveredNode))
                {
                    Transform[] transforms = obj.GetComponentsInChildren<Transform>();
                    foreach (RoomGridNode n in currentGrid.CheckForObjectCollisions(transforms, ignoreShelves))
                    {
                        if (n.worldPos != null && n.worldPos != Vector3.zero)  // If it is not a wall
                        {
                            MeshRenderer m;
                            nodes[n].TryGetComponent(out m);
                            if (m) m.material = decoratingGridHovered;
                        }
                    }
                }
            }

            if (selectedObject != null)
            {
                Transform[] transforms = selectedObject.GetComponentsInChildren<Transform>();
                foreach (RoomGridNode n in currentGrid.CheckForObjectCollisions(transforms, ignoreShelves))
                {
                    if (n.worldPos != null && n.worldPos != Vector3.zero)  // If it is not a wall
                    {
                        MeshRenderer m;
                        nodes[n].TryGetComponent(out m);
                        if (m) m.material = decoratingGridValidMat;
                    }
                }
            }


            //if (hoveredNode != null) bottomNodes[hoveredNode].GetComponent<MeshRenderer>().material = decoratingGridHovered;
        }

        else  // Placement Mode
        {
            if (objectPreview != null && selectedItemType != null)
            {
                if (hoveredNode != null && hoveredNode != previousHoveredNode && previousHoveredNode == previousPreviousHoveredNode)
                {                    objectPreview.transform.position = GetPlacementPosition();

                }
                else if (hoveredNode == null) objectPreview.transform.position = new Vector3(0, 100000, 0);
            }

            CheckPlacementValidity();
        }
    }

    private Vector3 GetPlacementPosition()
    {
        if (hoveringShelf)
            return hoveredNode.worldPos;

        else
            return hoveredNode.worldPos + selectedItemType.gridSnapOffset;

        //    //GetCurrentSurface() // Wall & ceiling rotations

    }



    public void MouseClick(bool pressed)
    {
        if (hoveredNode == null) return;
        if (placementMode)
        {
            if (selectionValid && selectedObject != null)  // Clicking on a valid space
            {
                if (Inventory.HasItem(selectedItemType.itemName) || movingObject)
                {
                    PlaceDecoration(selectedItemType);
                }
                else if (Money.instance.WithdrawMoney(selectedItemType.purchaseValue))
                {
                    PlaceDecoration(selectedItemType);
                }
            }
        }

        UpdateGridMaterials();
    }



    public void StartPlacing(GameObject d, DecorationItemSO t)
    {
        decorating = true;
        selectedObject = d;
        selectedItemType = t;
        placementMode = true;
        rotationInput = 0;
        inventoryScreen = null;

        // Create Grid
        for (int w = 0; w < currentGrid.roomSize.x; w++)
        {
            for (int h = 0; h < currentGrid.roomSize.y; h++)
            {
                for (int l = 0; l < currentGrid.roomSize.z; l++)
                {
                    GameObject node = GameObject.Instantiate(decoratingGridPrefab, currentGrid.grid[w][h][l].worldPos, Quaternion.identity);
                    node.transform.parent = currentGrid.transform;
                    node.transform.localScale = new Vector3(currentGrid.pointSize / 1.025f, currentGrid.pointSize / 1.025f, currentGrid.pointSize / 1.025f);
                    node.GetComponent<MeshRenderer>().enabled = showNodes;
                    nodes.Add(currentGrid.grid[w][h][l], node);
                }
            }
        }


        // Create Object Preview
        objectPreview = GameObject.Instantiate(selectedObject);
        objectPreview.name = "Object Preview";
        SetObjectMaterials(objectPreview, objectPreviewValidMat);
        foreach (Collider c in objectPreview.GetComponentsInChildren<Collider>())
            c.isTrigger = true;

        // Disable the floater if it has one
        WateverVolumeFloater floater;
        objectPreview.TryGetComponent(out floater);
        if (floater != null) floater.enabled = false;

        ignoreShelves = t.placementSurfaces.Contains(PlacementSurfaces.Shelf);
    }



    public void StopPlacing()
    {
        if (objectPreview)
        {
            objectPreview.SetActive(false);
            Destroy(objectPreview);
            objectPreview = null;
        }

        if (movingObject && selectedMovingObject && selectedItemType)
        {
            selectedMovingObject.SetActive(true);
            currentGrid.RebakeGrid();
        }


        //SetTransparentDecorations(false);


        CrossHairScript.ShowCrosshair();
        movingObject = false;
        decorating = false;
        hoveredNode = null;
        placementMode = false;
        selectedObject = null;
        ignoreShelves = false;
        selectedMovingObject = null;
        previousHoveredNode = null;
        previousPreviousHoveredNode = null;
        rotationInput = 0;

        foreach (GameObject n in nodes.Values)
        {
            if (!n.GetComponent<ShelfGridNode>())  // If it isn't a shelf node
                GameObject.Destroy(n);
        }
        nodes.Clear();
    }



    private void PlaceDecoration(DecorationItemSO so)
    {
        PlacementSurfaces surface = GetCurrentSurface();
        GameObject d = Instantiate(selectedObject, decorationParent);

        d.transform.position = GetPlacementPosition();

        Sequence sequence = DOTween.Sequence(d);
        d.transform.localScale = objectPreview.transform.localScale;
        sequence.Join(d.transform.DOScale(objectPreview.transform.localScale / 2, 0.1f).SetEase(Ease.InOutSine));
        sequence.Join(d.transform.DOScale(objectPreview.transform.localScale, 0.5f).SetEase(Ease.OutBounce));

        d.transform.rotation = objectPreview.transform.rotation;

        if (!movingObject)
            Inventory.RemoveItem(selectedItemType.itemName);

        // Set up decoration
        Decoration decoration;
        d.TryGetComponent(out decoration);
        if (decoration != null)
        {
            decoration.floating = surface == PlacementSurfaces.Water;
            decorationsInStore.Add(decoration);
        }

        // Set up tank
        TankController tank;
        d.TryGetComponent(out tank);
        if (tank != null)
        {
            tanksInStore.Add(tank);
            tank.tankName = "Tank " + tanksInStore.Count;
            tank.tankId = Time.time.ToString();
        }

        // Set up floater
        //WateverVolumeFloater floater;
        //d.TryGetComponent(out floater);
        //if (floater != null) floater.WaterVolumeHelper = currentTank.waterObject.GetComponent<WaterVolumeHelper>();


        if (hoveringShelf)
            hoveredNode.invalid = true;


        if (movingObject && selectedMovingObject && selectedItemType)
            PutDecorationAway(selectedMovingObject, selectedItemType, true);
        movingObject = false;


        //SetTransparentDecorations(transparentDecorations);
        currentGrid.RebakeGrid();

        PlayerStats.stats.roomDecorationCount = decorationsInStore.Count;
        PlayerStats.stats.tankCount = tanksInStore.Count;

        if (deselectOnPlace || selectedMovingObject || Inventory.GetItemUsingSO(selectedItemType).quantity <= 0)
            StopPlacing();
    }



    private PlacementSurfaces GetCurrentSurface()
    {
        if (hoveredNode == null) return PlacementSurfaces.Air;

        DecorationItemSO so = null; 
        if (currentGrid.CheckNodeForObject(hoveredNode).Count != 0)
        {
            foreach (GameObject g in currentGrid.CheckNodeForObject(hoveredNode))
            {
                Decoration d = null;
                if (g != objectPreview && g.TryGetComponent<Decoration>(out d))
                {
                    so = d.decorationSO;
                    break;
                }
            }
        }

        // Check for Water
        if (so != null && so.water)
            return PlacementSurfaces.Water;

        // Check for Shelf
        if (so != null && so.shelf)
            return PlacementSurfaces.Shelf;

        // Check for Ground
        if (hoveredNode.floor)
            return PlacementSurfaces.Ground;

        // Check for Wall
        if (hoveredNode.nWall || hoveredNode.eWall || hoveredNode.sWall || hoveredNode.wWall)
            return PlacementSurfaces.Wall;

        // Check for Ceiling
        if (hoveredNode.ceiling)
            return PlacementSurfaces.Ceiling;



        so = null;
        if (hoveredNode.nodeBelow != null && currentGrid.CheckNodeForObject(hoveredNode.nodeBelow).Count != 0)
        {
            foreach (GameObject g in currentGrid.CheckNodeForObject(hoveredNode.nodeBelow))
            {
                Decoration d = null;
                if (g != objectPreview && g.TryGetComponent<Decoration>(out d))
                {
                    so = d.decorationSO;
                    break;
                }
            }
        }

        // Check for water below
        if (so != null && so.water)
            return PlacementSurfaces.Water;

        // Check for shelf below
        if (so != null && so.shelf)
            return PlacementSurfaces.Shelf;



        return PlacementSurfaces.Air;
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
        if (selectedItemType == null) return;
        if (hoveredNode == null) return;

        selectionValid = true;


        //Check for colliding nodes and walls, if they are invalid then invalidate the placement
        if (hoveringShelf)
        {
            if (hoveredNode.invalid)
                selectionValid = false;
        }
        else
        {
            Transform[] transforms = objectPreview.GetComponentsInChildren<Transform>();
            List<RoomGridNode> hitNodes = currentGrid.CheckForObjectCollisions(transforms, ignoreShelves);

            foreach (RoomGridNode node in hitNodes)
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


            // If the surface is wrong
            if (!selectedItemType.placementSurfaces.Contains(GetCurrentSurface()))
                selectionValid = false;
        }


        // If the object is touching the player
        List<GameObject> hitObjects = currentGrid.CheckPlayerForObject();
        if (hitObjects.Contains(objectPreview))
            selectionValid = false;


        // Check Money
        if (!Inventory.HasItem(selectedItemType.itemName))
            if (selectedItemType.purchaseValue > Money.instance.money)
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

        rotationInput += Mathf.RoundToInt(rotationSnap * dir);
    }

    public void SetObjectRotation()
    {
        if (objectPreview == null) return;
        if (!placementMode) return;

        Vector3 vec = Camera.main.transform.eulerAngles;
        vec.x = (Mathf.Round(vec.x / rotationSnap) * rotationSnap) + 180 + rotationInput;
        vec.y = (Mathf.Round(vec.y / rotationSnap) * rotationSnap) + 180 + rotationInput;
        vec.z = (Mathf.Round(vec.z / rotationSnap) * rotationSnap) + 180 + rotationInput;
        objectPreview.transform.eulerAngles = Vector3.Scale(selectedItemType.rotationAxis, vec);

        UpdateGridMaterials();
    }

    public void MoveDecoration(GameObject obj, DecorationItemSO so)
    {
        if (!obj || !so) return;

        Quaternion rot = obj.transform.rotation;
        movingObject = true;
        selectedMovingObject = obj;
        selectedItemType = so;
        obj.SetActive(false);
        UpdateGridMaterials();
        currentGrid.RebakeGrid();
        StartPlacing(so.decorationPrefab, so);
        if (objectPreview != null)
            objectPreview.transform.rotation = rot;
    }

    public void PutDecorationAway(GameObject obj, DecorationItemSO so, bool tween = true)
    {
        if (!obj || !so) return;

        if (!movingObject)
            Inventory.AddItem(so.itemName);

        decorationsInStore.Remove(obj.GetComponent<Decoration>());

        if (tween) obj.gameObject.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() => DestroyDecoration(obj));
        else DestroyDecoration(obj);

        PlayerStats.stats.roomDecorationCount = decorationsInStore.Count;
        PlayerStats.stats.tankCount = tanksInStore.Count;
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
        foreach (RoomGridNode n in nodes.Keys)
        {
            MeshRenderer m;
            nodes[n].TryGetComponent(out m);
            if (m) m.material = decoratingGridMat;
        }
    }




    public void LoadDecorations(RoomDecorationSaveData[] data, TankSaveData[] tankData)
    {
        if (data == null || data.Length == 0) return;

        foreach (Decoration d in decorationParent.transform.GetComponentsInChildren<Decoration>())
        {
            Destroy(d.gameObject);
        }

        tanksInStore = new List<TankController>();

        foreach (RoomDecorationSaveData decorationSave in data)
        {
            GameObject d = GameObject.Instantiate(((DecorationItemSO)Inventory.GetSOForItem(Inventory.GetItemUsingName(decorationSave.name))).decorationPrefab, 
                new Vector3(decorationSave.position.X, decorationSave.position.Y, decorationSave.position.Z), 
                Quaternion.Euler(new Vector3(decorationSave.rotation.X, decorationSave.rotation.Y, decorationSave.rotation.Z)), 
                decorationParent);

            decorationsInStore.Add(d.GetComponent<Decoration>());

            // Set up decoration
            Decoration decoration;
            d.TryGetComponent(out decoration);
            if (decoration != null) decoration.locked = decorationSave.locked;

            if (decorationSave.tankSaveReference != -1)
            {
                TankController t = null;
                d.TryGetComponent(out t);
                if (t != null) LoadTank(t, tankData[decorationSave.tankSaveReference]);
            }
        }

        PlayerStats.stats.tankCount = tanksInStore.Count;
        PlayerStats.stats.roomDecorationCount = decorationsInStore.Count;
        currentGrid.RebakeGrid();
        StopPlacing();
    }


    public void LoadTank(TankController tank, TankSaveData data)
    {
        tank.tankName = data.tankName;
        tank.tankId = data.tankId;
        tank.AlarmIds = data.alarmIds.ToList<String>();

        if (data.openTank) tank.toggleTankOpen();

        foreach (ShrimpStats s in data.shrimp)
        {
            tank.SpawnShrimp(s, true);
        }

        DecorateTankController.Instance.LoadDecorations(tank, data);

        tank.GetComponent<TankUpgradeController>().LoadUpgrades(data.upgradeIDs);
        tank.upgradeState = data.upgradeState;

        if (data.waterFilled) tank.FillWater();
        else tank.EmptyWater();

        tank.waterTemperature = data.waterTemp;
        tank.waterQuality = data.waterQuality;
        tank.waterSalt = data.waterSalt;
        tank.waterAmmonium = data.waterHNC;
        tank.waterPh = data.waterPH;
        tank.dayLastFed = data.fedToday;
        tank.starvationTime = data.starvationTime;
        tank.starvationTimer = data.starvationTimer;
        foreach (FoodSaveData foodSave in data.shrimpFood)
        {
            GameObject newFood = Instantiate(((FoodItemSO)Inventory.GetSOForItem(foodSave.thisItem)).foodPrefab, foodSave.position, Quaternion.identity);
            newFood.GetComponent<ShrimpFood>().CreateFood(tank, foodSave);
        }

        tank.tankLoaded = true;
        tanksInStore.Add(tank);
    }
}
