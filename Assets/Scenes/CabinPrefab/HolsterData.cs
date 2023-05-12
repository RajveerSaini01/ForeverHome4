using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HolsterData", menuName = "PlayerData/HolsterData")]
public class HolsterData : ScriptableObject
{
    public List<Weapon> holster;
    public Weapon activeWeapon;

    // Could also implement some methods to set/read data,
    // do stuff with the data like parsing between types, fileIO etc

    // Especially ScriptableObjects also implement OnEnable and Awake
    // so you could still fill them with permanent data via FileIO at the beginning of your app and store the data via FileIO in OnDestroy !!

    private void Awake()
    {
        Debug.Log("Holster Scriptable Object Initialized");
    }

    private void OnEnable()
    {
        Debug.Log("Holster Scriptable Object ENABLED");
    }
    private void OnDisable()
    {
        Debug.Log("Holster Scriptable Object DISABLED");
    }
}