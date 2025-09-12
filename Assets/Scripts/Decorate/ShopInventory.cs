using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine.Windows;
using AYellowpaper.SerializedCollections;
using System.Linq;

public enum InventoryTabs
{
    All,
    Decorations,
    Shrimp,
    Supplies,
    Tools
}

public class ShopInventory : ScreenView
{
    private DecorateShopController shop;
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _contentBlock;
    private List<DecorationContentBlock> contentBlocks = new List<DecorationContentBlock>();

    [Header("Info Box")]
    [SerializeField] private CanvasGroup infoCanvasGroup;
    [SerializeField] GameObject selectedItemInfoPanel;
    [SerializeField] TMP_Text selectedItemNameText;
    [SerializeField] Image selectedItemImage;
    [SerializeField] TMP_Text selectedItemDescriptionText;
    [SerializeField] TMP_Text selectedItemQuantityText;
    [SerializeField] TMP_Text selectedItemPriceText;
    [SerializeField] TMP_Text selectedItemSizeText;
    [SerializeField] TMP_Text selectedItemSurfacesText;
    [SerializeField] float infoFadeSpeed = 0.5f;

    [Header("Shrimp Info Box")]
    [SerializeField] GameObject selectedShrimpInfoPanel;
    [SerializeField] TMP_Text selectedShrimpNameText;
    [SerializeField] TMP_Text selectedShrimpBreedText;
    [SerializeField] TMP_Text selectedShrimpValueText;
    [SerializeField] Image selectedShrimpPrimaryColour;
    [SerializeField] Image selectedShrimpSecondaryColour;
    [SerializeField] TMP_Text selectedShrimpGenderText;
    [SerializeField] TMP_Text selectedShrimpPatternText;
    [SerializeField] TMP_Text selectedShrimpHeadText;
    [SerializeField] TMP_Text selectedShrimpBodyText;
    [SerializeField] TMP_Text selectedShrimpEyesText;
    [SerializeField] TMP_Text selectedShrimpLegsText;
    [SerializeField] TMP_Text selectedShrimpTailText;
    [SerializeField] TMP_Text selectedShrimpTailFanText;

    [Header("Filters")]
    [SerializedDictionary("Button", "Filters")]
    [SerializeField] List<Button> tabs;
    public SerializedDictionary<Button, ShopInventoryFilter> tabFilters;
    private Button currentTab;
    public Color tabDeselectedColour, tabSelectedColour;



    [HideInInspector] public ItemSO selectedItemType;
    [HideInInspector] public ShrimpStats selectedShrimp;
    [HideInInspector] public GameObject selectedItemGameObject;


    public override void Open(bool switchTab)
    {
        player = Store.player;
        shop = Store.decorateController;
        selectedItemType = null;
        selectedItemGameObject = null;
        infoCanvasGroup.alpha = 0;
        base.Open(switchTab);
    }


