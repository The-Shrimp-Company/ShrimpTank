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
    private GameObject hoveredNode, selectedNode;


    [Header("Grid")]
    public GameObject decoratingGridPrefab;
    public Material decoratingGridMat;
    public Material decoratingGridHovered;
    public Material decoratingGridValidMat;
    public Material decoratingGridInvalidMat;


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
            if (hoveredNode == ray.collider.gameObject) return;


            foreach (GameObject node in bottomNodes.Values)
            {
                if (node == ray.collider.gameObject)
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


    private void ChangeHoveredNode(GameObject node)
    {
        foreach (GameObject n in bottomNodes.Values)
        {
            n.GetComponent<MeshRenderer>().material = decoratingGridMat;
        }

        hoveredNode = node;
        if (hoveredNode != null)
        {
            hoveredNode.GetComponent<MeshRenderer>().material = decoratingGridHovered;
        }
    }


    //public void MouseClick(Vector3 point, bool pressed)
    //{
    //    RaycastHit ray;
    //    if (Physics.Raycast(Camera.main.ScreenPointToRay(point), out ray, 3f, LayerMask.GetMask("Decoration")))
    //    {
    //        if (tank.decorationsInTank.Contains(ray.transform.gameObject))
    //        {
    //            DecorationItem item = ray.transform.GetComponent<DecorationItem>();
    //            if (item == null)
    //            {
    //                Debug.Log(ray.transform.name + " prefab is missing a decoration item script");
    //                return;
    //            }

    //            selectedItemGameObject = ray.transform.gameObject;
    //            selectedItemType = Inventory.GetSOForItem(item) as DecorationItemSO;
    //            ChangeSelectedItem();
    //        }
    //    }
    //}
}
