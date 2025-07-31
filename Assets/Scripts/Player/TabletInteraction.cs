using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabletInteraction : ScreenView
{
    [SerializeField]
    private GameObject SellButton, BuyButton, VetButton, UpgradeButton;


    [SerializeField]
    private GameObject SellScreen;
    [SerializeField]
    private GameObject BuyScreen;
    [SerializeField]
    private GameObject UpgradeScreen;
    [SerializeField]
    private GameObject InventoryScreen;
    [SerializeField]
    private GameObject EmailScreen;
    [SerializeField]
    private GameObject SettingsScreen;
    [SerializeField]
    private GameObject SaveScreen;
    [SerializeField]
    private GameObject VetScreen;

    protected override void Start()
    {
        base.Start();
    }


    public ShelfSpawn GetShelves() { return shelves; }

    private void Update()
    {
        BuyButton.SetActive(Tutorial.instance.flags.Contains("ShrimpStoreOpen"));
        SellButton.SetActive(Tutorial.instance.flags.Contains("OwnStoreOpen"));
        VetButton.SetActive(Tutorial.instance.flags.Contains("VetOpen"));
        UpgradeButton.SetActive(Tutorial.instance.flags.Contains("UpgradeStoreOpen"));
    }

    public void OpenSell()
    {
        GameObject sellScreen = Instantiate(SellScreen, transform.parent.transform);
        UIManager.instance.OpenScreen(sellScreen.GetComponent<ScreenView>());
        PlayerStats.stats.timesSellingAppOpened++;
    }

    public void OpenBuy()
    {
        GameObject buyScreen = Instantiate(BuyScreen, transform.parent.transform);
        UIManager.instance.OpenScreen(buyScreen.GetComponent<ScreenView>());
        PlayerStats.stats.timesShrimpShopAppOpened++;
    }

    public void OpenUpgrades()
    {
        GameObject upgradeScreen = Instantiate(UpgradeScreen, transform.parent.transform);
        UIManager.instance.OpenScreen(upgradeScreen.GetComponent<ScreenView>());
        PlayerStats.stats.timesItemShopAppOpened++;
    }

    public void OpenInventory()
    {
        GameObject inventoryScreen = Instantiate(InventoryScreen, transform.parent.transform);
        UIManager.instance.OpenScreen(inventoryScreen.GetComponent<ScreenView>());
        PlayerStats.stats.timesInventoryAppOpened++;
    }

    public void OpenEmails()
    {
        GameObject emailScreen = Instantiate(EmailScreen, transform.parent.transform);
        UIManager.instance.OpenScreen(emailScreen.GetComponent<ScreenView>());
        PlayerStats.stats.timesMailAppOpened++;
    }

    public void OpenSettings()
    {
        GameObject settingsScreen = Instantiate(SettingsScreen, transform.parent.transform);
        UIManager.instance.OpenScreen(settingsScreen.GetComponent<ScreenView>());
        PlayerStats.stats.timesSettingsAppOpened++;
    }

    public void OpenSave()
    {
        GameObject saveScreen = Instantiate(SaveScreen, transform.parent.transform);
        UIManager.instance.OpenScreen(saveScreen.GetComponent<ScreenView>());
    }

    public void OpenVet()
    {
        GameObject vetScreen = Instantiate(VetScreen, transform.parent.transform);
        UIManager.instance.OpenScreen(vetScreen.GetComponent<ScreenView>());
        PlayerStats.stats.timesVetOpened++;
    }
    
    public override void Close(bool switchTab)
    {

    }
}
