using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataStore : MonoBehaviour
{
    public static string StoreName;
    public static bool Assigned = false;


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLoad;
        Assigned = true;
    }

    public void OnLoad(Scene scene, LoadSceneMode mode)
    {
        if(scene.name != "ShopScene")
        {
            Assigned = false;
            SceneManager.sceneLoaded -= OnLoad;
            Destroy(gameObject);
        }
        else
        {
            Store.StoreName = StoreName;
        }
    }
}
