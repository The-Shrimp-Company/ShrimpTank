using SaveLoadSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : Interactable
{
    public static Radio MasterRadio;

    private bool active = false;

    [SerializeField] private DiageticScreen screen;

    private bool play = true;

    public List<AudioClip> clips;

    private Queue<AudioClip> playList = new Queue<AudioClip>();

    private int playlistLength;

    private AudioSource source;

    private bool pausedDueToApplicationFocus;


    private void Awake()
    {
        if (MasterRadio == null) 
        {
            SetupRadio();
        }
        else if (MasterRadio != this) 
        { 
            active = false; 
        }
    }

    private void SetupRadio()
    {
        MasterRadio = this;
        active = true;

        source = GetComponent<AudioSource>();
        foreach (AudioClip clip in clips)
        {
            playList.Enqueue(clip);
            playlistLength++;
        }


        // Loading Radio State
        if (!SaveManager.startNewGame)
        {
            int skipTo = PlayerStats.stats.currentSongPlaying + 1;
            PlayerStats.stats.currentSongPlaying = -1;
            for (int i = 0; i < skipTo; i++)
                StartNextSong();

            if (PlayerStats.stats.radioPlaying) Play();
            else Pause();
        }
        else  // If a new game is being started
        {
            Play();
            PlayerStats.stats.currentSongPlaying = -1;
        }
    }


    void Update()
    {
        if (active)
        {
            MouseHover();
            if (!source.isPlaying && play)  // If it should be playing but is not
            {
                MasterRadio.StartNextSong();
            }
        }
        else if(MasterRadio == null)
        {
            SetupRadio();
        }
    }

    
    public override void Action()
    {
        if (MasterRadio.source.isPlaying)
        {
            MasterRadio.Pause();
        }
        else
        {
            MasterRadio.Play();
        }

        PlayerStats.stats.timesRadioToggled++;
    }


    public override void OnHover()
    {
        //screen.gameObject.SetActive(true);
    }


    public void Play()
    {
        if (source)
        {
            source.UnPause();
            play = true;
            PlayerStats.stats.radioPlaying = true;
        }
    }


    public void Pause()
    {
        source.Pause();
        play = false;
        PlayerStats.stats.radioPlaying = false;
    }


    public void StartNextSong()
    {
        AudioClip toPlay = playList.Dequeue();
        playList.Enqueue(toPlay);
        source.clip = toPlay;
        source.Play();

        PlayerStats.stats.currentSongPlaying++;
        if (PlayerStats.stats.currentSongPlaying >= playlistLength)
            PlayerStats.stats.currentSongPlaying = 0;
    }


    // Prevents an issue where the game would skip a song anytime the application loses focus
    private void OnApplicationFocus(bool focus)
    {
        if (!active) return;
        if (focus && pausedDueToApplicationFocus)
        {
            Play();
            pausedDueToApplicationFocus = false;
        }
        else if (!focus && play)
        {
            Pause();
            pausedDueToApplicationFocus = true;
        }
    }

    private void OnDestroy()
    {
        if (active)
        {
            MasterRadio = null;
        }
    }
}
