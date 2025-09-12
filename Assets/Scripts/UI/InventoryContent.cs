using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryContent : ContentPopulation
{
    [SerializeField] private GameObject algaeWafers, foodPellets;

    private void Awake()
    {
        UpdateContent(Inventory.GetInventory());
    }

    private void UpdateContent(List<Item> inventory)
    {
        ClearContent();

        if (inventory == null || inventory.Count == 0) return;

        CreateContent(inventory.Count);
        for (int i = 0; i < inventory.Count; i++)
        {
            contentBlocks[i].SetText(inventory[i].itemName);
            contentBlocks[i].GetComponent<InventoryContentBlock>().quantity.text = inventory[i].quantity.ToString();
            contentBlocks[i].GetComponent<InventoryContentBlock>().item = inventory[i];
        }
    }



    public void UpgradeAssignment(TankUpgradeController controller, UpgradeTypes type, ScreenView oldScreen, GameObject parent)
    {
        Button button = transform.parent.GetComponentInChildren<BackButton>().GetComponent<Button>();

        UpdateContent(Inventory.GetInventoryItemsWithTag(ItemTags.TankUpgrade));  // Create content blocks for all items with the upgrade tag

        foreach (InventoryContentBlock block in contentBlocks)
        {
            if(((UpgradeItemSO)Inventory.GetSOForItem(block.item)).upgradeType == type)
            {
                block.ClearFunctions();
                InventoryContentBlock thisBlock = block;
                TankUpgradeController thisController = controller;
                block.AssignFunction(() =>
                {
                    if (Inventory.HasItem(thisBlock.item))
                    {
                        if (thisController.CheckForUpgrade(type))
                        {
                            Inventory.AddItem(Inventory.GetItemUsingSO(thisController.GetUpgrade(type).upgrade));
                        }
                        thisController.AddUpgrade(Inventory.GetSOForItem(thisBlock.item) as UpgradeItemSO);
                        Inventory.RemoveItem(thisBlock.item);

                        UIManager.instance.CloseScreen();
                    }
                });
            }
            else
            {
                Destroy(block.gameObject);
            }
        }
    }


    public void FoodAssignement(TankViewScript oldScreen, TankController tank, GameObject parent)
    {
        Button button = transform.parent.GetComponentInChildren<BackButton>().GetComponent<Button>();

        UpdateContent(Inventory.GetInventoryItemsWithTag(ItemTags.Food));  // Create content blocks for all items with the food tag

        foreach (ContentBlock block in contentBlocks)
        {
            block.ClearFunctions();
            ContentBlock thisBlock = block;
            block.AssignFunction(() =>
            {
                if (Inventory.HasItem(((InventoryContentBlock)block).item))
                {
                    Inventory.RemoveItem(((InventoryContentBlock)block).item);
                    GameObject newFood = Instantiate(((FoodItemSO)Inventory.GetSOForItem(((InventoryContentBlock)block).item)).foodPrefab, tank.GetRandomSurfacePosition(), Quaternion.identity);
                    newFood.GetComponent<ShrimpFood>().CreateFood(tank, ((InventoryContentBlock)block).item);
                }
                ContentBlockUpdate((InventoryContentBlock)block);
            });
        }
    }


    public void MedAssignment(ScreenView oldScreen, Shrimp shrimp, GameObject parent)
    {
        Shrimp[] shrimpList = new Shrimp[] { shrimp };
        MedAssignment(oldScreen, shrimpList, parent);
    }


    public void MedAssignment(ScreenView oldScreen, Shrimp[] shrimp, GameObject parent)
    {
        Button button = transform.parent.GetComponentInChildren<BackButton>().GetComponent<Button>();
        oldScreen.gameObject.SetActive(false);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            UIManager.instance.CloseScreen();
        });

        UpdateContent(Inventory.GetInventoryItemsWithTag(ItemTags.Medicine));  // Create content blocks for all items with the medicine tag

        foreach (InventoryContentBlock block in contentBlocks)
        {
            block.ClearFunctions();
            InventoryContentBlock thisBlock = block;
            Shrimp[] thisShrimp = shrimp;
            block.AssignFunction(() =>
            {
                if(Inventory.GetItemQuantity(thisBlock.item) >= thisShrimp.Length)
                {
                    foreach (Shrimp shrimp in thisShrimp)
                    {
                        //shrimp.GetComponent<IllnessController>().UseMedicine(thisBlock.item as Medicine);
                    }
                    Inventory.RemoveItem(thisBlock.item, thisShrimp.Length);
                    ContentBlockUpdate(thisBlock);
                }
            });
        }
    }

    private void ContentBlockUpdate(InventoryContentBlock block)
    {
        block.GetComponent<InventoryContentBlock>().quantity.text = Inventory.GetItemQuantity(block.item).ToString();
        if (!Inventory.HasItem(block.item))
        {
            Destroy(block.gameObject);
        }
    }
}
