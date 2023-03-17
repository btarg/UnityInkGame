using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(PauseMenu))]
public class InventoryUIManager : MonoBehaviour
{
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    GameObject inventoryUIElement;
    PlayerControls playerControls;
    public static bool inventoryIsOpen = false;

    GameObject playerObject;
    InventoryObject inventoryObject;

    public TextMeshProUGUI itemLabel;

    [Header("Buttons")]
    public GameObject closeButton;
    public GameObject showOnEmpty;
    public Sprite unknownItemSprite;
    public GameObject buttonPrefab;
    private List<GameObject> choices = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");

        OnEnable();
        onCloseInventory.Invoke();

    }

    private void OnEnable()
    {

        if (playerControls == null)
            playerControls = new PlayerControls();

        playerControls.Enable();
        playerControls.UI.Inventory.performed += InventoryButtonPressed;
        playerControls.UI.PauseMenu.performed += CloseButtonPressed;
        playerControls.UI.Cancel.performed += CloseButtonPressed;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.UI.Inventory.performed -= InventoryButtonPressed;
        playerControls.UI.PauseMenu.performed -= CloseButtonPressed;
        playerControls.UI.Cancel.performed -= CloseButtonPressed;
    }

    void CloseButtonPressed(InputAction.CallbackContext context)
    {
        if (inventoryIsOpen)
        {
            InventoryButtonPressed(context);
        }
    }

    void InventoryButtonPressed(InputAction.CallbackContext context)
    {
        ToggleInventory();
    }

    public void ToggleInventory()
    {
        if (PauseMenu.PauseMenuOpen || DialogueManager.GetInstance().dialogueIsPlaying)
            return;

        inventoryIsOpen = !inventoryIsOpen;

        if (inventoryIsOpen)
        {

            onOpenInventory.Invoke();


            ItemButtons();
            // default to selecting first option if there is no item equipped but there is atleast 1 option
            if (!EventSystem.current.alreadySelecting && choices[0].gameObject.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
            }

        }
        else
        {
            onCloseInventory.Invoke();
        }
    }

    void CreateInventoryButton(int cindex)
    {
        GameObject itemButtonInst = Instantiate(buttonPrefab, GameObject.FindGameObjectWithTag("InventoryButtons").transform);
        choices.Insert(cindex, itemButtonInst);
        cindex++;
    }

    void CreateButtons(InventoryObject inventoryObject)
    {
        // while there are not enough buttons to display the inventory, add more buttons
        int itemCount = inventoryObject.Container.Items.Length;

        for (var i = 0; i < itemCount; i++)
        {
            if (itemCount > choices.Count)
            {
                CreateInventoryButton(i);
            }

        }

        // hide all item buttons
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
            choiceButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }

    void ItemButtons()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        inventoryObject = playerObject.GetComponentInChildren<InventoryObject>();
        CreateButtons(inventoryObject);

        int index = 0;
        foreach (InventorySlot slot in inventoryObject.Container.Items)
        {
            // Empty items have id "-1"
            if (slot.ID == -1)
            {
                if (inventoryObject.Container.Items.Length == index)
                {
                    showOnEmpty.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(closeButton);
                }
                else
                {
                    index++;
                    continue;
                }

            }
            showOnEmpty.SetActive(false);
            GameObject itemButton = choices.ToArray()[index];

            InventoryItemCell cell = itemButton.GetComponent<InventoryItemCell>();

            cell.slotIndex = index;

            if (slot.amount > 1) {
                cell.itemAmount.text = "x" + slot.amount.ToString();
            } else {
                cell.itemAmount.text = "";
            }
            

            Sprite buttonSprite = unknownItemSprite;
            if (slot.item.sprite != null)
            {
                buttonSprite = slot.item.sprite;
            }
            cell.itemIconImage.sprite = buttonSprite;

            itemButton.SetActive(true);

            if (inventoryObject.isEquipped(slot))
            {
                EventSystem.current.SetSelectedGameObject(itemButton);
                cell.isEquipped = true;
            }
            else
            {
                itemButton.GetComponent<InventoryItemCell>().isEquipped = false;
            }

            itemButton.GetComponent<Button>().onClick.AddListener(() => OnItemButtonClick(slot));

            index++;
        }

    }

    public void UpdateItemLabel(int slotIndex)
    {
        itemLabel.text = "";
        if (slotIndex != -1)
        {
            InventorySlot slot = inventoryObject.Container.Items[slotIndex];
            itemLabel.text = slot.item.displayName;
            if (inventoryObject.isEquipped(slot)) {
                itemLabel.text = "<color=yellow>" + itemLabel.text + "</color>";
            }
        
        }
        

    }

    void OnItemButtonClick(InventorySlot slot)
    {
        if (!inventoryObject.isEquipped(slot))
        {
            inventoryObject.EquipSlot(slot);
        }
        else
        {
            inventoryObject.UnequipCurrent();
        }
        UpdateItemLabel(Array.IndexOf(inventoryObject.Container.Items, slot));
        ItemButtons();

    }
}
