using SaveLoadSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SettingsScreen : ScreenView
{
    public Slider slider;
    public Slider volume;

    protected override void Start()
    {
        base.Start();

        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            slider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
        else
        {
            slider.value = 0.5f;
        }

        if (PlayerPrefs.HasKey("Volume"))
        {
            AudioController.VolumeChange(PlayerPrefs.GetFloat("Volume"));
            volume.value = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            AudioController.VolumeChange(0f);
            volume.value = 0f;
            PlayerPrefs.SetFloat("Volume", 0f);
        }
    }

    public void ValueChange()
    {
        PlayerPrefs.SetFloat("Sensitivity", slider.value);
        player.GetComponent<CameraControls>().LoadSensitivity();
    }

    public void VolumeChange(Slider value)
    {
        AudioController.VolumeChange(value.value);

        PlayerPrefs.SetFloat("Volume", value.value);
    }

    public void FOVChange(Slider value)
    {
        PlayerPrefs.SetFloat("FOV", value.value);
        Camera.main.fieldOfView = value.value;
    }
}
