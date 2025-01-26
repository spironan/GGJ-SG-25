using System.Collections.Generic;
using UnityEngine;

public class MasterAudioController : MonoBehaviour
{
    public static MasterAudioController instance { get; private set; }

    [SerializeField]
    private GameObject sfxSource, bgmSource;
    
    [SerializeField]    
    private int currentLayer = 0;

    private AudioSource[] audioSources;
    private AudioSource[] sfxControllers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sfxControllers = sfxSource.GetComponentsInChildren<AudioSource>();

        audioSources = bgmSource.GetComponentsInChildren<AudioSource>();
        //foreach (AudioSource audioSource in audioSources)
        //{
        //    Debug.Log(audioSource.gameObject.name);
        //}
    }

    public void OnGameStart()
    {
        PlayAllBGM();
    }

    public void OnGameLost()
    {
        StopAllBGM();
    }

    public void PlaySFX(AudioClip clip)
    {
        // run some logic to play desired clip
        AudioSource source = null;
        foreach (AudioSource sfxSource in sfxControllers)
        {
            if (!sfxSource.isPlaying)
            {
                source = sfxSource;
                break;
            }
        }

        if (source != null)
        {
            source.clip = clip;
            source.Play();
        } else
        {
            Debug.Log("All sfx sources are busy! skip playing this incoming sfx request");
        }
    }

    public void PlayBGM()
    {

    }

    private void PlayAllBGM()
    {
        foreach (AudioSource audioSource in GetComponentsInChildren<AudioSource>())
        {
            if (audioSource.gameObject == sfxSource)
            {
                continue;
            }

            audioSource.Play();
        }
    }

    private void StopAllBGM()
    {
        foreach (AudioSource audioSource in GetComponentsInChildren<AudioSource>())
        {
            if (audioSource.gameObject == sfxSource)
            {
                continue;
            }

            audioSource.Stop();
        }
    }

    public void UnmuteNextDynamicBGMLayer()
    {
        if (currentLayer < audioSources.Length)
            audioSources[currentLayer++].mute = false;
    }

}
