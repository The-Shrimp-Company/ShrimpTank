using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;

public class DecorateTankController
{
    private static TankController currentTank;
    private static TankGrid currentGrid;

    private static List<GameObject> bottomNodes = new List<GameObject>();

    public static void StartDecorating(TankController t)
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
                GameObject node = GameObject.Instantiate(currentGrid.decoratingGridPrefab, currentGrid.grid[w][h][l].worldPos + new Vector3(0, -(currentGrid.pointSize / 2), 0), Quaternion.identity);
                node.transform.parent = currentTank.transform;
                node.transform.localScale = new Vector3(currentGrid.pointSize, currentGrid.pointSize / 10, currentGrid.pointSize);
                bottomNodes.Add(node);
            }
        }
    }


    public static void StopDecorating()
    {
        currentTank = null;
        currentGrid = null;

        currentTank.waterObject.SetActive(true);


        foreach(GameObject n in bottomNodes)
        {
            GameObject.Destroy(n);
        }
        bottomNodes.Clear();
    }





    //public static void Update()
    //{
    //    MouseDetection();
    //}


    //public void MouseDetection()
    //{
    //    RaycastHit ray;
    //    if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current), out ray, 3f, LayerMask.GetMask("Decoration")))
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
