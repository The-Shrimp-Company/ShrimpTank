using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MyDropdown : MonoBehaviour
{
    [Serializable]
    /// <summary>
    /// Class to store the text and/or image of a single option in the dropdown list.
    /// </summary>
    public class OptionData
    {
        public delegate void Func(bool nowSet);

        private Func myFunc;

        [SerializeField]
        private string m_Text;

        private bool m_set = true;

        /// <summary>
        /// The text associated with the option.
        /// </summary>
        public string text { get { return m_Text; } set { m_Text = value; } }

        public Func func { get { return myFunc; } set { myFunc = value; } }

        public bool set { get { return m_set; } set { m_set = value; } }

        public OptionData() { }

        public OptionData(string text)
        {
            this.text = text;
        }


        public OptionData(Func func)
        {
            this.func = func;
        }

    }


    [SerializeField] private GameObject dropdownMenu;
    [SerializeField] private GameObject dropdownItem;

    [SerializeField] private Sprite check;
    [SerializeField] private Sprite uncheck;

    public List<OptionData> options;

    private GameObject myDropdown;
    private GameObject blocker;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Show);
    }

    private void Show()
    {
        Canvas rootCanvas = GetComponentInParent<Canvas>();

        if (myDropdown == null)
        {
            myDropdown = Instantiate(dropdownMenu, transform);
            Debug.Log(myDropdown);
        }

        if(blocker != null) Destroy(blocker);

        blocker = CreateBlocker(rootCanvas);

        PopulateDropdown();
    }

    private void Hide()
    {
        Destroy(myDropdown);
        Destroy(blocker);
    }

    /// <summary>
    /// Create a blocker that blocks clicks to other controls while the dropdown list is open.
    /// </summary>
    /// <remarks>
    /// Override this method to implement a different way to obtain a blocker GameObject.
    /// </remarks>
    /// <param name="rootCanvas">The root canvas the dropdown is under.</param>
    /// <returns>The created blocker object</returns>
    protected virtual GameObject CreateBlocker(Canvas rootCanvas)
    {
        // Create blocker GameObject.
        GameObject blocker = new GameObject("Blocker");

        // Setup blocker RectTransform to cover entire root canvas area.
        RectTransform blockerRect = blocker.AddComponent<RectTransform>();
        blockerRect.SetParent(rootCanvas.transform, false);
        blockerRect.anchorMin = Vector3.zero;
        blockerRect.anchorMax = Vector3.one;
        blockerRect.sizeDelta = Vector2.zero;

        // Make blocker be in separate canvas in same layer as dropdown and in layer just below it.
        Canvas blockerCanvas = blocker.AddComponent<Canvas>();
        blockerCanvas.overrideSorting = true;
        Canvas dropdownCanvas = myDropdown.GetComponent<Canvas>();
        blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
        blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;

        // Find the Canvas that this dropdown is a part of
        Canvas parentCanvas = null;
        Transform parentTransform = transform;
        while (parentTransform != null)
        {
            parentCanvas = parentTransform.GetComponent<Canvas>();
            if (parentCanvas != null)
                break;

            parentTransform = parentTransform.parent;
        }

        // If we have a parent canvas, apply the same raycasters as the parent for consistency.
        if (parentCanvas != null)
        {
            Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
            for (int i = 0; i < components.Length; i++)
            {
                Type raycasterType = components[i].GetType();
                if (blocker.GetComponent(raycasterType) == null)
                {
                    blocker.AddComponent(raycasterType);
                }
            }
        }
        else
        {
            // Add raycaster since it's needed to block.
            GetOrAddComponent<GraphicRaycaster>(blocker);
        }


        // Add image since it's needed to block, but make it clear.
        Image blockerImage = blocker.AddComponent<Image>();
        blockerImage.color = Color.clear;

        // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
        Button blockerButton = blocker.AddComponent<Button>();
        blockerButton.onClick.AddListener(Hide);

        return blocker;
    }

    private static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (!comp)
            comp = go.AddComponent<T>();
        return comp;
    }

    private void PopulateDropdown()
    {
        Transform content = GetComponentInChildren<VerticalLayoutGroup>().transform;

        for (int i = 0; i < options.Count; i++)
        {
            GameObject item = Instantiate(dropdownItem, content);
            Image itemImage = item.transform.GetChild(0).GetComponent<Image>();

            if (options[i].text != "") item.GetComponentInChildren<TextMeshProUGUI>().text = options[i].text;


            if (options[i].set) itemImage.sprite = check;
            else itemImage.sprite = uncheck;

            int tI = i;
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (options[tI].set)
                {
                    itemImage.sprite = uncheck;
                    options[tI].func(false);
                    options[tI].set = false;
                }
                else
                {
                    itemImage.sprite = check;
                    options[tI].func(true);
                    options[tI].set = true;
                }

            });
        }
    }
}
