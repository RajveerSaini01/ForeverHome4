using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countdown;
    public TextMeshProUGUI warning;
    private float timer = 600.0f; // 10 minutes in seconds
    private bool red = false;
    private bool countdownStarted = false;
    private float countdownTimer = 120.0f; // 2 minutes in seconds

    void FixedUpdate()
    {
        if (countdownTimer > 0)
        {
            timer -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            int milliseconds = Mathf.FloorToInt((timer * 100) % 100);
            countdown.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
            //Entering Wave of Enemies
            if (timer <= 0f && !countdownStarted) // start countdown after 10 minutes
            {
                SceneManager.LoadScene("HomeWorld");
                /*
                countdownStarted = true;
                countdown.color = Color.red;
                warning.text = "KILL THEM ALL";
                red = true;*/
            }

            if (countdownStarted && countdownTimer > 0)
            {
                countdownTimer -= Time.deltaTime;
                minutes = Mathf.FloorToInt(countdownTimer / 60);
                seconds = Mathf.FloorToInt(countdownTimer % 60);
                milliseconds = Mathf.FloorToInt((countdownTimer * 100) % 100);
                countdown.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
            }
            //Exiting Waves of Enemies
            else if (countdownStarted && countdownTimer <= 0)
            {
                countdownStarted = false;
                countdown.color = Color.white;
                warning.text = "BEFORE NEXT WAVE";
                red = false;
                countdownTimer = 10.0f;
            }
        }
        else
        {
            timer = 600.0f;
            countdownStarted = false;
            countdown.color = Color.white;
            warning.text = "BEFORE NEXT WAVE";
            red = false;
            countdownTimer = 120.0f;
        }
    }
}