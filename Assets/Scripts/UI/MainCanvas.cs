using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private GameObject lastCreated;

    [SerializeField]
    private GameObject tablet;
    private RectTransform _tabletRect;
    [SerializeField]
    private RectTransform _tabletRestingCoord;
    [SerializeField]
    private RectTransform _tabletActiveCoord;

    [SerializeField] private Animator tabletAnim;

    // Start is called before the first frame update
    void Start()
    {
        UIManager.instance.SetCanvas(transform);
        _tabletRect = tablet.GetComponent<RectTransform>();
    }

    public GameObject RaiseScreen(GameObject screen)
    {
        RaiseTablet();
        lastCreated = Instantiate(screen, transform);
        UIManager.instance.OpenScreen(lastCreated.GetComponent<ScreenView>());
        UIManager.instance.SetCursorMasking(true);
        return lastCreated;
    }

    public void LowerScreen()
    {
        LowerTablet();
        Destroy(lastCreated);
        UIManager.instance.SetCursorMasking(false);
        UIManager.instance.CloseScreen();
    }

    public void RaiseTablet()
    {
        tabletAnim.SetTrigger("Raise");
    }

    public void LowerTablet()
    {
        tabletAnim.SetTrigger("Lower");
    }
}
