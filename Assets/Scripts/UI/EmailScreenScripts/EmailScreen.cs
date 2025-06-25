using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailScreen : ScreenView
{
    [SerializeField] private GameObject ShrimpSelection;
    [SerializeField] private CanvasGroup window;

    private ShrimpSelectionWindow activeSelection;

    protected override void Start()
    {
        base.Start();
        CustomerManager.Instance.EmailOpen(this);
    }

    public void OpenSelection(Request request)
    {
        ShrimpSelectionWindow screen = Instantiate(ShrimpSelection, transform).GetComponent<ShrimpSelectionWindow>();
        activeSelection = screen;
        window.interactable = false;
        screen.Populate(request, this);
    }

    public void OpenFullSelection(float price, Email email)
    {
        ShrimpSelectionWindow screen = Instantiate(ShrimpSelection, transform).GetComponent<ShrimpSelectionWindow>();
        activeSelection = screen;
        window.interactable = false;
        screen.PopulateFull(price, this, email);
    }

    public void CloseSelection()
    {
        Destroy(activeSelection.gameObject);
        window.interactable = true;
    }

}
