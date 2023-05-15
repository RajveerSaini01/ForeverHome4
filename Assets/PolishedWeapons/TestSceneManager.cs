using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneManager : MonoBehaviour
{
    public static event Action OnReady;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Invoking Ready State");
        OnReady?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
