using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    static public AudioController instance;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (PlayerPrefs.HasKey("Volume"))
        {
            VolumeChange(PlayerPrefs.GetFloat("Volume"));
        }
        else
        {
            VolumeChange(-20f);
            PlayerPrefs.SetFloat("Volume", -20f);
        }
    }

    static public void VolumeChange(float changeTo)
    {
        instance.mixer.SetFloat("Volume", changeTo);
    }
}
