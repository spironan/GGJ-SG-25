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
        audioSources = bgmSource.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            Debug.Log(audioSource.gameObject.name);
        }
    }

    void PrepareTracks(uint layers = 5)
    {
        
    }

    public void PlaySFX()
    {

    }

    public void PlayBGM()
    {

    }

    public void UnmuteNextDynamicBGMLayer()
    {
        if (currentLayer < audioSources.Length)
            audioSources[currentLayer++].mute = false;
    }

    public void MuteAllBGM()
    {

    }

}
