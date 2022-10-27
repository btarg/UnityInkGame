using System;
using UnityEngine;
using System.Collections.Generic;


public class InventoryObject : MonoBehaviour
{
    public Inventory Container;
    private InventorySlot equippedSlot = null;
    public List<string> pickedUpItems;

    private void Awake()
    {
        pickedUpItems = new List<string>();
    }

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

        StatusConsole.PrintToConsole(String.Format("Added <color=yellow>{0}</color> (x{1}) to your inventory", _item.displayName, _amount));

    }

    public InventorySlot SetEmptySlot(InventoryItem _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].ID <= -1)
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


    public void RemoveItem(InventoryItem _item, int removeAmount = -1)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item)
            {
                InventorySlot currentItem = Container.Items[i];
                string name = currentItem.item.displayName;

                // -1 remove amount purges the whole slot
                if ((currentItem.amount - removeAmount) <= 0 || removeAmount == -1)
                {
                    Container.Items[i].UpdateSlot(-1, null, 0);
                }
                else
                {
                    Container.Items[i].UpdateSlot(currentItem.item.Id, currentItem.item, currentItem.amount - removeAmount);
                }
                
                if (removeAmount == -1) {
                    StatusConsole.PrintToConsole(String.Format("Removed every <color=yellow>{0}</color> from your inventory", name));
                } else {
                    StatusConsole.PrintToConsole(String.Format("Removed <color=yellow>{0}</color> (x{1}) from your inventory", name, removeAmount));

                }
            }
        }
    }

    public void EquipSlot(InventorySlot slot)
    {
        if (slot.item == null)
            return;

        equippedSlot = slot;
        Debug.Log("Equipped item: " + slot.item.displayName + " of amount " + slot.amount);
    }

    public void UnequipCurrent()
    {
        equippedSlot = null;
        Debug.Log("Unequipped item");
        // TODO: make this do something
    }

    public bool isEquipped(InventorySlot slot)
    {
        return equippedSlot != null &&
        equippedSlot.ID == slot.ID
        && equippedSlot.item == slot.item
        && equippedSlot.amount == slot.amount;
    }

    public InventorySlot getEquippedSlot()
    {
        return equippedSlot;
    }

    [ContextMenu("Load")]
    public void Load()
    {
        SaveObject so = SaveHelper.currentSaveObject();

        if (so == null)
        {
            return;
        }

        pickedUpItems = so.pickedUpItems;

        Inventory newContainer = so.inventory;
        for (int i = 0; i < Container.Items.Length; i++)
        {
            Container.Items[i].UpdateSlot(newContainer.Items[i].ID, newContainer.Items[i].item, newContainer.Items[i].amount);
        }
        EquipSlot(so.equippedSlot);

    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
        equippedSlot = null;
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