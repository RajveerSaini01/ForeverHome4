
using System;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
   public Item item;

   [SerializeField] private int quantity = 1; // for use 1-object multiple quantity case, like Ammo
   
   // Used by InteractHandler via SendMessage() during runtime.
   private void OnInteract(string nm)
   {
      // arg 'nm' unused at this point because Ray is doesn't understand how Andre set up the Item class / Interactable inheritance yet
      Debug.Log("Picking up Item. " + item.name);
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      player.GetComponent<Inventory>().AddToInventory(new InventoryItem(item, quantity));
      Destroy(gameObject);
   }
}
