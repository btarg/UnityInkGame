using UnityEngine;
using System.Collections.Generic;

public class InventoryObject : MonoBehaviour
{
    public Inventory Container;
    private InventorySlot equippedSlot;
    public List<string> pickedUpItems;

    public void AddItem(InventoryItem _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].ID == _item.Id)
            {
                Container.Items[i].AddAmount(_amount);
                return;
            }
        }
        SetEmptySlot(_item, _amount);

    }
    public InventorySlot SetEmptySlot(InventoryItem _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].ID <= -1)
            {
                Container.Items[i].UpdateSlot(_item.Id, _item, _amount);
                return Container.Items[i];
            }
        }
        //set up functionality for full inventory
        return null;
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.ID, item2.item, item2.amount);
        item2.UpdateSlot(item1.ID, item1.item, item1.amount);
        item1.UpdateSlot(temp.ID, temp.item, temp.amount);
    }


    public void RemoveItem(InventoryItem _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].item == _item)
            {
                Container.Items[i].UpdateSlot(-1, null, 0);
            }
        }
    }

    public void EquipSlot(InventorySlot slot) {
        if (slot.item == null)
            return;

        equippedSlot = slot;
        Debug.Log("Equipped item: " + slot.item.displayName + " of amount " + slot.amount);
    }

    public void UnequipCurrent() {
        equippedSlot = null;
        Debug.Log("Unequipped item");
        // TODO: make this do something
    }

    public bool isEquipped(InventorySlot slot) {
        return equippedSlot != null &&
        equippedSlot.ID == slot.ID
        && equippedSlot.item == slot.item
        && equippedSlot.amount == slot.amount;
    }

    public InventorySlot getEquippedSlot() {
        return equippedSlot;
    }

    [ContextMenu("Load")]
    public void Load()
    {
        SaveObject so = SaveHelper.currentSaveObject();
        Inventory newContainer = so.inventory;
        for (int i = 0; i < Container.Items.Length; i++)
        {
            Container.Items[i].UpdateSlot(newContainer.Items[i].ID, newContainer.Items[i].item, newContainer.Items[i].amount);
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
    }
}
[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[6];
}
[System.Serializable]
public class InventorySlot
{
    public int ID = -1;
    public InventoryItem item;
    public int amount;
    public InventorySlot()
    {
        ID = -1;
        item = null;
        amount = 0;
    }
    public InventorySlot(int _id, InventoryItem _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(int _id, InventoryItem _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}