using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using AYellowpaper.SerializedCollections;
using System.Linq;


public class ItemShop : ScreenView
{
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject _content;
    [SerializeField] private GameObject _contentBlock;
    private List<DecorationContentBlock> contentBlocks = new List<DecorationContentBlock>();

    [Header("Info Box")]
    [SerializeField] private CanvasGroup infoCanvasGroup;
    [SerializeField] TMP_Text selectedItemNameText;
    [SerializeField] Image selectedItemImage;
    [SerializeField] TMP_Text selectedItemDescriptionText;
    [SerializeField] TMP_Text selectedItemQuantityText;
    [SerializeField] TMP_Text selectedItemPriceText;
    [SerializeField] TMP_Text selectedItemSizeText;
    [SerializeField] TMP_Text selectedItemSurfacesText;
    [SerializeField] float infoFadeSpeed = 0.5f;

    [Header("Filters")]
    [SerializedDictionary("Button", "Filters")]
    [SerializeField] List<Button> tabs;
    public SerializedDictionary<Button, ItemShopFilter> tabFilters;
    private Button currentTab;
    public Color tabDeselectedColour, tabSelectedColour;



    [HideInInspector] public ItemSO selectedItemType;
    [HideInInspector] public GameObject selectedItemGameObject;


    public override void Open(bool switchTab)
    {
        player = Store.player;
        selectedItemType = null;
        selectedItemGameObject = null;
        infoCanvasGroup.alpha = 0;
        ChangeSelectedItem(null, null);
        ChangeTab(tabs[0]);
        UIManager.instance.SetCursorMasking(true);  // Enable cursor masking
        UpdateContent();
        base.Open(switchTab);
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

        List<Item> items = Inventory.GetInventory(false, false);
        items = items.Concat(Inventory.GetShrimpInventory()).ToList();
        items = Inventory.SortItemsByName(items);

        // Filter items here
        if (currentTab != null && tabFilters[currentTab] != null)
        {
            items = Inventory.FilterItemsWithTags(items, GetTabFilters(currentTab));
            foreach (ItemTags tag in tabFilters[currentTab].GetActiveSubFilters())
            {
                items = Inventory.FilterItemsWithTag(items, tag);
            }
        }

        foreach (Item i in items)
        {

            DecorationContentBlock content = Instantiate(_contentBlock, _content.transform).GetComponent<DecorationContentBlock>();
            contentBlocks.Add(content);

            ItemSO so = Inventory.GetSOForItem(i);

            if (so == null)
            {
                Debug.LogWarning("Cannot find SO for " + i.itemName);
                return;
            }
            else if (so.reputationUnlockRequirement > Reputation.GetReputation()) return;  // Item not unlocked

            if (so.itemImage == null)
            {
                content.SetText(i.itemName);
                content.itemImage.gameObject.SetActive(false);
            }
            else
            {
                content.SetText("");
                content.itemImage.sprite = so.itemImage;
            }

            content.SetDecoration(so);
            content.ownedText.text = i.quantity.ToString();
            content.priceText.text = "£" + so.purchaseValue.ToString();

            if (selectedItemType == so) content.buttonSprite.color = content.selectedColour;
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
                    if (Money.instance.WithdrawMoney(so.purchaseValue))
                    {
                        Inventory.AddItem(Inventory.GetItemUsingSO(so), so.purchaseQuantity);
                        
                        if (so.tags.Contains(ItemTags.Food)) PlayerStats.stats.timesBoughtFood++;
                    }
                }

                UpdateContent();
            });
        }
    }

    public void ChangeSelectedItem(ItemSO so, GameObject obj)
    {
        selectedItemType = so;
        selectedItemGameObject = obj;

        if (selectedItemType != null)
        {
            selectedItemNameText.text = selectedItemType.itemName;
            selectedItemImage.sprite = selectedItemType.itemImage;
            if (selectedItemType == null) selectedItemImage.gameObject.SetActive(false);
            selectedItemDescriptionText.text = selectedItemType.itemDescription;
            selectedItemQuantityText.text = "x" + selectedItemType.purchaseQuantity.ToString();

            if (selectedItemType.purchaseQuantity == 1) selectedItemPriceText.text = "£" + selectedItemType.purchaseValue;
            else selectedItemPriceText.text = "£" + selectedItemType.purchaseValue / selectedItemType.purchaseQuantity + " Each";

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
}