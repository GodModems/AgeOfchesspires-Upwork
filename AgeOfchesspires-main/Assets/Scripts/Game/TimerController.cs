using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public TMP_Text timerText;

    private float startTime;
    private float timerDuration = 600f; // 10 minutes in seconds
    private float timeRemaining;
    private bool isTimerRunning = false;

    private string timerStr;

    void Start()
    {
        StartTimer();
        PauseTimer();
        ResetTimeRemaining();
    }

    public void ResetTimeRemaining()
    {
        timerDuration = 600f; // 10 minutes in seconds
        timeRemaining = 600f; //Mathf.Max(0, startTime + timerDuration - Time.time);
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timeRemaining = Mathf.Max(0, startTime + timerDuration - Time.time);
            UpdateTimerDisplay();

            if (timeRemaining < 0)
            {
                isTimerRunning = false;
                timeRemaining = 0;
                UpdateTimerDisplay();
                Debug.Log("Timer has finished!");

                GameObject.FindObjectOfType<MultiplayerGamePlay>().ShowGameOverTemp(true);
            }
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isTimerRunning = true;
    }

    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    public void ResumeTimer()
    {
        startTime = Time.time - (timerDuration - timeRemaining);
        isTimerRunning = true;
    }

    void UpdateTimerDisplay()
    {
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);

        RoomPlayer.Local.myTimerText = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = RoomPlayer.Local.myTimerText;
    }
}
