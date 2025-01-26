using UnityEngine;

public class PlaySFXHookups : MonoBehaviour
{
    public AudioClip metronome;
    public AudioClip correct;
    public AudioClip bubblePop;
    public AudioClip lose;
    public AudioClip rowclear;
    public AudioClip winSFX;

    public Player player;
    public Life playerHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BpmTracker.instance.OnBeatInc += PlaySFXOnBeatInc;
        player.OnPlayerSuccessfulAction += PlaySFXOnSuccessfulSwap;
        playerHP.OnDamageTaken += PlaySFXOnDmgTaken;
        playerHP.OnDeath+= PlaySFXOnDeath;
        GameManager.instance.OnCompleteRow += PlaySFXOnRowClear;
        GameManager.instance.OnCompleteAll += PlaySFXOnFinish;
    }

    private void PlaySFXOnFinish()
    {
        Debug.Log("SFX Played On Win");
        MasterAudioController.instance.PlaySFX(winSFX);
    }
    private void PlaySFXOnBeatInc()
    {
        Debug.Log("SFX Played On Successfull Swap");
        MasterAudioController.instance.PlaySFX(metronome, 0.08f);
    }

    private void PlaySFXOnSuccessfulSwap()
    {
        Debug.Log("SFX Played On Successfull Swap");
        MasterAudioController.instance.PlaySFX(correct);
    }

    private void PlaySFXOnDmgTaken()
    {
        Debug.Log("SFX Played On Damage Taken");
        MasterAudioController.instance.PlaySFX(bubblePop);
    }

    private void PlaySFXOnDeath()
    {
        Debug.Log("SFX Played On Death");
        MasterAudioController.instance.PlaySFX(lose);
    }

    private void PlaySFXOnRowClear()
    {
        Debug.Log("SFX Played On Row Cleared");
        MasterAudioController.instance.PlaySFX(rowclear);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
