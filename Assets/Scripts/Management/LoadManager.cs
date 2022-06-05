using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : Singletone<LoadManager>
{
    public override void Awake()
    {
        base.Awake();
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

    private void LoadHUD()
    {
        SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
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