using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InventoryData", menuName = "PlayerData/InventoryData")]
public class InventoryData : ScriptableObject
{
    public List<InventoryItem> inventory;
    
    private void Awake()
    {
        Debug.Log("Inventory Scriptable Object Initialized");
    }

    private void OnEnable()
    {
        Debug.Log("Inventory Scriptable Object ENABLED");
    }
    
    private void OnDisable()
    {
        Debug.Log("Inventory Scriptable Object DISABLED");
    }
}