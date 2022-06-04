using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(PauseMenu))]
public class InventoryUIManager : MonoBehaviour
{
    GameObject inventoryUIElement;
    PauseMenu pauseMenu;
    PlayerControls playerControls;
    public static bool inventoryIsOpen = false;

    GameObject playerObject;
    InventoryObject inventoryObject;

    [Header("Buttons")]
    [SerializeField] GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    // Start is called before the first frame update
    void Start()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerControls.UI.Inventory.performed += InventoryButtonPressed;
        playerControls.UI.PauseMenu.performed += CloseButtonPressed;
        playerControls.UI.Cancel.performed += CloseButtonPressed;

        playerObject = GameObject.FindGameObjectWithTag("Player");

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;

        // Get buttons text
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        InitButtons();
        inventoryUIElement = getInventoryUI();

        if (inventoryUIElement != null)
            inventoryUIElement.SetActive(false);
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

        inventoryUIElement = getInventoryUI();
        if (inventoryUIElement == null)
        {
            return;
        }

        inventoryIsOpen = !inventoryIsOpen;

        if (inventoryIsOpen)
        {
            inventoryUIElement.SetActive(true);
            pauseMenu.StopTime();
            ItemButtons();
            // default to selecting first option if there is no item equipped but there is atleast 1 option
            if (!EventSystem.current.alreadySelecting && choices[0].activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(choices[0]);
            }

        }
        else
        {
            inventoryUIElement.SetActive(false);
            pauseMenu.Resume();

            InitButtons();
        }
    }

    public GameObject getInventoryUI()
    {
        // stupid hacky shit because GetChild sucks
        foreach (Transform child in gameObject.transform)
        {
            if (child != null && child.gameObject.CompareTag("InventoryMenu"))
            {
                return child.gameObject;
            }
        }
        return null;
    }

    void InitButtons()
    {
        pauseMenu = gameObject.GetComponent<PauseMenu>();

        // hide all item buttons
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
            choiceButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        }
    }

    void ItemButtons()
    {
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
                prefix = "* ";
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
