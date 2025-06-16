using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class ButtonSounds : MonoBehaviour
{

    private void Start()
    {

        GetComponent<Button>().onClick.AddListener(() =>
        {
            AudioManager.instance.GetButtonClick();
        });
    }

}
