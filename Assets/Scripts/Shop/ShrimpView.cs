using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShrimpView : ScreenView
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_InputField title;
    [SerializeField] private GameObject tankView;
    [SerializeField] private TextMeshProUGUI age, gender, body, pattern, legs, tail, tailFan, eyes, head;
    [SerializeField] private RenderTexture texture;
    protected Shrimp _shrimp;
    [SerializeField] private GameObject currentTankScreen, medScreen;
    [SerializeField] private Image primaryColour, secondaryColour;
    [SerializeField] private Slider hunger;
    public Vector3 panelRestPos;

    [Header("Status Bar")]
    [SerializeField] private TextMeshProUGUI idealTempLabel;
    [SerializeField] private TextMeshProUGUI tempMarginLabel;
    [SerializeField] private TextMeshProUGUI idealSaltLabel;
    [SerializeField] private TextMeshProUGUI saltMarginLabel;
    [SerializeField] private TextMeshProUGUI idealPhLabel;
    [SerializeField] private TextMeshProUGUI phMarginLabel;
    [SerializeField] private TextMeshProUGUI idealHnoLabel;
    [SerializeField] private TextMeshProUGUI hnoMarginLabel;


    [Header("Menu Open/Close")]
    public float openAnimationSpeed = 0.3f;
    [SerializeField] private Vector3 panelClosePos;

    [Header("Menu Switch")]
    public float switchAnimationSpeed = 0.5f;
    public Ease switchAnimationEase;
    [SerializeField] private Vector3 panelSwitchInPos;
    [SerializeField] private Vector3 panelSwitchOutPos;

    [Header("Shrimp Switch")]
    [SerializeField] private Vector2 shrimpSwitchPunch;

    public override void Open(bool switchTab)
    {
        StartCoroutine(OpenTab(switchTab));
        base.Open(switchTab);
    }


    public void Update()
    {
        if (_shrimp != null)
        {
            //hunger.value = _shrimp.stats.hunger;
            tempMarginLabel.text = Mathf.Round(_shrimp.tank.waterTemperature - _shrimp.stats.temperaturePreference).ToString();
            saltMarginLabel.text = Mathf.Round(_shrimp.tank.waterSalt - _shrimp.stats.salineLevel).ToString();
            phMarginLabel.text = Mathf.Round(_shrimp.tank.waterPh - _shrimp.stats.PhPreference).ToString();
            hnoMarginLabel.text = Mathf.Round(_shrimp.tank.waterAmmonium - _shrimp.stats.ammoniaPreference).ToString();
        }
    }

    public void Click()
    {
        CurrentTankScreen screen = Instantiate(currentTankScreen, UIManager.instance.GetCanvas()).GetComponent<CurrentTankScreen>();
        UIManager.instance.OpenScreen(screen);
        screen.SetShrimp(_shrimp);
    }

    public void MedScreen()
    {
        GameObject screen = Instantiate(medScreen, UIManager.instance.GetCanvas());
        UIManager.instance.OpenScreen(screen.GetComponent<ScreenView>());
        
        screen.GetComponentInChildren<InventoryContent>().MedAssignment(this, _shrimp, screen);
    }

    public void MouseClick(Vector2 point)
    {
        RaycastHit ray;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(point), out ray, 3f, LayerMask.GetMask("Shrimp")))
        {
            if (_shrimp.tank.shrimpInTank.Contains(ray.transform.GetComponent<Shrimp>()))
            {
                _shrimp.gameObject.layer = LayerMask.NameToLayer("Shrimp");
                _shrimp.GetComponentInChildren<ShrimpCam>().Deactivate();
                player.GetComponent<PlayerUIController>().UnsetShrimpCam();
                _shrimp = ray.transform.GetComponent<Shrimp>();
                Populate(_shrimp);
                GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
                //GetComponent<Canvas>().planeDistance = 1;
                UIManager.instance.SetCursorMasking(false);
                _shrimp.GetComponentInChildren<ShrimpCam>().SetCam();

                DOTween.Kill(panel);
                panel.transform.localPosition = panelRestPos;
                panel.DOPunchAnchorPos(shrimpSwitchPunch, 0.25f);
            }
        }
    }

    

    /// <summary>
    /// Fills the shrimp view with the information about shrimp
    /// </summary>
    /// <param name="Shrimp"></param>
    public void Populate(Shrimp Shrimp)
    {
        player = GameObject.Find("Player");
        _shrimp = Shrimp;
        title.text = _shrimp.stats.name;
        //title.placeholder.GetComponent<TextMeshProUGUI>().text = _shrimp.stats.name;
        age.text = "Age: " + (ShrimpManager.instance.IsShrimpAdult(_shrimp.stats) ? "Adult" : "Child");
        gender.text = "Sex: " + (_shrimp.stats.sex == true ? "Male" : "Female");
        pattern.text = "Pattern: " + GeneManager.instance.GetTraitSO(_shrimp.stats.pattern.activeGene.ID).traitName;
        body.text = "Body: " + GeneManager.instance.GetTraitSO(_shrimp.stats.body.activeGene.ID).set;
        legs.text = "Legs: " + GeneManager.instance.GetTraitSO(_shrimp.stats.legs.activeGene.ID).set;
        eyes.text = "Eyes: " + GeneManager.instance.GetTraitSO(_shrimp.stats.eyes.activeGene.ID).set;
        tail.text = "Tail: " + GeneManager.instance.GetTraitSO(_shrimp.stats.tail.activeGene.ID).set;
        head.text = "Head: " + GeneManager.instance.GetTraitSO(_shrimp.stats.head.activeGene.ID).set;
        tailFan.text = "Tail Fan: " + GeneManager.instance.GetTraitSO(_shrimp.stats.tailFan.activeGene.ID).set;
        primaryColour.color = GeneManager.instance.GetTraitSO(_shrimp.stats.primaryColour.activeGene.ID).colour;
        secondaryColour.color = GeneManager.instance.GetTraitSO(_shrimp.stats.secondaryColour.activeGene.ID).colour;
        //hunger.value = _shrimp.stats.hunger;
        idealTempLabel.text = _shrimp.stats.temperaturePreference.ToString();
        idealSaltLabel.text = _shrimp.stats.salineLevel.ToString();
        idealHnoLabel.text = _shrimp.stats.ammoniaPreference.ToString();
        idealPhLabel.text = _shrimp.stats.PhPreference.ToString();
        _shrimp.FocusShrimp();
        _shrimp.gameObject.layer = LayerMask.NameToLayer("SelectedShrimp");
        player.GetComponent<PlayerUIController>().SetShrimpCam(_shrimp.GetComponentInChildren<ShrimpCam>());
    }


    public override void Exit()
    {
        _shrimp.gameObject.layer = LayerMask.NameToLayer("Shrimp");
        _shrimp.GetComponentInChildren<ShrimpCam>().Deactivate();
        player.GetComponent<PlayerUIController>().UnsetShrimpCam();
        TankController tank = _shrimp.tank.GetComponent<TankController>();
        Camera.main.transform.SetPositionAndRotation(tank.GetCam().transform.position, tank.GetCam().transform.rotation);
        UIManager.instance.CloseScreen();
    }

    public void SetName(TextMeshProUGUI input)
    {
        _shrimp.name = input.text;
        _shrimp.stats.name = input.text;
        title.text = input.text;
        _shrimp.shrimpNameChanged = true;
    }

    public void UISelect()
    {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Empty");
    }

    public void UIUnselect()
    {
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("TankView");
    }



    public override void Close(bool switchTab)
    {
        _shrimp.gameObject.layer = LayerMask.NameToLayer("Shrimp");
        _shrimp.GetComponentInChildren<ShrimpCam>().Deactivate();
        _shrimp.StopFocusingShrimp();
        player.GetComponent<PlayerUIController>().UnsetShrimpCam();
        StartCoroutine(CloseTab(switchTab));
    }


    public IEnumerator OpenTab(bool switchTab)
    {
        UIManager.instance.SetCursorMasking(true);  // Enable cursor masking
        panel.transform.localPosition = switchTab ? panelSwitchInPos : panelClosePos;  // Move the panel offscreen


        if (switchTab)  // If we are switching from another menu
        {
            yield return new WaitForSeconds(switchAnimationSpeed / 1.2f);  // Wait for the other one to close partially
            panel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, switchAnimationSpeed).SetEase(switchAnimationEase, 1.4f);  // Move this panel onto the screen
            yield return new WaitForSeconds(switchAnimationSpeed);
        }
        else  // If we don't have a menu open already
        {
            GetComponent<CanvasGroup>().alpha = 0f;  // Make the menu transparent
            GetComponent<CanvasGroup>().DOFade(1, openAnimationSpeed).SetEase(Ease.OutCubic);  // Fade the menu in

            panel.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, openAnimationSpeed).SetEase(Ease.OutBack);  // Move this panel onto the screen

            yield return new WaitForSeconds(openAnimationSpeed);
        }
    }



    public IEnumerator CloseTab(bool switchTab)
    {
        UIManager.instance.SetCursorMasking(true);  // Enable cursor masking
        DOTween.Kill(panel);  // End currently running tweens

        if (switchTab)  // If we are switching to another menu
        {
            panel.GetComponent<RectTransform>().DOAnchorPos(panelSwitchOutPos, switchAnimationSpeed).SetEase(switchAnimationEase, 1.4f);  // Move this panel off the screen
            yield return new WaitForSeconds(0.5f);
        }
        else  // If we are fully closing the menu
        {
            panel.GetComponent<RectTransform>().DOAnchorPos(panelClosePos, openAnimationSpeed).SetEase(Ease.InBack);  // Move this panel off the screen
            GetComponent<CanvasGroup>().DOFade(0, openAnimationSpeed).SetEase(Ease.InCubic);  // Fade the panel out

            yield return new WaitForSeconds(openAnimationSpeed);
        }

        DOTween.Kill(panel);  // End the tweens
        base.Close(switchTab);
    }

    public Shrimp GetShrimp() { return _shrimp; }
}
