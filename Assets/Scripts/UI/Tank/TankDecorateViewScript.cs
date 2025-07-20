using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System;
using System.Xml;
using DG.Tweening;

public class TankDecorateViewScript : ScreenView
{
    protected TankController tank;
    [SerializeField] private GameObject leftPanel;
    protected Shrimp _shrimp;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _contentBlock;
    private List<DecorationContentBlock> contentBlocks = new List<DecorationContentBlock>();

    private DecorationItemSO selectedItemType;
    private GameObject selectedItemGameObject;


    public override void Open(bool switchTab)
    {
        Debug.Log("Opening tank decorate");
        player = GameObject.Find("Player");
        shelves = GetComponentInParent<ShelfSpawn>();
        tank = GetComponentInParent<TankController>();
        tank.tankDecorateViewScript = this;
        selectedItemType = null;
        selectedItemGameObject = null;
        ChangeSelectedItem();
        StartCoroutine(OpenTab(switchTab));
        UpdateContent();
        base.Open(switchTab);
    }

    private void OnEnable()
    {
        UpdateContent();
    }

    public virtual void Update()
    {
        
    }


    public void SelectAll()
    {
        //if (allSelected)
        //{
        //    selectedShrimp = new List<Shrimp>();
        //    foreach(TankContentBlock block in contentBlocks)
        //    {
        //        block.checkbutton.GetComponent<Checkbox>().Uncheck();
        //    }
        //    multiSelect.Uncheck();
        //}
        //else
        //{
        //    foreach (Shrimp shrimp in tank.shrimpInTank)
        //    {
        //        selectedShrimp.Add(shrimp);
        //    }
        //    foreach(TankContentBlock block in contentBlocks)
        //    {
        //        block.checkbutton.GetComponent<Checkbox>().Check();
        //    }
        //    multiSelect.Check();
        //}
        //allSelected = !allSelected;
    }

    public void UpdateContent()
    {
        Debug.Log("Updating tank decorate view content");
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }

        contentBlocks.Clear();

        List<Item> items = Inventory.GetInventory(false, true);

        // Filter items here


