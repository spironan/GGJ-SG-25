using UnityEngine;
using System;

public class BpmTracker : MonoBehaviour
{
    public static BpmTracker instance { get; private set; }

    [SerializeField]
    private uint BPM = 80;
    private float timePerBeatMS = 0.0f;
    public uint CurrentBeat { get; private set; }
    public uint CurrentMeasure { get; private set; } = 0;

    public event Action OnBeatInc;
    public event Action OnMeasure;

    public event Action OnWindowOpen;
    public event Action OnWindowClose;

    public bool BeatWindow { get; private set; } = false;
    [SerializeField]
    private float timeLineacy = 0.40f; // time in ms +- both sides

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        OnMeasure += HardCodeAudioPlayTimes;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnGameStart()
    {
        Debug.Log("BPM set to " + BPM);
        timePerBeatMS = 60.0f / BPM;
        InvokeRepeating("IncrementBPM", 0, timePerBeatMS);
        // e.g. 550, 1300, 2050
        InvokeRepeating("ToggleWindowOn", timePerBeatMS - (timeLineacy * 0.5f), timePerBeatMS);
        // e.g. 950, 1700, 2450
        InvokeRepeating("ToggleWindowOff", timePerBeatMS + (timeLineacy * 0.5f), timePerBeatMS);

        BPMChangedGameplayEvent.BroadcastEvent(BPM);
    }

    public void OnGameEnd()
    {
        CancelInvoke();
    }

    private void IncrementBPM()
    {
        //BeatIncremented = true;
        ++CurrentBeat;
        OnBeatInc?.Invoke();
        if(CurrentBeat%4 == 0)
        {
            ++CurrentMeasure;
            OnMeasure?.Invoke();
            //Debug.Log("On Measure Invoked!");
        }
        //Debug.Log("Current beat:" + CurrentBeat);
    }

    private void ToggleWindowOn()
    {
        BeatWindow = true;
        OnWindowOpen?.Invoke();
        //Debug.Log("Beat Window turn on");
    }

    private void ToggleWindowOff()
    {
        BeatWindow = false;
        OnWindowClose?.Invoke();
        //Debug.Log("Beat Window turn offed");
    }

    private void HardCodeAudioPlayTimes()
    {
        switch (CurrentMeasure)
        {
            case 5:
            case 13:
            case 17:
            case 25:
                MasterAudioController.instance.UnmuteNextDynamicBGMLayer();
                break;
        }
    }
}
