using UnityEngine;

public class MasterAudioController : MonoBehaviour
{
    public static MasterAudioController instance { get; private set; }

    [SerializeField]
    private GameObject sfxSource, bgmSource;

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

    public void UnmuteBGMLayer(uint layer = 0)
    {

    }

    public void MuteAllBGM()
    {

    }

}
