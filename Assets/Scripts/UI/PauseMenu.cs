using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    SceneLoader loader;

    public static bool GameIsPaused = false;
    public static bool PauseMenuOpen = false;
    public static bool canPause = true;
    GameObject pauseMenuUI;
    GameObject playerObject;
    [SerializeField] GameObject firstButtonSelected;
    [SerializeField] TextMeshProUGUI saveDetailsText;

    public List<GameObject> objects;
    private Canvas disableCanvas;

    public UnityEvent onSaved;
    PlayerControls controls;

    KinematicPlayer kinematicPlayer;
    bool couldMove = true;

    SaveHelper helper;

    private void Awake() {
        helper = gameObject.AddComponent<SaveHelper>();
    }

    // Make sure the game isn't paused on startup
    void Start()
    {
        pauseMenuUI = getPauseMenuUI();
        pauseMenuUI.SetActive(false);
        disableCanvas = GameObject.Find("PlayerHUD").GetComponent<Canvas>();

        // Use new input system
        controls = new PlayerControls();
        controls.Enable();
        controls.UI.PauseMenu.performed += HandlePause;

        loader = SceneLoader.GetInstance();

        playerObject = GameObject.FindGameObjectWithTag("Player");
        kinematicPlayer = playerObject.GetComponent<KinematicPlayer>();

        Resume();
    }

    private void OnDisable() {
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

        disableCanvas = GameObject.Find("PlayerHUD").GetComponent<Canvas>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        kinematicPlayer = playerObject.GetComponent<KinematicPlayer>();

        pauseMenuUI = getPauseMenuUI();

        if (pauseMenuUI == null) {
            return;
        }

        if (GameIsPaused)
        {
            Debug.Log("Game Resumed");
            Resume();
        }
        else
        {
            Debug.Log("Game Paused");
            StopTime();
            OpenPauseMenu();
        }
    }

    public void Resume()
    {
        // Resume Game
        EventSystem.current.SetSelectedGameObject(null);

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        PauseMenuOpen = false;

        // Enable all objects in list
        EnableAll();

        kinematicPlayer.canMove = couldMove;
        disableCanvas.enabled = true;

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
        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonSelected);

        SaveObject currentSave = SaveHelper.currentSaveObject();

        // Display timestamp or "never"
        string lastSave = "Never";
        if (currentSave != null)
        {
            lastSave = DateTime.FromFileTime(currentSave.timestamp).ToString();
        }

        saveDetailsText.text = "Last save:<br><color=yellow>" + lastSave + "</color>";

        // Enable all buttons (we disable the save button after click to prevent spamming)
        foreach (Button b in pauseMenuUI.GetComponentsInChildren<Button>())
        {
            b.enabled = true;
        }
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;

        couldMove = kinematicPlayer.canMove;
        kinematicPlayer.canMove = false;
        disableCanvas.enabled = false;

        // Disable all objects in list
        DisableAll();
    }

    public void EnableAll()
    {
        foreach (var obj in objects)
            obj.SetActive(true);
    }

    public void DisableAll()
    {
        foreach (var obj in objects)
            obj.SetActive(false);
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