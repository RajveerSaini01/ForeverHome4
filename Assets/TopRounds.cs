using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopRounds : MonoBehaviour
{
    public TextMeshProUGUI killsText;

    private void Start()
    {
        int rounds = PlayerPrefs.GetInt("Kills", 0);
        int topRounds = PlayerPrefs.GetInt("TopRounds", 0);

        if (rounds > topRounds)
        {
            // Update TopKills value and text if kills is higher
            PlayerPrefs.SetInt("TopRounds", rounds);
            PlayerPrefs.Save();

            killsText.text = rounds.ToString() + "\n<color=gold>New Record!</color>";
        }
        else
        {
            killsText.text = topRounds.ToString();
        }
    }
}
