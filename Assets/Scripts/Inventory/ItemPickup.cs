using UnityEngine;
using System.Collections.Generic;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem item;
    public int amount = 1;
    public bool canPickup = true;
    public bool destroyOnPickup = true;

    private void Start()
    {
        // Check if this pickup has already been collected
        SaveObject saveObject = SaveHelper.currentSaveObject() ?? null;
        if (saveObject == null)
        {
            return;
        }
        List<string> pickedUp = saveObject.pickedUpItems;


        pickedUp.ForEach(delegate (string name)
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
        });

    }

    public void GivePlayerItem()
    {
        if (!canPickup || amount == 0)
            return;

        Debug.Log("Giving player item: " + item);
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        InventoryObject inventoryObject = playerObject.GetComponent<InventoryObject>();
        inventoryObject.AddItem(item, amount);

        // Add to save file to be destroyed when loaded
        inventoryObject.pickedUpItems.Add(gameObject.name);
        canPickup = false;

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}
