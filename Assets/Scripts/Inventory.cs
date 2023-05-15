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
    public GameObject inventoryCanvas;
    public Transform itemsParent;

    
    
    
    private void Awake()
    {
        if (inventoryData != null)
        {
            inventory = inventoryData.inventory;
            slots = itemsParent.GetComponentsInChildren<InventorySlot>();
            UpdateUI();
            
            Debug.Log($"Parent found: {inventoryCanvas.name}");
        }
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
        Camera cam = Camera.main;
        GameObject drop = Instantiate(inventoryItem.item.itemDropPrefab, cam.transform.position,  cam.transform.rotation);
        
        Ray camRay = cam.ViewportPointToRay(new Vector3(0.5f,0.5f, 0f));
        Rigidbody dropPhysics = drop.GetComponent<Rigidbody>();
        dropPhysics.AddForce(camRay.direction * 1.2f, ForceMode.Impulse); // Why doesnt this go forwards?
            
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
            Debug.Log($"Toggled");
            Debug.Log($"{inventoryCanvas.name}");
            //inventoryCanvas.SetActive(true);
            
            inventoryCanvas.SetActive(!inventoryCanvas.activeInHierarchy);
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