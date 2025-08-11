using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiageticScreen : Interactable
{
    private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        MouseHover();
        canvas.transform.LookAt(Camera.main.transform);
    }

    public override void OnStopHover()
    {
        gameObject.SetActive(false);
    }

    public override void OnHover()
    {
        //Debug.Log("This one works");
    }
}
