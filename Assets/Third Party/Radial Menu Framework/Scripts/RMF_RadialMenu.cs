using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;


[AddComponentMenu("Radial Menu Framework/RMF Core Script")]
public class RMF_RadialMenu : MonoBehaviour {

    [HideInInspector]
    public RectTransform rt;

    public GameObject player;
    //public RectTransform baseCircleRT;
    //public Image selectionFollowerImage;

    [Tooltip("Adjusts the radial menu for use with a gamepad or joystick. You might need to edit this script if you're not using the default horizontal and vertical input axes.")]
    public bool useGamepad = false;

    [Tooltip("With lazy selection, you only have to point your mouse (or joystick) in the direction of an element to select it, rather than be moused over the element entirely.")]
    public bool useLazySelection = true;


    [Tooltip("If set to true, a pointer with a graphic of your choosing will aim in the direction of your mouse. You will need to specify the container for the selection follower.")]
    public bool useSelectionFollower = true;

    [Tooltip("If using the selection follower, this must point to the rect transform of the selection follower's container.")]
    public RectTransform selectionFollowerContainer;

    [Tooltip("This is the text object that will display the labels of the radial elements when they are being hovered over. If you don't want a label, leave this blank.")]
    public TMP_Text nameLabel;
    public TMP_Text textLabel;
    public RectTransform background;

    [Tooltip("This is the list of radial menu elements. This is order-dependent. The first element in the list will be the first element created, and so on.")]
    public List<RMF_RadialMenuElement> elements = new List<RMF_RadialMenuElement>();

    public List<Sprite> elementSprites = new List<Sprite>();
    public List<float> elementHeights = new List<float>();

    public float animationLength = 0.75f;



    [Tooltip("Controls the total angle offset for all elements. For example, if set to 45, all elements will be shifted +45 degrees. Good values are generally 45, 90, or 180")]
    public float globalOffset = 0f;


    [HideInInspector]
    public float currentAngle = 0f; //Our current angle from the center of the radial menu.

    [HideInInspector] public bool menuOpen;
    [HideInInspector] public bool justOpened;
    [HideInInspector] public int currentElementCount;

    [HideInInspector]
    public int index = 0; //The current index of the element we're pointing at.

    private float angleOffset; //The base offset. For example, if there are 4 elements, then our offset is 360/4 = 90

    private int previousActiveIndex = 0; //Used to determine which buttons to unhighlight in lazy selection.

    private PointerEventData pointer;

    private CanvasGroup canvasGroup;

    void Awake() {

        pointer = new PointerEventData(EventSystem.current);

        rt = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;

        if (rt == null)
            Debug.LogError("Radial Menu: Rect Transform for radial menu " + gameObject.name + " could not be found. Please ensure this is an object parented to a canvas.");

        if (useSelectionFollower && selectionFollowerContainer == null)
            Debug.LogError("Radial Menu: Selection follower container is unassigned on " + gameObject.name + ", which has the selection follower enabled.");


        SetupElements(elements.Count);
    }


