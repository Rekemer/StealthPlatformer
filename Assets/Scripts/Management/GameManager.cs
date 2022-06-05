using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singletone<GameManager>
{
    public bool IsGameWon { get; set; }
    public bool IsGameOver { get; set; }
    private float timeLeft;

    void Start()
    {
       
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