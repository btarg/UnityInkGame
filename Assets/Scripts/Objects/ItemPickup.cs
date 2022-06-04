using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem item;
    public int amount = 1;
    public bool canPickup = true;

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
        Destroy(gameObject);
    }
}
