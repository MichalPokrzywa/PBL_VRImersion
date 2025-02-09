using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    [SerializeField] float targetTime = 30;
    [SerializeField] bool timeIsRunning = false;
    [SerializeField] TMP_Text timeText;

    float timer;
    public string TimerValue => FormatTime(timer);
    public string TimePassed => FormatTime(targetTime - timer);

    public Action OnTimerEnd;

    void Update()
    {
        if (timeIsRunning)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                DisplayTime(timer);
            }
            else
            {
                timer = 0;
                timeIsRunning = false;
                OnTimerEnd?.Invoke();
            }
        }
    }

    public void ResetTimer()
    {
        timer = targetTime;
        timeIsRunning = true;
        DisplayTime(timer);
    }

    public void StopTimer()
    {
        timeIsRunning = false;
    }

    public void ResumeTimer()
    {
        timeIsRunning = true;
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay = Mathf.Max(timeToDisplay, 0);
        timeText.text = FormatTime(timeToDisplay);
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
