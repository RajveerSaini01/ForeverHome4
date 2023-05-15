using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class InsultScript : MonoBehaviour
{
    public TextMeshProUGUI insultText;
    private string[] insults = {
        "Nice try, but not quite good enough.",
        "You're not really cut out for this, are you?",
        "I mean, I'm sure there's someone out there who's worse than you...",
        "You know, some people just aren't meant to succeed.",
        "It's okay, not everyone can be a winner.",
        "Well, that was... something.",
        "You might want to consider playing a different game.",
        "I'm not sure if that was bad luck or just plain incompetence.",
        "Maybe try practicing a little more before you play again.",
        "I'm not saying you're terrible, but...",
        "Well, you certainly gave it your all. Or did you?",
        "I'm sure you'll do better next time. Just kidding, I have no idea." ,
        "You should stick with making games instead of playing them.",
        "Did your graphics programming knowledge help you this time?",
        "Maybe you should get back to your three jobs..."
    };

    private void Awake()
    {
        DisplayInsult();
    }

    public void DisplayInsult()
    {
        // Pick a random insult from the array
        string insult = insults[Random.Range(0, insults.Length)];
        
        // Update the text to display the insult
        insultText.text = insult;
    }
}