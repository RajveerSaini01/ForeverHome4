
using System;
using UnityEngine;

public class ItemPickup : Interactable
{
   public Item item;
   
   
   public void Start()
   {
      // At this point FirstPersonController does not exist
      LevelGeneration.OnReady += OnMapReady;
      
   }

   private void OnMapReady()
   {
      // At this point,FirstPersonController exists
   }

   public override void Interact()
   {
      base.Interact();
   
      PickUp();
   }

   void PickUp()
   {
      Debug.Log("Inside Item " + item.name);
      
         Debug.Log("Picking up Item. " + item.name);
         bool wasPickedUp = Inventory.instance.Add(item);
      
         if(wasPickedUp)
            Destroy(gameObject);
      
   }
}
