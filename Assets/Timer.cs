using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class Timer : MonoBehaviour
{
     float countdown = 300;
     public TMP_Text tex;

     public TMP_Text minutes;

     public TMP_Text seconds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (countdown > 0)
        {
            countdown -= Time.deltaTime;
        }

        double b = System.Math.Round(countdown, 2);
        tex.text = b.ToString();

        float min = Mathf.FloorToInt(countdown / 60);
        float sec = Mathf.FloorToInt(countdown % 60);
        minutes.text = min.ToString();
        seconds.text = sec.ToString();

        if (countdown < 3f)
        {
            SceneManager.LoadScene("Home");
        }
    }
}
