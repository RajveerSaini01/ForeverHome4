using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreHUD : MonoBehaviour
{
    private int kills;
    private int waves;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI wavesText;

    void Start()
    {
        // Load the values from PlayerPrefs, defaulting to 0 if they haven't been set yet
        PlayerPrefs.SetInt("Kills", 0);
        PlayerPrefs.Save();
        kills = 0;
        waves = PlayerPrefs.GetInt("Waves", 0);

        // Update the text
        killsText.text = kills.ToString();
        wavesText.text = waves.ToString();
        
        // Set default color as white
        Color color = new Color(1f, 1f, 1f);
        killsText.color = color;
        wavesText.color = color;
    }

    public void IncreaseKills()
    {
        // Increment the kills value and update the text
        kills++;
        killsText.text = kills.ToString();

        // Set color based on number of kills
        float r = Mathf.Lerp(1, 1, Mathf.Clamp01((float)kills / 20f));
        float g = 0f;
        float b = Mathf.Lerp(1, 0, Mathf.Clamp01((float)kills / 20f));
        Color color = new Color(r, g, b);
        killsText.color = color;

        // Save the kills value to PlayerPrefs
        PlayerPrefs.SetInt("Kills", kills);
        PlayerPrefs.Save();
    }

    public void IncreaseWaves()
    {
        // Increment the waves value and update the text
        waves++;
        wavesText.text = waves.ToString();

        //Change color of waves text based on waves completed
        float r = Mathf.Lerp(1, 1, Mathf.Clamp01((float)waves / 6f));
        float g = 0f;
        float b = Mathf.Lerp(1, 0, Mathf.Clamp01((float)waves / 6f));
        Color color = new Color(r, g, b);
        wavesText.color = color;

        // Save the waves value to PlayerPrefs
        PlayerPrefs.SetInt("Waves", waves);
        PlayerPrefs.Save();
    }
}