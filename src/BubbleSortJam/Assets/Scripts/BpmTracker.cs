using UnityEngine;
using System;

public class BpmTracker : MonoBehaviour
{
    public static BpmTracker instance { get; private set; }

    [SerializeField]
    private uint BPM = 80;
    private float timePerBeatMS = 0.0f;
    public uint CurrentBeat { get; private set; }

    public event Action OnBeatInc;
    public event Action OnMeasure;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("BPM set to " + BPM);
        timePerBeatMS = 60.0f / BPM;
        InvokeRepeating("IncrementBPM", 0, timePerBeatMS);
        // e.g. 550, 1300, 2050
        InvokeRepeating("ToggleWindowOn", timePerBeatMS - (timeLineacy * 0.5f), timePerBeatMS);
        // e.g. 950, 1700, 2450
        InvokeRepeating("ToggleWindowOff", timePerBeatMS + (timeLineacy * 0.5f), timePerBeatMS);
    }

    private void IncrementBPM()
    {
        //BeatIncremented = true;
        ++CurrentBeat;
        OnBeatInc?.Invoke();
        if(CurrentBeat%4 == 0)
        {
            OnMeasure?.Invoke();
            //Debug.Log("On Measure Invoked!");
        }
        //Debug.Log("Current beat:" + CurrentBeat);
    }

    private void ToggleWindowOn()
    {
        BeatWindow = true;
        //Debug.Log("Beat Window turn on");
    }

    private void ToggleWindowOff()
    {
        BeatWindow = false;
        //Debug.Log("Beat Window turn offed");
    }

}
