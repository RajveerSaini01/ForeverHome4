using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pause : MonoBehaviour
{
    public bool paused;

    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        paused = false;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleState();
        }
    }

    public void ToggleState()
    {
        paused = !paused;
        pauseMenu.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;

    }
    
    // public void ResumeGame()
    // {
    //     pauseMenu.SetActive(false);
    //     Time.timeScale = 1f;
    //     isGamePaused = false;
    // }
    //
    // void PauseGame()
    // {
    //     pauseMenu.SetActive(true);
    //     Time.timeScale = 0f;
    //     isGamePaused = true;
    // }
    
    
}
