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
    [SerializeField] Sprite genericInteractionCrosshair;
    [SerializeField] Sprite talkInteractionCrosshair;
    [SerializeField] Sprite pickupInteractionCrosshair;
    [SerializeField] Sprite goToInteractionCrosshair;
    [SerializeField] Sprite itemRequiredInteractionCrosshair;

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

    private void Awake() {
        if (interactPrompt == null) {
            interactPrompt = GameObject.FindGameObjectWithTag("InteractPrompt");
        }
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

            if (interactive != null && interactive.CanInteract)
            {
                interactPrompt.SetActive(true);
                switch (interactive.interactionType)
                {
                    case InteractiveObject.InteractionType.Generic:
                        interactPrompt.GetComponentInChildren<Image>().sprite = genericInteractionCrosshair;
                        break;
                    case InteractiveObject.InteractionType.Talk:
                        interactPrompt.GetComponentInChildren<Image>().sprite = talkInteractionCrosshair;
                        break;
                    case InteractiveObject.InteractionType.Pickup:
                        interactPrompt.GetComponentInChildren<Image>().sprite = pickupInteractionCrosshair;
                        break;
                    case InteractiveObject.InteractionType.GoTo:
                        interactPrompt.GetComponentInChildren<Image>().sprite = goToInteractionCrosshair;
                        break;

                     case InteractiveObject.InteractionType.ItemRequired:
                        interactPrompt.GetComponentInChildren<Image>().sprite = itemRequiredInteractionCrosshair;
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
