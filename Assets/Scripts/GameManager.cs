using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    [SerializeField] private Timer timer;
    [SerializeField] private float maxTime;
    private float timeLeft;

    void Start()
    {
        timer.Init(maxTime);
        timeLeft = maxTime;
        StartCountDown();
    }

    private void StartCountDown()
    {
        StartCoroutine(CountDownRoutine());
    }

    IEnumerator CountDownRoutine()
    {
        while (timeLeft > 0)
        {
            if (timer != null && timer.isPaused == false)
            {
                timer.UpdateTimer(timeLeft);
                timeLeft -= Time.deltaTime;
            }

            yield return null;
        }
    }

    public void AddTime(float time)
    {
        timer.UpdateTimer(timeLeft + time);
        timeLeft = Mathf.Clamp(timeLeft + time, 0f, maxTime);
    }

    public void Pause()
    {
        timer.isPaused = true;
    }

    public void Continue()
    {
        timer.isPaused = false;
    }
}