using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource effectAudioSource;
    public AudioSource musicAudioSource;
    public float GeneralVolume;
    public bool playBackgroundMusic;
    public AudioClip backgroundMusic;

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
        
        playmusic();
    }

    public void PlayAudio(AudioClip clip)
    {
        effectAudioSource.PlayOneShot(clip, GeneralVolume);
    }

    //this will play a random noise out of a list, .6f is a good volume generally
    public void playRandomOneshot(AudioClip[] audioClips, float volumn)
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        print("playing: " + audioClips[randomIndex].name);
        effectAudioSource.PlayOneShot(audioClips[randomIndex], volumn);
    }

    private void playmusic()
    {

        if (playBackgroundMusic)
        {
            musicAudioSource.loop = true;
            musicAudioSource.Play();
        }
    }


}
