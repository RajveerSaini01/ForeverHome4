using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Scriptable Object Data
    [SerializeField] private InventoryData inventoryData;
    public int capacity = 20;
    private List<InventoryItem> inventory = new();
    
    // Canvas GameObject Data
    private InventorySlot[] slots;
    [SerializeField] private GameObject inventoryCanvas;
    [SerializeField] private Transform itemsParent;
    private void Awake()
    {
        inventory = inventoryData.inventory;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        UpdateUI();
    }

    public void AddToInventory(InventoryItem inventoryItem)
    {
        Debug.Log($"Adding {inventoryItem.item.name} to inventory");

        // For every item in my inventory, if the item im adding into it already exists, increment the existing item's quantity, instead of adding new.
        bool preExisting = false;
        foreach (InventoryItem item in inventory)
        {
            if (item.item.name == inventoryItem.item.name)
            {
                item.quantity += inventoryItem.quantity; // add incoming quantity to already present quantity
            }
            preExisting = true;
            break;
        }

        if (!preExisting) // this check can be done smarter, by utilizing the fact that the inventory wont Contain() if it doesnt exist
        {
            inventory.Add(inventoryItem);
        }

        UpdateUI();
    }
    
    public void UseFromInventory(InventoryItem inventoryItem)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject drop = Instantiate(inventoryItem.item.itemDropPrefab, player.transform.position, Quaternion.identity);
        drop.GetComponent<Rigidbody>().AddForce(player.transform.forward, ForceMode.Impulse);
            
        // piggyback
        RemoveFromInventory(inventoryItem);
    }
    
    public void RemoveFromInventory(InventoryItem inventoryItem)
    {
        Debug.Log($"Removing {inventoryItem.item.name} from inventory");

        if (inventoryItem.quantity > 1)
        {
            inventoryItem.quantity--;
        }
        else
        {
            inventory.Remove(inventoryItem);
        }

        UpdateUI();
    }
    
    private void Update()
    {
        // Toggle inventory
        if (Input.GetButtonDown("MainInventory"))
        {
            inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
        }
    }
    private void UpdateUI()
    {
        Debug.Log("Updating UI");
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.Count)
            {
                slots[i].AddItem(inventory[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}

[Serializable]
public class InventoryItem
{
    public Item item;
    public int quantity;
    public InventoryItem(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}