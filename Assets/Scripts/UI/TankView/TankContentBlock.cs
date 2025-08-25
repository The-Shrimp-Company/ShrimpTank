using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankContentBlock : ContentBlock
{
    private Shrimp _shrimp;
    private TankViewScript _tankView;
    [SerializeField]
    private GameObject shrimpView;

    public Button main, checkbutton;

    [Header("Status Bar")]
    public Image tempArrow;
    public Image HNOArrow;
    public Image pHArrow;
    public Image saltArrow;
    public Sprite arrow;
    public Sprite check;

    [Header("Shrimp Colours")]
    [SerializeField] private Image primaryColour;
    [SerializeField] private Image secondaryColour;

    public void SetShrimp(Shrimp shrimp, TankViewScript tankView)
    {
        _shrimp = shrimp;
        _tankView = tankView;
        primaryColour.color = GeneManager.instance.GetTraitSO(_shrimp.stats.primaryColour.activeGene.ID).colour;
        secondaryColour.color = GeneManager.instance.GetTraitSO(_shrimp.stats.secondaryColour.activeGene.ID).colour;
    }

    private void Update()
    {
        UpdateArrow(_shrimp.tank.waterTemperature, _shrimp.stats.temperaturePreference, tempArrow);
        UpdateArrow(_shrimp.tank.waterSalt, _shrimp.stats.salineLevel, saltArrow);
        UpdateArrow(_shrimp.tank.waterPh, _shrimp.stats.PhPreference, pHArrow);
        UpdateArrow(_shrimp.tank.waterAmmonium, _shrimp.stats.ammoniaPreference, HNOArrow);
        
    }

    private void UpdateArrow(float currentStat, float idealStat, Image statArrow)
    {
        if(Mathf.Abs(currentStat - idealStat) > 10)
        {
            statArrow.sprite = arrow;
            statArrow.transform.localScale = new Vector3(1, -Mathf.Sign(currentStat - idealStat), 1);
        }
        else
        {
            statArrow.sprite = check;
            statArrow.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void Pressed()
    {
        GameObject newitem = Instantiate(shrimpView);
        UIManager.instance.OpenScreen(newitem.GetComponent<ScreenView>());
        newitem.GetComponent<ShrimpView>().Populate(_shrimp);
        _shrimp.GetComponentInChildren<ShrimpCam>().SetCam();
        newitem.GetComponent<Canvas>().worldCamera = UIManager.instance.GetCamera();
        newitem.GetComponent<Canvas>().planeDistance = 1;
        UIManager.instance.SetCursorMasking(false);
    }
}
