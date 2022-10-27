using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;
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

    [Header("Buttons")]
    private Button[] choices = null;
    private TextMeshProUGUI[] choicesText;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        
        OnEnable();
        onCloseInventory.Invoke();
        
    }

    private void OnEnable() {

        if (playerControls == null)
            playerControls = new PlayerControls();

        playerControls.Enable();
        playerControls.UI.Inventory.performed += InventoryButtonPressed;
        playerControls.UI.PauseMenu.performed += CloseButtonPressed;
        playerControls.UI.Cancel.performed += CloseButtonPressed;
    }

    private void OnDisable() {
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

        // inventoryUIElement = getInventoryUI();
        // if (inventoryUIElement == null)
        // {
        //     return;
        // }

        inventoryIsOpen = !inventoryIsOpen;

        if (inventoryIsOpen)
        {
            // inventoryUIElement.SetActive(true);
            
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
            // inventoryUIElement.SetActive(false);
            
            onCloseInventory.Invoke();
        }
    }

    void InitButtons()
    {
        // hide all item buttons
        foreach (Button choiceButton in choices)
        {
            choiceButton.gameObject.SetActive(false);
            choiceButton.onClick.RemoveAllListeners();
        }
    }

    void ItemButtons()
    {

        if (choices == null) {
            // Get choice buttons as children of the "InventoryButtons" object
            choices = GameObject.FindGameObjectWithTag("InventoryButtons").GetComponentsInChildren<Button>();

            choicesText = new TextMeshProUGUI[choices.Length];
            int cindex = 0;
            

            // Get buttons text
            foreach (Button choice in choices)
            {
                choicesText[cindex] = choice.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                cindex++;
            }
        }

        playerObject = GameObject.FindGameObjectWithTag("Player");
        inventoryObject = playerObject.GetComponent<InventoryObject>();

        InitButtons();

        int index = 0;
        foreach (InventorySlot slot in inventoryObject.Container.Items)
        {
            // Empty items have id "-1"
            if (slot.ID == -1)
            {
                index++;
                continue;
            }

            GameObject itemButton = choices[index].gameObject;

            // show button with text
            itemButton.SetActive(true);

            string prefix = "";
            if (inventoryObject.isEquipped(slot))
            {
                EventSystem.current.SetSelectedGameObject(itemButton);
                prefix = "<color=red>*</color> ";
            }

            choicesText[index].text = String.Format("{0}{1} (x{2})", prefix, slot.item.displayName, slot.amount.ToString());

            itemButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnItemButtonClick(slot));

            index++;
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
        ItemButtons();

    }
}
