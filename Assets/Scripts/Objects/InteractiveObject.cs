using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

public class InteractiveObject : MonoBehaviour
{
    public enum InteractionType { Generic, Talk, Pickup, GoTo, ItemRequired };

    [Header("Interaction Properties")]
    public InteractionType interactionType = InteractionType.Generic;
    public UnityEvent onPlayerInteract;
    public bool canInteract = true;
    public bool hasCooldown = false;
    public float cooldownTime = 1f;

    [Header("Equipped Item Requirement")]
    public InventoryItem requiredItem = null;
    public int requiredItemAmount = 1;
    public bool takeRequiredItemFromPlayer = true;
    private bool requirementMet = false;
    InventoryObject inventory;

    public string interactionDescription = "Interact";

    [Header("Character Properties")]
    [SerializeField] private CharacterScriptableObject character;

    private void Start()
    {
        UpdateInteractMessage();
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<InventoryObject>();
    }

    private void Update()
    {

        if (requiredItem == null)
            return;

        if (inventory == null)
            inventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<InventoryObject>();

        // Check if the equipped item is the correct type and amount
        InventorySlot equipped = inventory.getEquippedSlot();
        if (equipped == null)
            return;

        if (equipped.item == requiredItem && equipped.amount >= requiredItemAmount) {
            requirementMet = true;
        } else {
            requirementMet = false;
        }

        UpdateInteractMessage();

    }

    private void UpdateInteractMessage()
    {

        if (requiredItem != null)
        {
            
            // Now check if the requirement was met
            if (!requirementMet) {

                interactionType = InteractionType.ItemRequired;
                interactionDescription = String.Format("<color=red>Requires {0} (x{1})</color>", requiredItem.displayName, requiredItemAmount);
                // Don't do the normal interaction stuff
                return;

            }

        }

        if (character != null)
        {
            string characterColor = ColorUtility.ToHtmlStringRGB(character.characterColor);
            string colouredName = String.Format("<color=#{0}>{1}</color>", characterColor, character.characterName);
            string interactMessage = "";
            switch (interactionType)
            {
                case InteractiveObject.InteractionType.Talk:
                    interactMessage = "Speak to ";
                    break;
                case InteractiveObject.InteractionType.Generic:
                    interactMessage = "Interact with ";
                    break;
            }

            interactionDescription = interactMessage + colouredName;
        }

        ItemPickup itemPickup = gameObject.GetComponent<ItemPickup>();

        if (itemPickup != null)
        {

            string colouredName = String.Format("<color=yellow>{0}</color>", itemPickup.item.displayName);

            colouredName += String.Format(" (x{0})", itemPickup.amount.ToString());

            interactionType = InteractiveObject.InteractionType.Generic;
            string interactMessage = "Look at ";
            if (itemPickup.canPickup)
            {
                interactionType = InteractiveObject.InteractionType.Pickup;
                interactMessage = "Take ";
            }

            interactionDescription = interactMessage + colouredName;

        }

    }


    public void PlayerInteract()
    {
        if (requiredItem != null && !requirementMet) {
            StatusConsole.PrintToConsole("You do not have the required item equipped.");
            return;
        }

        if (canInteract)
        {
            onPlayerInteract.Invoke();

            if (requiredItem != null && requirementMet) {
                // Take the item from the player if necessary
                if (takeRequiredItemFromPlayer)
                    inventory.RemoveItem(requiredItem, requiredItemAmount);
            }

        }
        if (hasCooldown)
            StartCoroutine(CooldownTimer());

        UpdateInteractMessage();
    }

    IEnumerator CooldownTimer()
    {
        canInteract = false;

        // do not activate cooldown until there is no dialogue playing
        yield return new WaitForEndOfFrame();
        yield return new WaitWhile(() => DialogueManager.GetInstance().dialogueIsPlaying);

        yield return new WaitForSeconds(cooldownTime);
        canInteract = true;
    }
}
