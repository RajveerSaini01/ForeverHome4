using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndKills : MonoBehaviour
{
    public TextMeshProUGUI killsText;

    private void Start()
    {
        // Retrieve the "Kills" value from PlayerPrefs and update the text
        int kills = PlayerPrefs.GetInt("Kills", 0);
        killsText.text = kills.ToString();
    }
    
}
