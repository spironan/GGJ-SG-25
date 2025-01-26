using TMPro;
using UnityEngine;

public class TimerDebug : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float TimeSinceStarted = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TimeSinceStarted += Time.deltaTime;
        
        int minutes = Mathf.FloorToInt(TimeSinceStarted / 60);
        int seconds = Mathf.FloorToInt(TimeSinceStarted % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes,seconds);
    }
}
