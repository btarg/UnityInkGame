using UnityEngine;
using System.Collections.Generic;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem item;
    public int amount = 1;
    public bool canPickup = true;
    public bool destroyOnPickup = true;
    
    GameObject player;
    InventoryObject inventoryObject;

    private void Awake()
    {
        // Check if this pickup has already been collected
        player = GameObject.FindGameObjectWithTag("Player");
        inventoryObject = player.GetComponentInChildren<InventoryObject>();

        // Try loading if the list is empty to start with
        if (inventoryObject.pickedUpItems.Count < 1) {
            inventoryObject.Load();
        }
        
        foreach (string name in inventoryObject.pickedUpItems)
        {
            if (name == gameObject.name)
            {
                canPickup = false;
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
                return;
            }
        }

    }

    public void GivePlayerItem()
    {
        if (!canPickup || amount == 0)
            return;

        Debug.Log("Giving player item: " + item);
        player = GameObject.FindGameObjectWithTag("Player");
        inventoryObject = player.GetComponentInChildren<InventoryObject>();
        inventoryObject.AddItem(item, amount);

        // Add to save file to be destroyed when loaded
        if (!inventoryObject.pickedUpItems.Contains(gameObject.name)) {
            inventoryObject.pickedUpItems.Add(gameObject.name);
        }

        canPickup = false;

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}
