using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(GetComponentInParent<ScreenView>().Exit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
