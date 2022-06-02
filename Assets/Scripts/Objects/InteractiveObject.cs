using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

public class InteractiveObject : MonoBehaviour
{
    public enum InteractionType {Generic, Talk};

    public InteractionType interactionType = InteractionType.Generic;
    public UnityEvent onPlayerInteract;
    public bool canInteract = true;
    public bool hasCooldown = false;
    public float cooldownTime = 1f;

    public string interactionDescription = "Interact";

    [Header("Character Properties")]
    [SerializeField] private string characterScriptableObjectName;

    private void Start()
    {
        CharacterScriptableObject character = DialogueManager.GetInstance().GetCharacterByName(characterScriptableObjectName);

        if (character != null)
        {
            InteractiveObject io = gameObject.GetComponent<InteractiveObject>();
            string characterColor = ColorUtility.ToHtmlStringRGB(character.characterColor);
            string colouredName = String.Format("<color=#{0}>{1}</color>", characterColor, character.characterName);
            string interactMessage = "";
            switch (io.interactionType)
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
    }


    public void PlayerInteract()
    {
        onPlayerInteract.Invoke();

        if (hasCooldown && canInteract)
            StartCoroutine(CooldownTimer());
    }

    IEnumerator CooldownTimer() {
        canInteract = false;
        yield return new WaitForSeconds(cooldownTime);
        canInteract = true;
    }
}
