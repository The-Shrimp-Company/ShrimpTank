using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class OpeningScreenController : MonoBehaviour
{
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("Confirm").clicked += () =>
        {
            Debug.Log(root.Q<TextField>("TextField").text);
        };
    }
}
