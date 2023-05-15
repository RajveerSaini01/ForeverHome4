using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    // Code adapted from Comp-3 Interactive channel on youtube

    public float radius;
    public float innerRadius;
    
    [Range(0, 360)]
    public float angle;
    public GameObject playerRef;
    public LayerMask playerMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer;

    private void Awake()
    {
        LevelGeneration.OnReady += OnMapReady;

        TestSceneManager.OnReady += OnMapReady;
    }
 
    void OnMapReady()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        if (playerRef == null)
        {
            Debug.LogError($"{name} could not find GameObject with tag 'Player'");
        }
        StartCoroutine(FOVRoutine());
    }

    //Every 2 milliseconds, check if player is within LOS. Saves on performance. 
    private IEnumerator FOVRoutine()
    {
        while(true)
        {
            //Check for distance threshold ("hearing") and angle ("sight")
            // Then do a RayCast to check for any obstructions in LoS.
            
            yield return new WaitForSeconds(0.2f); // DONT PUT THIS AT BOTTOM OR Continue; WILL CAUSE INFINITE LOOP
            Vector3 playerPos = playerRef.transform.position;
            float playerDistance = Vector3.Distance(transform.position, playerPos);
            // "Hearing" Check
            if (playerDistance < innerRadius)
            {
                canSeePlayer = true;
                continue;
            }

            // "Sight Cone" Check
            Vector3 playerDir = (playerPos - transform.position).normalized;
            if (playerDistance < radius && Vector3.Angle(transform.forward, playerDir) < angle/2f)
            {
                Ray ray = new(transform.position, playerDir);
                if (Physics.Raycast(ray.origin, ray.direction, obstructionMask)) // Mask is what to ignore for collisions
                {
                    canSeePlayer = true;
                    continue;
                }
            }
            canSeePlayer = false;
        }
    }
}
