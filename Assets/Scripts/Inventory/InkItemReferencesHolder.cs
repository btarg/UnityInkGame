using UnityEngine;

public class InkItemReferencesHolder : MonoBehaviour
{
    private static InkItemReferencesHolder instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one References Holder in the scene");
        }
        instance = this;

    }

    [SerializeField] InventoryItem[] items;
    [SerializeField] GameEvent[] gameEvents;

    public static GameEvent GetEventFromName(string name)
    {
        foreach (GameEvent e in instance.gameEvents)
        {
            if (e.name == name)
            {
                return e;
            }
        }
        return null;
    }

    public static InventoryItem GetItemFromId(int itemID)
    {
        foreach (InventoryItem item in instance.items)
        {
            if (item.Id == itemID)
            {
                return item;
            }
        }

        return null;

    }
}
