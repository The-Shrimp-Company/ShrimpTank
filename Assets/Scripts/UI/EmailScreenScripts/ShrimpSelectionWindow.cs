using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrimpSelectionWindow : MonoBehaviour
{
    public ShrimpSelectionPopulation screen;

    public void Populate(Request request, EmailScreen emailScreen)
    {
        screen.Populate(request, emailScreen);
    }

    public void PopulateFull(float price, EmailScreen emailScreen, Email email)
    {
        screen.PopulateFull(price, emailScreen, email);
    }

    public void PopulateExcluding(EmailScreen emailScreen, Email email, List<ShrimpStats> shrimpToExclude)
    {
        screen.PopulateExcluding(emailScreen, email, shrimpToExclude);
    }

    public ShrimpSelectionWindow PopulateForShrimpSale(ShrimpSlotScript shrimpSlot)
    {
        screen.PopulateForSaleSelection(shrimpSlot);
        return this;
    }
}
