using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeedDefault = 0.04f;
    private float typingSpeed = 0.04f;

    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject choicesUIElement;
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Input UI")]
    [SerializeField] private GameObject inputUIElement;

    [Header("Characters")]
    [SerializeField] private CharacterScriptableObject[] characterScriptableObjects;

    // Sound manager should be private as it is added by the script
    private SoundManager soundManager;

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }
    private CharacterScriptableObject currentCharacter;

    private bool canContinueToNextLine = false;
    private bool waitingForInput = false;
    private bool submitted = false;

    private Coroutine displayLineCoroutine;

    private static DialogueManager instance;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string SPEED_TAG = "delay";
    private const string AUTO_TAG = "continueafter";
    private const string INPUT_TAG = "input";

    public DialogueVariables dialogueVariables;

    PlayerControls playerControls;

    bool skipTyping = false;
    float autoContinue = float.PositiveInfinity;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        soundManager = gameObject.AddComponent<SoundManager>();

        playerControls = new PlayerControls();
        playerControls.Enable();

        playerControls.FirstPerson.Interact.performed += OnSubmitPressed;
        playerControls.FirstPerson.Jump.performed += OnSubmitPressed;
        playerControls.UI.PauseMenu.performed += OnSubmitPressed;
        playerControls.UI.Submit.performed += OnSubmitPressed;
        playerControls.InputBox.Navigate.performed += OnNavigatePressed;

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        choicesUIElement.SetActive(false);

        // get all of the choices text 
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    void OnNavigatePressed(InputAction.CallbackContext context)
    {
        // We are pressing down to get to the submit button the input screen
        if (waitingForInput) {
            SelectSubmitButton(null);
        }
    }
    
    void OnSubmitPressed(InputAction.CallbackContext context)
    {
        skipTyping = true;
    }

    private void Update()
    {
        // return right away if dialogue isn't playing of if the game is paused
        if (!dialogueIsPlaying || Time.timeScale == 0f || waitingForInput)
        {
            return;
        }

        // handle continuing to the next line in the dialogue when submit is pressed
        if (canContinueToNextLine
            && currentStory.currentChoices.Count == 0
            && (playerControls.FirstPerson.Interact.WasPressedThisFrame()
            || playerControls.FirstPerson.Jump.WasPressedThisFrame()
            || playerControls.UI.PauseMenu.WasPressedThisFrame()
            || playerControls.UI.Submit.WasPressedThisFrame()) || autoContinue != float.PositiveInfinity && canContinueToNextLine)
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(currentStory);

        // reset portrait, layout, and speaker
        displayNameText.text = "???";
        portraitAnimator.Play("default");

        // add functions to ink
        currentStory.BindExternalFunction("playSound", (string name) =>
        {
            soundManager.Play(name);
        });
        // currentStory.BindExternalFunction ("playSound", (string name, float volume, float pitch, float delay) => {
        //     soundManager.Play(name);
        // });

        ContinueStory();
    }

    private void SubmitInput()
    {
        submitted = true;
    }

    private void SelectSubmitButton(string unused)
    {
        StartCoroutine(SelectSubmitButtonCoroutine());
    }
    IEnumerator SelectSubmitButtonCoroutine()
    {
        GameObject submitButton = inputUIElement.GetComponentInChildren<Button>().gameObject;

        if (EventSystem.current.currentSelectedGameObject != submitButton)
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(submitButton);
        }

    }

    private void GetPlayerInput(string outVar)
    {
        StartCoroutine(WaitForInput(outVar));
    }

    private IEnumerator WaitForInput(string outVar)
    {
        // wait until the dialog is finished before opening the input box
        waitingForInput = true;
        yield return new WaitUntil(() => canContinueToNextLine == true);
        inputUIElement.SetActive(true);

        TMP_InputField inputField = inputUIElement.GetComponentInChildren<TMP_InputField>();

        // select input box
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(inputUIElement);
        inputField.ActivateInputField();

        // listen for moving away from the box
        inputField.onSubmit.AddListener(SelectSubmitButton);

        // make the button able to submit the input 
        inputUIElement.GetComponentInChildren<Button>().onClick.AddListener(SubmitInput);

        // wait until we enter an input (we do this because we can only get outVar here)
        yield return new WaitUntil(() => submitted == true);

        // set the ink variable
        currentStory.variablesState[outVar] = inputField.text;

        inputUIElement.SetActive(false);
        submitted = false;
        waitingForInput = false;

        ContinueStory();

    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        typingSpeed = typingSpeedDefault;
        autoContinue = float.PositiveInfinity;
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            // set text for the current dialogue line
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));
            // handle tags
            HandleTags(currentStory.currentTags);
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        // set the text to the full line, but set the visible characters to 0
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        // hide items while text is typing
        continueIcon.SetActive(false);
        HideChoices();
        inputUIElement.SetActive(false);

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;
        skipTyping = false;

        // display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            // if the submit button is pressed, finish up displaying the line right away
            if (skipTyping && autoContinue == float.PositiveInfinity)
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }

            // check for rich text tag, if found, add it without waiting
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            // if not rich text, add the next letter and wait a small time
            else
            {
                yield return new WaitForSeconds(typingSpeed);
                dialogueText.maxVisibleCharacters++;
                PlayTypingSound();
            }
        }


        // if we are in auto mode, then wait a bit before moving on
        StartCoroutine(ContinueToNextLine());
    }

    private void PlayTypingSound()
    {
        if (currentCharacter != null && currentCharacter.characterTypingSounds.Length > 0)
            soundManager.PlayRandomClip(currentCharacter.characterTypingSounds);
    }

    IEnumerator ContinueToNextLine()
    {
        // actions to take after the entire line has finished displaying
        DisplayChoices();
        if (autoContinue != float.PositiveInfinity)
        {
            yield return new WaitForSeconds(autoContinue);
        }
        else if (!waitingForInput)
        {
            continueIcon.SetActive(true);
        }
        canContinueToNextLine = true;

    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
        choicesUIElement.SetActive(false);
    }

    public CharacterScriptableObject GetCharacterByName(string name)
    {
        foreach (CharacterScriptableObject character in characterScriptableObjects)
        {
            if (character.name == name)
            {
                return character;
            }
        }
        return null;
    }

    private void HandleTags(List<string> currentTags)
    {
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // handle the tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    // Use scriptable object to get character details
                    currentCharacter = GetCharacterByName(tagValue);

                    if (currentCharacter != null)
                    {
                        displayNameText.text = currentCharacter.characterName;
                    }
                    else
                    {
                        displayNameText.text = "???";
                    }

                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case SPEED_TAG:
                    // NEW typing speed tag
                    typingSpeed = float.Parse(tagValue);
                    break;
                case AUTO_TAG:
                    autoContinue = float.Parse(tagValue);
                    break;
                case INPUT_TAG:
                    GetPlayerInput(tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count < 1)
        {
            return;
        }

        choicesUIElement.SetActive(true);

        // defensive check to make sure our UI can support the number of choices coming in
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                + currentChoices.Count);
        }

        int index = 0;
        // enable and initialize the choices up to the amount of choices for this line of dialogue
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        // go through the remaining choices the UI supports and make sure they're hidden
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        // Event System requires we clear it first, then wait
        // for at least one frame before we set the current selected object.
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            // NOTE: The below two lines were added to fix a bug after the Youtube video was made
            ContinueStory();
        }
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }

}
