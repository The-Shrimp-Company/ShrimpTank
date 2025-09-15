using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScreenshotter : MonoBehaviour
{
    void Start()
    {
        ScreenCapture.CaptureScreenshot(SaveManager.GetScreenshotFilepath(), 3);
        Debug.Log("Screenshot saved to " + SaveManager.GetScreenshotFilepath());
    }
}