    public void OpenInventory(List<InventoryTabs> t)
    {
        int startingTab = -1;
        if (t != null && t.Count != 0)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].gameObject.SetActive(t.Contains((InventoryTabs)i));
                if (startingTab == -1 && t.Contains((InventoryTabs)i)) startingTab = i;
            }
        }

        Store.player.GetComponent<HeldItem>().StopHoldingItem();
        ChangeSelectedItem(null, null);
        ChangeTab(tabs[startingTab]);
        StartCoroutine(OpenTab(false));
        UpdateContent();
    }

    private void OnEnable()
    {
        UpdateContent(); 
    }




    public void UpdateContent()
    {
        foreach (Transform child in _content.transform)
        {
            Destroy(child.gameObject);
        }

        contentBlocks.Clear();

        List<Item> items = Inventory.GetInventory(true, false);
        items = items.Concat(Inventory.GetShrimpInventory()).ToList();
        items = Inventory.SortItemsByQuantityThenName(items);

        // Filter items here
        if (currentTab != null && tabFilters[currentTab] != null)
        {
            items = Inventory.FilterItemsWithTags(items, GetTabFilters(currentTab));
            items = Inventory.FilterItemsWithTags(items, tabFilters[currentTab].GetActiveSubFilters());
        }

        foreach (Item i in items)
        {
            ShrimpItem shrimp = i as ShrimpItem;

            DecorationContentBlock content = Instantiate(_contentBlock, _content.transform).GetComponent<DecorationContentBlock>();
            contentBlocks.Add(content);

            if (shrimp == null)
            { 
                ItemSO so = Inventory.GetSOForItem(i);

                if (so == null)
                {
                    Debug.LogWarning("Cannot find SO for " + i.itemName);
                    return;
                }

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
                        ChangeSelectedItem(so, content.gameObject);
                    }
                    else  // If it is already selected
                    {
                        DecorationItemSO d = so as DecorationItemSO;
                        MedicineItemSO m = so as MedicineItemSO;
                        FoodItemSO f = so as FoodItemSO;
                        if (d != null)
                        {
                            shop.StartPlacing(d.decorationPrefab, d);
                            Close();
                            return;
                        }
                        else if (m != null || f != null) 
                        {
                            Store.player.GetComponent<HeldItem>().HoldItem(i);
                            Close();
                            return;
                        }
                    }

                    UpdateContent();
                });
            }

            else  // Shrimp Item
            {
                content.SetText(shrimp.shrimp.GetBreedname());

                content.ownedText.gameObject.SetActive(false);
                content.priceText.gameObject.SetActive(false);

                if (selectedShrimp.name == shrimp.shrimp.name && selectedShrimp.birthTime == shrimp.shrimp.birthTime) content.buttonSprite.color = content.selectedColour;
                else if (i.quantity > 0) content.buttonSprite.color = content.inInventoryColour;
                else content.buttonSprite.color = content.notInInventoryColour;

                content.button.onClick.AddListener(() =>
                {
                    if (content.buttonSprite.color != content.selectedColour)  // If it isn't already selected
                    {
                        ChangeSelectedItem(shrimp.shrimp);
                    }
                    else  // If it is already selected
                    {
                        Store.player.GetComponent<HeldItem>().HoldItem(i);
                        Close();
                        return;
                    }

                    UpdateContent();
                });
            }
        }
    }

    public void ChangeSelectedItem(ItemSO so, GameObject obj)
    {
        selectedItemType = so;
        selectedShrimp = new ShrimpStats();
        selectedItemGameObject = obj;
        selectedItemInfoPanel.SetActive(true);
        selectedShrimpInfoPanel.SetActive(false);

        if (selectedItemType != null)
        {
            selectedItemNameText.text = selectedItemType.itemName;
            selectedItemImage.sprite = selectedItemType.itemImage;
            selectedItemDescriptionText.text = selectedItemType.itemDescription;
            selectedItemQuantityText.text = "x" + Inventory.GetItemUsingSO(selectedItemType).quantity;
            selectedItemPriceText.text = "£" + selectedItemType.purchaseValue / selectedItemType.purchaseQuantity;

            DecorationItemSO d = selectedItemType as DecorationItemSO;
            if (d != null)
            {
                selectedItemSizeText.text = d.itemSize.ToString();

                List<string> surfaces = new List<string>();
                foreach (PlacementSurfaces s in d.placementSurfaces) surfaces.Add(s.ToString());
                selectedItemSurfacesText.text = string.Join(", ", surfaces);
            }
            else
            {
                selectedItemSizeText.text = "";
                selectedItemSurfacesText.text = "";
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

    public void ChangeSelectedItem(ShrimpStats shrimp)
    {
        selectedShrimp = shrimp;
        selectedItemType = null;
        selectedItemGameObject = null;
        selectedItemInfoPanel.SetActive(false);
        selectedShrimpInfoPanel.SetActive(true);

        if (selectedShrimp.name != "")
        {
            selectedShrimpNameText.text = shrimp.name;
            selectedShrimpBreedText.text = shrimp.GetBreedname();
            selectedShrimpValueText.text = "£" + EconomyManager.instance.GetShrimpValue(shrimp).RoundMoney().ToString();
            selectedShrimpPrimaryColour.color = GeneManager.instance.GetTraitSO(shrimp.primaryColour.activeGene.ID).colour;
            selectedShrimpSecondaryColour.color = GeneManager.instance.GetTraitSO(shrimp.secondaryColour.activeGene.ID).colour;
            selectedShrimpGenderText.text = shrimp.sex ? "Male" : "Female";
            selectedShrimpPatternText.text = GeneManager.instance.GetTraitSO(shrimp.pattern.activeGene.ID).traitName;
            selectedShrimpHeadText.text = GeneManager.instance.GetTraitSO(shrimp.head.activeGene.ID).set.ToString();
            selectedShrimpBodyText.text = GeneManager.instance.GetTraitSO(shrimp.body.activeGene.ID).set.ToString();
            selectedShrimpEyesText.text = GeneManager.instance.GetTraitSO(shrimp.eyes.activeGene.ID).set.ToString();
            selectedShrimpLegsText.text = GeneManager.instance.GetTraitSO(shrimp.legs.activeGene.ID).set.ToString();
            selectedShrimpTailText.text = GeneManager.instance.GetTraitSO(shrimp.tail.activeGene.ID).set.ToString();
            selectedShrimpTailFanText.text = GeneManager.instance.GetTraitSO(shrimp.tailFan.activeGene.ID).set.ToString();

            infoCanvasGroup.DOKill();
            infoCanvasGroup.DOFade(1, infoFadeSpeed).SetEase(Ease.InOutSine);
        }
        else
        {
            infoCanvasGroup.DOKill();
            infoCanvasGroup.DOFade(0, infoFadeSpeed).SetEase(Ease.InOutSine);
        }
    }


    public void ChangeTab(Button b)
    {
        foreach(Button x in tabFilters.Keys)
        {
            x.GetComponent<Image>().color = tabDeselectedColour;
            tabFilters[x].gameObject.SetActive(false);
        }

        b.GetComponent<Image>().color = tabSelectedColour;
        tabFilters[b].gameObject.SetActive(true);
        tabFilters[b].DeselectAllFilters();
        currentTab = b;

        ChangeSelectedItem(null, null);
        UpdateContent();
    }

    private List<ItemTags> GetTabFilters(Button button)
    {
        if (tabFilters[button].allTab)
        {
            List<ItemTags> tags = new List<ItemTags>();
            foreach (Button filter in tabFilters.Keys)
            {
                if (filter.gameObject.activeSelf)
                    tags = tags.Concat(tabFilters[filter].GetTabFilters()).ToList();
            }
            return tags;
        }

        return tabFilters[currentTab].GetTabFilters();
    }



    public override void Close(bool switchTab) { CloseScreen(); }
    public override void Close() { CloseScreen(); }

    private void CloseScreen()
    {
        UIManager.instance.GetCursor().SetActive(false);
        UIManager.instance.GetTooltips().SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        UIManager.instance.input.SwitchCurrentActionMap("Move");
        base.Close();
    }


    public void UISelect()
    {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Empty");
    }

    public void UIUnselect()
    {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
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
}