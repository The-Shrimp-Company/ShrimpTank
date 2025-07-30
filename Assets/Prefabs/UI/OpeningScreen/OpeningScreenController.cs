using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class OpeningScreenController : MonoBehaviour
{
    private AsyncOperation operation;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        StartCoroutine(LoadScene());

        root.Q<Button>("Confirm").clicked += () =>
        {
            operation.allowSceneActivation = true;
            DataStore.StoreName = root.Q<TextField>("TextField").text;
        };
    }

    IEnumerator LoadScene()
    {
        yield return null;
        operation = SceneManager.LoadSceneAsync("OldShopScene");
        operation.allowSceneActivation = false;
        yield return null;
    }
}
