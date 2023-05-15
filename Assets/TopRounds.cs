using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopRounds : MonoBehaviour
{
    public TextMeshProUGUI killsText;

    private void Start()
    {
        //I know I refer to them as waves everywhere else but i forgot :(
        int rounds = PlayerPrefs.GetInt("Waves", 0);
        int topRounds = PlayerPrefs.GetInt("TopRounds", 0);

        if (rounds > topRounds)
        {
            // Update TopKills value and text if kills is higher
            PlayerPrefs.SetInt("TopRounds", rounds);
            PlayerPrefs.Save();

            killsText.text = rounds.ToString() + "\nNew Record!";
        }
        else
        {
            killsText.text = topRounds.ToString();
        }
    }
}
