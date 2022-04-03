using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singletone<GameManager>
{
    [SerializeField] private Timer timer;
    [SerializeField] private float maxTime;
    public bool IsGameWon { get; set; }
    public bool IsGameOver { get; set; }
    private float timeLeft;

    void Start()
    {
        timer.Init(maxTime);
        timeLeft = maxTime;
        StartCountDown();
    }

    void Update()
    {
        if (IsGameWon)
        {
            UIManager.Instance.ShowWinScreen();
        }
        else
        {
            if (IsGameOver)
            {
                UIManager.Instance.ShowLoseScreen();
            }
            
        }
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

        IsGameOver = true;
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

    public void ReloadScene()
    {
        var indexOfScene = SceneManager.GetActiveScene().buildIndex;
        LoadLevel(indexOfScene);
    }
    public void LoadLevel(string levelName)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (Application.CanStreamedLevelBeLoaded(levelIndex))
        {
            if (levelIndex >= 0 && SceneManager.sceneCountInBuildSettings > levelIndex)
            {
                SceneManager.LoadScene(levelIndex);
            }
            else
            {
                Debug.LogWarning("invalid scene index");
            }
        }
        
    }
    public void LoadNextLevel()
    {
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        var nextSceneIndex = ++currentSceneIndex;
        var totalSceneCount = SceneManager.sceneCountInBuildSettings;
        LoadLevel(nextSceneIndex % totalSceneCount);
    }
}