﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

public class InteractiveObject : MonoBehaviour
{
    public enum InteractionType { Generic, Talk, Pickup };

    public InteractionType interactionType = InteractionType.Generic;
    public UnityEvent onPlayerInteract;
    public bool canInteract = true;
    public bool hasCooldown = false;
    public float cooldownTime = 1f;

    public string interactionDescription = "Interact";

    [Header("Character Properties")]
    [SerializeField] private CharacterScriptableObject character;

    private void Start()
    {
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

            if (itemPickup.amount > 1) {
                colouredName += String.Format(" (x{0})", itemPickup.amount.ToString());
            }
            
            string interactMessage = "Look at ";
            switch (interactionType)
            {
                case InteractiveObject.InteractionType.Pickup:
                    if (itemPickup.canPickup)
                    {
                        interactMessage = "Take ";
                    }
                    break;
            }

            interactionDescription = interactMessage + colouredName;

        }
    }


    public void PlayerInteract()
    {
        onPlayerInteract.Invoke();

        if (hasCooldown && canInteract)
            StartCoroutine(CooldownTimer());
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
