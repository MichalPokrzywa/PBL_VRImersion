using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    [SerializeField] float targetTime = 30;
    [SerializeField] bool timeIsRunning = true;
    [SerializeField] TMP_Text timeText;

    float timer;

    public Action OnTimerEnd;

    void Start()
    {
        ResetTimer();
    }

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
        timeIsRunning = true;
        timer = targetTime;
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
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

}
