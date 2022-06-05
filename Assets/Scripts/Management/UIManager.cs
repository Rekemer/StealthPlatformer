using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singletone<UIManager>
{
    [SerializeField] private GameObject endGameWinScreen;
    [SerializeField] private GameObject endGameLoseScreen;
    void Start()
    {
        EventSystem.current.OnGettingSpotted += ShowLoseScreen;
        EventSystem.current.OnGettingToFinish += ShowWinScreen;
        endGameWinScreen.SetActive(false);
        endGameLoseScreen.SetActive(false);
    }

    public void ShowWinScreen()
    {
        endGameWinScreen.SetActive(true);
    } 
    public void ShowLoseScreen()
    {
        endGameLoseScreen.SetActive(true);
    }
    void Update()
    {
        
    }
}
