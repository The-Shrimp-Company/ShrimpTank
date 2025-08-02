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
using System.Threading.Tasks;

public class TankDecorateViewScript : ScreenView
{
    protected TankController tank;
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _contentBlock;
    private List<DecorationContentBlock> contentBlocks = new List<DecorationContentBlock>();
    [SerializeField] private CanvasGroup infoCanvasGroup;

    [SerializeField] GameObject selectedItemInfoBox;
    [SerializeField] TMP_Text selectedItemNameText;
    [SerializeField] Image selectedItemImage;
    [SerializeField] GameObject selectedItemButtons;
    [SerializeField] GameObject placingItemButtons;

    [SerializeField] float infoFadeSpeed = 0.5f;

    [HideInInspector] public DecorationItemSO selectedItemType;
    [HideInInspector] public GameObject selectedItemGameObject;


    public override void Open(bool switchTab)
    {
        Debug.Log("Opening tank decorate");
        player = GameObject.Find("Player");
        shelves = GetComponentInParent<ShelfSpawn>();
        tank = GetComponentInParent<TankController>();
        tank.tankDecorateViewScript = this;
        selectedItemType = null;
        selectedItemGameObject = null;
        infoCanvasGroup.alpha = 0;
        ChangeSelectedItem(null, null);
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
        items = Inventory.FilterItemsWithTag(items, ItemTags.TankDecoration);


        foreach (Item i in items)
        {
            DecorationItemSO so = Inventory.GetSOForItem(i) as DecorationItemSO;

            if (so == null)
            {
                Debug.LogWarning("Cannot find SO for " + i.itemName);
                return;
            }


            if (DecorateTankController.Instance.editingTop && so.canFloat == FloatingItem.Grounded) continue;
            if (!DecorateTankController.Instance.editingTop && so.canFloat == FloatingItem.Floats) continue;


            DecorationContentBlock content = Instantiate(_contentBlock, _content.transform).GetComponent<DecorationContentBlock>();
            contentBlocks.Add(content);
            content.SetText(i.itemName);
            content.SetDecoration(so);
            content.ownedText.text = i.quantity.ToString();
            content.priceText.text = "£" + so.purchaseValue.ToString();

            if (selectedItemType == so) content.buttonSprite.color = content.selectedColour;
            else if (i.quantity > 0) content.buttonSprite.color = content.inInventoryColour;
            else if (so.purchaseValue <= Money.instance.money) content.buttonSprite.color = content.notInInventoryColour;
            else if (so.purchaseValue > Money.instance.money) content.buttonSprite.color = content.cannotAffordColour;

            content.button.onClick.AddListener(() =>
            {
                if (content.buttonSprite.color != content.selectedColour)  // If it isn't already selected
                {
                    DecorateTankController.Instance.decorateView = this;
                    DecorateTankController.Instance.StartPlacing(so.decorationPrefab);

                    ChangeSelectedItem(so, content.gameObject);
                }
                else  // If it is already selected
                {
                    DecorateTankController.Instance.StopPlacing();

                    ChangeSelectedItem(null, null);
                }

                UpdateContent();
            });
        }
    }

    public void ChangeSelectedItem(DecorationItemSO so, GameObject obj)
    {
        selectedItemType = so;
        selectedItemGameObject = obj;

        if (selectedItemType != null)
        {
            selectedItemNameText.text = selectedItemType.itemName;
            selectedItemImage.sprite = selectedItemType.itemImage;

            if (DecorateTankController.Instance.placementMode)
            {
                placingItemButtons.SetActive(true);
                selectedItemButtons.SetActive(false);
            }
            else
            {
                placingItemButtons.SetActive(false);
                selectedItemButtons.SetActive(true);
            }

            infoCanvasGroup.DOKill();
            infoCanvasGroup.DOFade(1, infoFadeSpeed).SetEase(Ease.InOutSine);
        }
        else
        {
            infoCanvasGroup.DOKill();
            infoCanvasGroup.DOFade(0, infoFadeSpeed).SetEase(Ease.InOutSine);
        }
    }


    public async void ClearTank()
    {
        int delay = 400 / tank.decorationsInTank.Count;
        for (int i = tank.decorationsInTank.Count - 1; i >= 0; i--)
        {
            if (tank.decorationsInTank[i] == null) continue;
            DecorateTankController.Instance.selectedObject = tank.decorationsInTank[i];
            ChangeSelectedItem(tank.decorationsInTank[i].GetComponent<Decoration>().decorationSO, tank.decorationsInTank[i]);
            PutAway();
            await Task.Delay(delay);
        }
    }


    public void ChangeEditLayer(bool top)
    {
        ChangeSelectedItem(null, null);
        DecorateTankController.Instance.ChangeEditLayer(top);
        UpdateContent();
    }


    public void ChangeCamera()
    {
        DecorateTankController.Instance.ChangeCam(1);
    }

    public void ToggleDecorationTransparency()
    {
        DecorateTankController.Instance.ToggleTransparentDecorarions();
    }

    public void ToggleShrimpTransparency()
    {
        DecorateTankController.Instance.ToggleTransparentShrimp();
    }

    public void PutAway()
    {
        DecorateTankController.Instance.PutDecorationAway();
    }

    public void Move()
    {
        DecorateTankController.Instance.MoveDecoration();
    }

    public void Rotate()
    {
        DecorateTankController.Instance.RotateObject();
    }


    public override void Close(bool switchTab) { CloseScreen(); }
    public override void Close() { CloseScreen(); }

    private void CloseScreen()
    {
        Camera.main.transform.position = tank.GetCam().transform.position;
        Camera.main.transform.rotation = tank.GetCam().transform.rotation;
        DecorateTankController.Instance.StopDecorating();
        UIManager.instance.SetCursorMasking(true);
        base.Close();
    }


    public void UISelect()
    {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Empty");
    }

    public void UIUnselect()
    {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("TankView");
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


    public TankController GetTank() {  return tank; }
}