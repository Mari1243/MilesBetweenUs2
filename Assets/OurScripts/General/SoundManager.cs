using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource effectAudioSource;
    public AudioSource musicAudioSource;
    public float GeneralVolume;
    public bool playBackgroundMusic;
    public AudioClip backgroundMusic;
    private bool isPaused;

    private AudioClip lastPlayedMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object when loading new scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
        
    }

    public void PlayAudio(AudioClip clip)
    {
        if(effectAudioSource!=null && isPaused)
        {
            effectAudioSource.UnPause();
        }
        else
        {
            effectAudioSource.PlayOneShot(clip, GeneralVolume);

        }
    }

    public void PauseAudio()
    {
        if(effectAudioSource != null)
        {
            effectAudioSource.Pause();
            isPaused = true;
        }
    }
    //this will play a random noise out of a list, .6f is a good volume generally
    public void playRandomOneshot(AudioClip[] audioClips, float volumn)
    {
        int randomIndex = UnityEngine.Random.Range(0, audioClips.Length);
        print("playing: " + audioClips[randomIndex].name);
        effectAudioSource.PlayOneShot(audioClips[randomIndex], volumn);
    }

    public void playmusic()
    {
        Console.WriteLine("playmusic called from "+Environment.StackTrace);
        //print("playing music");
    
        musicAudioSource.loop = true;
        musicAudioSource.clip=lastPlayedMusic;
        musicAudioSource.Play();
    }
    public void changeMusic(AudioClip newClip)
    {
        Console.WriteLine("changeMusic called from "+Environment.StackTrace);
        
        //print("playing music");
        if (playBackgroundMusic)    
        {
            musicAudioSource.loop = true;

            musicAudioSource.clip = newClip;
            musicAudioSource.Play();

            lastPlayedMusic = newClip;
        }
    }
    public void playLoopingAudio(AudioClip clip)
    {
        print("playing looping audio " + Environment.StackTrace);
        musicAudioSource.clip = clip;
        musicAudioSource.loop = true; // Set looping to true
        musicAudioSource.Play();
    }


    public void loopAudioClip()
    {
        effectAudioSource.loop = true;
    }
    
    public void lowerMusic()
    {
        musicAudioSource.volume = .3f;
    }

}
