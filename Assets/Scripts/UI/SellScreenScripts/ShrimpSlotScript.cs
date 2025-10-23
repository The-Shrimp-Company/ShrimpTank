using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShrimpSlotScript : MonoBehaviour
{
    [SerializeField] private GameObject shrimpPreview;
    [SerializeField] private TMP_InputField price;
    [SerializeField] private GameObject shrimpSelectionScreen;
    private GameObject currentSelectionScreen;

    public int index;

    private Shrimp _shrimp;
    private ShrimpSelectionBlock currentPreview;

    private void Update()
    {
        if (CustomerManager.Instance.shrimpSaleSlots[index] == null || CustomerManager.Instance.shrimpSaleSlots[index].shrimp != _shrimp)
        {
            if (currentPreview != null) Destroy(currentPreview.gameObject);
            price.interactable = false;
            price.placeholder.GetComponent<TextMeshProUGUI>().text = "Set Price";
            price.text = "";
        }
        else
        {
            if (price.text.Length > 0 && price.text[0] != '£')
            {
                price.text = "£" + price.text;
            }
        }
    }

    public void SetValues(int newIndex)
    {
        index = newIndex;
        if(currentPreview != null)
        {
            Destroy(currentPreview.gameObject);
        }
        if (CustomerManager.Instance.shrimpSaleSlots[index] != null && CustomerManager.Instance.shrimpSaleSlots[index].shrimp != null)
        {
            _shrimp = CustomerManager.Instance.shrimpSaleSlots[index].shrimp;
            currentPreview = Instantiate(shrimpPreview, transform.GetChild(0)).GetComponent<ShrimpSelectionBlock>();
            currentPreview.Populate(_shrimp.stats);
            price.interactable = true;
            price.placeholder.GetComponent<TextMeshProUGUI>().text = "Set Price";
            if (CustomerManager.Instance.shrimpSaleSlots[index].value != 0) price.text = CustomerManager.Instance.shrimpSaleSlots[index].value.ToString();
            else price.text = "";
        }
        else
        {
            price.interactable = false;
            price.placeholder.GetComponent<TextMeshProUGUI>().text = "";
        }
    }

    public void SetShrimp()
    {
        currentSelectionScreen = Instantiate(shrimpSelectionScreen, transform.parent.parent.parent.parent).GetComponent<ShrimpSelectionWindow>().PopulateForShrimpSale(this).gameObject;
    }

    public void CloseSelection()
    {
        SetValues(index);
        Destroy(currentSelectionScreen);
    }

    public void SetPrice(string value)
    {
        if (value.Length > 0 && value[0] == '£')
        {
            value = value.Substring(1);
        }
        float price = float.Parse(value);
        CustomerManager.Instance.shrimpSaleSlots[index].value = price;
        CustomerManager.Instance.shrimpSaleSlots[index].shrimp.stats.assignedValue = price;
    }

    public void UISelect()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().SwitchCurrentActionMap("Empty");
    }

    public void UIUnselect()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().SwitchCurrentActionMap("TankView");
    }
}
