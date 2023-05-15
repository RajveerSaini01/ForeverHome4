using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndRounds : MonoBehaviour
{
    public TextMeshProUGUI roundsText;

    private void Start()
    {
        // Retrieve the "Kills" value from PlayerPrefs and update the text
        int rounds = PlayerPrefs.GetInt("Rounds", 0);
        roundsText.text = rounds.ToString();
    }
}
