
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    SceneLoader loader;

    public static bool GameIsPaused = false;
    public static bool PauseMenuOpen = false;
    public static bool canPause = true;

    public UnityEvent onPause;
    public UnityEvent onUnpause;

    GameObject playerObject;
    [SerializeField] GameObject firstButtonSelected;
    [SerializeField] TextMeshProUGUI saveDetailsText;

    private Canvas disableCanvas;

    public UnityEvent onSaved;
    PlayerControls controls;

    ExamplePlayer kinematicPlayer;
    bool couldMove = true;

    SaveHelper helper;

    [SerializeField] private TextAsset saveTextJSON;
    [SerializeField] private TextAsset hubChoiceJSON;

    // Make sure the game isn't paused on startup
    void Start()
    {
        helper = gameObject.AddComponent<SaveHelper>();


        disableCanvas = GameObject.Find("PlayerHUD").GetComponent<Canvas>();

        loader = SceneLoader.GetInstance();

        playerObject = GameObject.FindGameObjectWithTag("Player");
        kinematicPlayer = playerObject.GetComponentInChildren<ExamplePlayer>();


        OnEnable();
        onUnpause.Invoke();
    }

    private void OnEnable()
    {

        // Use new input system
        if (controls == null)
            controls = new PlayerControls();

        controls.Enable();
        controls.UI.PauseMenu.performed += HandlePause;
    }
    private void OnDisable()
    {
        controls.Disable();
        controls.UI.PauseMenu.performed -= HandlePause;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameIsPaused || DialogueManager.GetInstance().dialogueIsPlaying)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void HandlePause(InputAction.CallbackContext value)
    {
        if (!canPause || DialogueManager.GetInstance().dialogueIsPlaying || InventoryUIManager.inventoryIsOpen)
            return;

        if (GameIsPaused)
        {
            onUnpause.Invoke();
        }
        else
        {
            onPause.Invoke();
        }
    }

    public void PauseGame()
    {
        StopTime();
        OpenPauseMenu();
        Debug.Log("Game Paused");
    }

    public void Resume()
    {
        disableCanvas = GameObject.Find("PlayerHUD").GetComponent<Canvas>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        kinematicPlayer = playerObject.GetComponentInChildren<ExamplePlayer>();

        // Resume Game
        EventSystem.current.SetSelectedGameObject(null);

        Time.timeScale = 1f;
        GameIsPaused = false;
        PauseMenuOpen = false;

        if (kinematicPlayer != null)
            kinematicPlayer.canMove = couldMove;

        disableCanvas.enabled = true;
        Debug.Log("Game Resumed");

    }

    public void OpenPauseMenu()
    {
        PauseMenuOpen = true;

        // Pause Game
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonSelected);
    }

    public void StopTime()
    {
        disableCanvas = GameObject.Find("PlayerHUD").GetComponent<Canvas>();

        Time.timeScale = 0f;
        FreezePlayer();

        disableCanvas.enabled = false;
    }

    public void FreezePlayer()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        kinematicPlayer = playerObject.GetComponentInChildren<ExamplePlayer>();

        couldMove = kinematicPlayer.canMove;
        kinematicPlayer.canMove = false;
        GameIsPaused = true;

    }

    public void MenuButton()
    {
        helper.Save(MenuButtonCallback);
    }

    void MenuButtonCallback(bool b)
    {
        LoadingScreen.GetInstance().LoadScene("MainMenu", false);
    }

    public void HubButton()
    {
        onUnpause.Invoke();
        if (!DialogueManager.GetInstance().dialogueIsPlaying)
        {
            DialogueManager.GetInstance().EnterDialogueMode(hubChoiceJSON);
        }
    }

    public void LoadButton()
    {
        loader.QuickLoad();
        onUnpause.Invoke();

    }

    public void SaveButton()
    {
        helper.Save(SaveButtonCallback);
    }

    void SaveButtonCallback(bool b)
    {
        onUnpause.Invoke();
        onSaved.Invoke();

        if (!DialogueManager.GetInstance().dialogueIsPlaying)
            DialogueManager.GetInstance().EnterDialogueMode(saveTextJSON);
    }


}