using UnityEngine;
using TMPro;
using System;

public class TimerQM : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private float elapsedTime;
    private bool isRunning;

    void Start()
    {
        elapsedTime = 0f;
        //isRunning = true; // Start the timer as running
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
            timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
    }
}