    private void SetupElements(int elementCount)
    {
        currentElementCount = elementCount;
        angleOffset = (360f / (float)elementCount);

        foreach(Transform child in elements[0].transform.parent)
            child.gameObject.SetActive(false);

        //Loop through and set up the elements.
        for (int i = 0; i < elementCount; i++)
        {
            if (elements[i] == null)
            {
                Debug.LogError("Radial Menu: element " + i.ToString() + " in the radial menu " + gameObject.name + " is null!");
                continue;
            }

            elements[i].gameObject.SetActive(true);

            elements[i].parentRM = this;

            elements[i].setAllAngles((angleOffset * i) + globalOffset, angleOffset);

            elements[i].assignedIndex = i;

            RectTransform rt = elements[i].GetComponent<RectTransform>();
            rt.rotation = Quaternion.Euler(0, 0, -((360f / (float)elementCount) * i) - globalOffset);

            rt = elements[i].transform.GetChild(0).GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.localPosition.x, elementHeights[elementCount - 1] / 3, rt.localPosition.z);
            rt.DOLocalMoveY(elementHeights[elementCount - 1], animationLength).SetEase(Ease.OutBack).SetDelay((float)(i + 1) / 10);

            Vector3 scale = rt.localScale;
            rt.localScale = Vector3.zero;
            rt.DOScale(scale, animationLength).SetEase(Ease.OutBack).SetDelay((float)(i + 1) / 10);

            elements[i].transform.GetComponentInChildren<Image>().sprite = elementSprites[elementCount - 1];
        }
    }

    public void DisplayMenu(Dictionary<string, UnityAction> actions, string name)
    {
        SetupElements(actions.Count);
        menuOpen = true;
        justOpened = true;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        nameLabel.text = name;

        Vector3 scale = background.localScale;
        background.localScale = Vector3.zero;
        background.DOScale(scale, animationLength).SetEase(Ease.OutQuad);


        int i = 0;
        foreach (var action in actions)
        {
            elements[i].name = action.Key;
            elements[i].gameObject.GetComponent<RMF_RadialMenuElement>().label = action.Key;
            elements[i].gameObject.GetComponentInChildren<TMP_Text>().text = action.Key;
            elements[i].gameObject.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            elements[i].gameObject.GetComponentInChildren<Button>().onClick.AddListener(HideMenu);
            elements[i].gameObject.GetComponentInChildren<Button>().onClick.AddListener(action.Value);

            i++;
        }

        selectButton(0);
    }

    public void HideMenu()
    {
        if (!menuOpen) return;

        menuOpen = false;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Start() 
    {
        if (useGamepad) {
            EventSystem.current.SetSelectedGameObject(gameObject, null); //We'll make this the active object when we start it. Comment this line to set it manually from another script.
            if (useSelectionFollower && selectionFollowerContainer != null)
                selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, -globalOffset); //Point the selection follower at the first element.
        }

    }

    // Update is called once per frame
    void Update() {

        //If your gamepad uses different horizontal and vertical joystick inputs, change them here!
        //==============================================================================================
        bool joystickMoved = Input.GetAxis("Horizontal") != 0.0 || Input.GetAxis("Vertical") != 0.0;
        //==============================================================================================


        float rawAngle;
        
        if (!useGamepad)
            rawAngle = Mathf.Atan2(Input.mousePosition.y - rt.position.y, Input.mousePosition.x - rt.position.x) * Mathf.Rad2Deg;
        else
            rawAngle = Mathf.Atan2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * Mathf.Rad2Deg;

        //If no gamepad, update the angle always. Otherwise, only update it if we've moved the joystick.
        if (!useGamepad)
            currentAngle = normalizeAngle(-rawAngle + 90 - globalOffset + (angleOffset / 2f));
        else if (joystickMoved)
            currentAngle = normalizeAngle(-rawAngle + 90 - globalOffset + (angleOffset / 2f));

        //Handles lazy selection. Checks the current angle, matches it to the index of an element, and then highlights that element.
        if (angleOffset != 0 && useLazySelection) {

            //Current element index we're pointing at.
            index = (int)(currentAngle / angleOffset);

            if (elements[index] != null) {

                //Select it.
                selectButton(index);

                //If we click or press a "submit" button (Button on joystick, enter, or spacebar), then we'll execut the OnClick() function for the button.
                if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Submit")) {

                    ExecuteEvents.Execute(elements[index].button.gameObject, pointer, ExecuteEvents.submitHandler);


                }
            }

        }

        //Updates the selection follower if we're using one.
        if (useSelectionFollower && selectionFollowerContainer != null) {
            if (!useGamepad || joystickMoved)
                selectionFollowerContainer.rotation = Quaternion.Euler(0, 0, rawAngle + 270);
           

        }

        justOpened = false;
    }


    //Selects the button with the specified index.
    private void selectButton(int i) {

          if (elements[i].active == false) {

            elements[i].highlightThisElement(pointer); //Select this one

            if (previousActiveIndex != i) 
                elements[previousActiveIndex].unHighlightThisElement(pointer); //Deselect the last one.
            

        }

        previousActiveIndex = i;

    }

    //Keeps angles between 0 and 360.
    private float normalizeAngle(float angle) {

        angle = angle % 360f;

        if (angle < 0)
            angle += 360;

        return angle;

    }


}
