
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;

    InventoryItem inventoryItem;
    public TMP_Text quantity;

    public void AddItem(InventoryItem newInventoryItem)
    {
        inventoryItem = newInventoryItem;

        icon.sprite = inventoryItem.item.icon;
        icon.enabled = true;
        removeButton.interactable = true;

        if (inventoryItem.item.isStackable)
        {
            quantity.text = inventoryItem.quantity.ToString();
            quantity.enabled = true;
        }
        else
        {
            quantity.enabled = false; 
        }
    }

    public void ClearSlot()
    {
        inventoryItem = null;

        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
        quantity.enabled = false;
    }

    public void UseButton()
    {
        if (inventoryItem != null)
        {
            inventoryItem.item.Use();
            Debug.Log($"{inventoryItem.item.name} clicked!");
            GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().RemoveFromInventory(inventoryItem);
        }
    }
    public void OnRemoveButton()
    {
        if (inventoryItem != null)
        {
            Debug.Log($"Removing {inventoryItem.item.name}!");
            GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().UseFromInventory(inventoryItem);
        }
        else
        {
            Debug.Log($"Nothing to remove");
        }
    }

    // public void UseItem()
    // {
    //     if (inventoryItem != null)
    //     {
    //         
    //         if (inventoryItem.item.isStackable)
    //         {
    //              inventoryItem.quantity--;
    //             
    //                 if (inventoryItem.quantity == 0)
    //                 {
    //                     
    //                     ClearSlot();
    //                     Inventory.instance.Remove(inventoryItem);
    //                 }
    //                 else
    //                 {
    //                     quantity.text = inventoryItem.quantity.ToString();
    //                 }
    //         }
    //        
    //     }
    // }
}
