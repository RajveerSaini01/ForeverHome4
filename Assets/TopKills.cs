using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TopKills : MonoBehaviour
{
    public TextMeshProUGUI killsText;

    private void Start()
    {
        int kills = PlayerPrefs.GetInt("Kills", 0);
        int topKills = PlayerPrefs.GetInt("TopKills", 0);

        if (kills > topKills)
        {
            // Update TopKills value and text if kills is higher
            PlayerPrefs.SetInt("TopKills", kills);
            PlayerPrefs.Save();

            killsText.text = kills.ToString() + "\nNew Record!";
        }
        else
        {
            killsText.text = topKills.ToString();
        }
    }
}
