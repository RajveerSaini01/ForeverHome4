using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCounter : MonoBehaviour
{
    private int enemyCount;
    private ScoreHUD _scoreHUD;

    private void Awake()
    { 
        LevelGeneration.OnReady += OnMapReady;
        TestSceneManager.OnReady += OnMapReady;
    }

    private void OnMapReady()
    {
        // Count the gameobjects with the "enemy" tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        enemyCount = enemies.Length;

        // Print the enemy count
        Debug.Log("Number of enemies: " + enemyCount);
    }
    void Update()
    {
        if(enemyCount <= 0){
            _scoreHUD = GameObject.FindGameObjectWithTag("Player").GetComponent<ScoreHUD>();
            _scoreHUD.IncreaseWaves();
            SceneManager.LoadScene("PsychoWorld");
        }
    }
}
