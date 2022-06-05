using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDLoader : MonoBehaviour
{
    private void Awake()
    {
        if (SceneManager.GetSceneByName("HUD").isLoaded == false)
        {
            SceneManager.LoadScene("HUD", LoadSceneMode.Additive);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
