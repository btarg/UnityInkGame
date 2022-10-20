
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

    KinematicPlayer kinematicPlayer;
    bool couldMove = true;

    SaveHelper helper;

    // Make sure the game isn't paused on startup
    void Start()
    {
        helper = gameObject.AddComponent<SaveHelper>();
        // pauseMenuUI = getPauseMenuUI();
        // pauseMenuUI.SetActive(false);

        disableCanvas = GameObject.Find("PlayerHUD").GetComponent<Canvas>();
    
        loader = SceneLoader.GetInstance();

        playerObject = GameObject.FindGameObjectWithTag("Player");
        kinematicPlayer = playerObject.GetComponent<KinematicPlayer>();

        OnEnable();
        onUnpause.Invoke();
    }

    private void OnEnable() {

        // Use new input system
        if (controls == null)
            controls = new PlayerControls();

        controls.Enable();
        controls.UI.PauseMenu.performed += HandlePause;
    }
    private void OnDisable() {
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

    public void PauseGame() {
        StopTime();
        OpenPauseMenu();
        Debug.Log("Game Paused");
    }

    public void Resume()
    {
        disableCanvas = GameObject.Find("PlayerHUD").GetComponent<Canvas>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        kinematicPlayer = playerObject.GetComponent<KinematicPlayer>();

        // Resume Game
        EventSystem.current.SetSelectedGameObject(null);

        // pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
        PauseMenuOpen = false;

        if (kinematicPlayer != null)
            kinematicPlayer.canMove = couldMove;

        disableCanvas.enabled = true;
        Debug.Log("Game Resumed");

    }

    public GameObject getPauseMenuUI()
    {
        // stupid hacky shit because GetChild sucks
        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.CompareTag("PauseMenu"))
            {
                return child.gameObject;
            }
        }
        return null;
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
        playerObject = GameObject.FindGameObjectWithTag("Player");
        kinematicPlayer = playerObject.GetComponent<KinematicPlayer>();

        Time.timeScale = 0f;
        GameIsPaused = true;

        couldMove = kinematicPlayer.canMove;
        kinematicPlayer.canMove = false;
        disableCanvas.enabled = false;
    }

    public void MenuButton()
    {
        helper.Save();
        SceneManager.LoadScene("MainMenu");

    }

    public void LoadButton()
    {
        loader.QuickLoad();
        Resume();
    }

    public void SaveButton()
    {
        helper.Save();

        Resume();
        onSaved.Invoke();

    }

}