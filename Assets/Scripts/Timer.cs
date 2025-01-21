using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    public float targetTime = 30;
    public bool timeIsRunning = true;
    public TMP_Text timeText;

    public Action OnTimerEnd;

    private void Start()
    {
        timeIsRunning = true;
    }

    private void Update()
    {
        if (timeIsRunning)
        {
            if(targetTime > 0)
            {
                targetTime -= Time.deltaTime;
                DisplayTime(targetTime);
            }
            else
            {
                targetTime = 0;
                timeIsRunning = false;
                OnTimerEnd?.Invoke();
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

}