        foreach (DecorationItem i in items)
        {
            DecorationItemSO so = Inventory.GetSOForItem(i) as DecorationItemSO;

            if (so == null)
            {
                Debug.LogWarning("Cannot find SO for " + i.itemName);
                return;
            }
             
            DecorationContentBlock content = Instantiate(_contentBlock, _content.transform).GetComponent<DecorationContentBlock>();
            contentBlocks.Add(content);
            content.main.onClick.AddListener(() =>
            {
                //GameObject newitem = Instantiate(shrimpView);
                //UIManager.instance.OpenScreen(newitem.GetComponent<ScreenView>());
                //newitem.GetComponent<ShrimpView>().Populate(thisShrimp);
                //thisShrimp.GetComponentInChildren<ShrimpCam>().SetCam();
                //newitem.GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
                //newitem.GetComponent<Canvas>().planeDistance = 1;
                //UIManager.instance.SetCursorMasking(false);
            });
        }
    }

    public void ChangeSelectedItem()
    {
        
    }



    
    public void MouseClick(Vector3 point, bool pressed)
    {
        RaycastHit ray;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(point), out ray, 3f, LayerMask.GetMask("Decoration")))
        {
            if (tank.decorationsInTank.Contains(ray.transform.gameObject))
            {
                DecorationItem item = ray.transform.GetComponent<DecorationItem>();
                if (item == null) 
                {
                    Debug.Log(ray.transform.name + " prefab is missing a decoration item script");
                    return;
                }

                selectedItemGameObject = ray.transform.gameObject;
                selectedItemType = Inventory.GetSOForItem(item) as DecorationItemSO;
                ChangeSelectedItem();
            }
        }
    }


    public void UISelect()
    {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Empty");
    }

    public void UIUnselect()
    {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("TankView");
    }

    public override void Close(bool switchTab)
    {
        StartCoroutine(CloseTab(switchTab));
    }

    public IEnumerator OpenTab(bool switchTab)
    {
        UIManager.instance.SetCursorMasking(true);  // Enable cursor masking
        yield return new WaitForSeconds(0);

        //if ((switchTab && switchAnimationSpeed != 0) || (!switchTab && openAnimationSpeed != 0))  // Setting anim speed to 0 disables the animation
        //{
        //    // Move the panels offscreen depending on if we are switching or opening a new tab
        //    leftPanel.transform.localPosition = switchTab ? leftPanelSwitchInPos : leftPanelClosePos;
        //    upgradeBox.transform.localPosition = switchTab ? upgradeBoxSwitchInPos : upgradeBoxClosePos;

        //    // Temporarily hide extra sections that shouldn't be showing
        //    upgradeBox.enabled = false;
        //    contextBox.enabled = false;
        //    contextBox.gameObject.SetActive(false);

        //    if (switchTab)  // If we are switching from another menu
        //    {
        //        yield return new WaitForSeconds(switchAnimationSpeed / 1.2f);  // Wait for the other one to close partially

        //        // Move this menu onto the screen
        //        leftPanel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, switchAnimationSpeed).SetEase(switchAnimationEase, 1.4f);
        //        upgradeBox.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, switchAnimationSpeed).SetEase(switchAnimationEase, 1.4f);

        //        yield return new WaitForSeconds(switchAnimationSpeed);
        //    }
        //    else  // If we don't have a menu open already
        //    {
        //        GetComponent<CanvasGroup>().alpha = 0f;  // Make the menu transparent
        //        GetComponent<CanvasGroup>().DOFade(1, openAnimationSpeed).SetEase(Ease.OutCubic);  // Fade the menu in

        //        // Move this menu onto the screen
        //        leftPanel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, openAnimationSpeed).SetEase(Ease.OutBack);
        //        upgradeBox.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, openAnimationSpeed).SetEase(Ease.OutBack);

        //        yield return new WaitForSeconds(openAnimationSpeed);
        //    }
        //}

        //// Enable all parts that we disabled earlier
        //contextBox.gameObject.SetActive(true);
        //upgradeBox.enabled = true;
        //contextBox.enabled = true;
    }



    public IEnumerator CloseTab(bool switchTab)
    {
        UIManager.instance.SetCursorMasking(true);  // Enable cursor masking
        yield return new WaitForSeconds(0);
        //if ((switchTab && switchAnimationSpeed != 0) || (!switchTab && openAnimationSpeed != 0))  // Setting anim speed to 0 disables the animation
        //{
        //    // End currently running tweens
        //    DOTween.Kill(leftPanel);

        //    // Remove extra sections of the menu
        //    upgradeBox.enabled = false;
        //    contextBox.enabled = false;

        //    if (switchTab)  // If we are switching to another menu
        //    {
        //        // Move the panels offscreen
        //        leftPanel.GetComponent<RectTransform>().DOAnchorPos(leftPanelSwitchOutPos, switchAnimationSpeed).SetEase(switchAnimationEase, 1.4f);
        //        upgradeBox.GetComponent<RectTransform>().DOAnchorPos(upgradeBoxSwitchOutPos, switchAnimationSpeed).SetEase(switchAnimationEase, 1.4f);
        //        contextBox.GetComponent<RectTransform>().DOAnchorPos(upgradeBoxSwitchOutPos, switchAnimationSpeed).SetEase(switchAnimationEase, 1.4f);

        //        yield return new WaitForSeconds(switchAnimationSpeed);
        //    }
        //    else  // If we are fully closing the menu
        //    {
        //        // Move the panels offscreen
        //        leftPanel.GetComponent<RectTransform>().DOAnchorPos(leftPanelClosePos, openAnimationSpeed).SetEase(Ease.InBack);
        //        upgradeBox.GetComponent<RectTransform>().DOAnchorPos(upgradeBoxClosePos, openAnimationSpeed).SetEase(Ease.InBack);
        //        contextBox.GetComponent<RectTransform>().DOAnchorPos(upgradeBoxClosePos, openAnimationSpeed).SetEase(Ease.InBack);
                
        //        GetComponent<CanvasGroup>().DOFade(0, openAnimationSpeed).SetEase(Ease.InCubic);  // Fade the menu out

        //        yield return new WaitForSeconds(openAnimationSpeed);
        //    }

        //    // End the tweens
        //    DOTween.Kill(leftPanel);
        //    DOTween.Kill(upgradeBox);
        //    DOTween.Kill(contextBox);
        //}


        base.Close(switchTab);
    }

    public TankController GetTank() {  return tank; }
}