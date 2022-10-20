using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class FirstPersonPlayer : MonoBehaviour
{
    public float interactRange = 4f;
    public Camera fpsCam;
    [SerializeField] GameObject interactPrompt;

    [Header("Texture2Ds")]
    [SerializeField] Texture2D genericInteractionCrosshair;
    [SerializeField] Texture2D talkInteractionCrosshair;
    [SerializeField] Texture2D pickupInteractionCrosshair;
    [SerializeField] Texture2D goToInteractionCrosshair;
    [SerializeField] Texture2D itemRequiredInteractionCrosshair;

    PlayerControls controls;
    bool isPressingInteract;
    PlayerInput playerInput;

    // Start is called before the first frame update
    void Start()
    {
        fpsCam = GetComponent<Camera>();
        controls = new PlayerControls();
        controls.Enable();

        controls.FirstPerson.Interact.performed += OnInteract;
        controls.FirstPerson.Interact.canceled += OnInteractCancel;

    }


    void OnInteract(InputAction.CallbackContext context)
    {
        isPressingInteract = true;
    }
    void OnInteractCancel(InputAction.CallbackContext context)
    {
        isPressingInteract = false;
    }


    // Update is called once per frame
    void Update()
    {
    
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, interactRange)
        && Time.timeScale != 0
        && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            Transform objectHit = hit.transform;
            InteractiveObject interactive = objectHit.gameObject.GetComponent<InteractiveObject>();

            if (interactive != null && interactive.canInteract)
            {
                interactPrompt.SetActive(true);

                switch (interactive.interactionType)
                {
                    case InteractiveObject.InteractionType.Generic:
                        interactPrompt.GetComponentInChildren<RawImage>().texture = genericInteractionCrosshair;
                        break;
                    case InteractiveObject.InteractionType.Talk:
                        interactPrompt.GetComponentInChildren<RawImage>().texture = talkInteractionCrosshair;
                        break;
                    case InteractiveObject.InteractionType.Pickup:
                        interactPrompt.GetComponentInChildren<RawImage>().texture = pickupInteractionCrosshair;
                        break;
                    case InteractiveObject.InteractionType.GoTo:
                        interactPrompt.GetComponentInChildren<RawImage>().texture = goToInteractionCrosshair;
                        break;

                     case InteractiveObject.InteractionType.ItemRequired:
                        interactPrompt.GetComponentInChildren<RawImage>().texture = itemRequiredInteractionCrosshair;
                        break;   
                }

                string interactString = "[" + controls.FirstPerson.Interact.GetBindingDisplayString() + "] <br>";
                interactString += interactive.interactionDescription;

                interactPrompt.GetComponentInChildren<TextMeshProUGUI>().text = interactString;

                if (isPressingInteract)
                {
                    isPressingInteract = false;
                    interactive.PlayerInteract();
                }
            }

        }
        else
        {
            interactPrompt.SetActive(false);
        }

    }
}
