using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject saveOptionsUI;
    [SerializeField] Button playButton;
    [SerializeField] Button backButton;
    [SerializeField] Button deleteButton;

    PlayerControls controls;

    int deleteAttempts = 0;

    private void Start()
    {
        saveOptionsUI.SetActive(false);
        controls = new PlayerControls();
        controls.Enable();

        controls.UI.Cancel.performed += OnCancelPressed;
        backButton.onClick.AddListener(() => OnCancelPressed(new InputAction.CallbackContext()));
    }

    private void OnDisable()
    {
        controls.UI.Cancel.performed -= OnCancelPressed;
    }

    public void SelectCell(MainMenuCell cell)
    {
        saveOptionsUI.SetActive(true);

        foreach (MainMenuCell c in FindObjectsOfType<MainMenuCell>())
        {
            c.loadButton.enabled = false;
        }

        foreach (ColourOnHover c in saveOptionsUI.GetComponentsInChildren<ColourOnHover>()) {
            c.OnDeselected();
        }

        if (!EventSystem.current.alreadySelecting)
        {
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
            playButton.gameObject.GetComponent<ColourOnHover>().OnSelected();
        }

        // Reset buttons
        playButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();

        // this will reset the delete button
        deleteAttempts = 0;
        DeleteFile(null);

        // Update current slot
        SaveManager.SetSaveSlot(cell.saveSlot);

        if (SaveManager.SaveExists())
        {
            playButton.GetComponentInChildren<TextMeshProUGUI>().text = "[ LOAD ]";
            playButton.onClick.AddListener(() => TryLoadingScene(cell.saveSlot));
            deleteButton.onClick.AddListener(() => DeleteFile(cell));
            deleteButton.gameObject.SetActive(true);
        }
        else
        {
            playButton.GetComponentInChildren<TextMeshProUGUI>().text = "[ NEW ]";
            playButton.onClick.AddListener(() => StartNewGame());
            deleteButton.gameObject.SetActive(false);
        }


    }

    void DeleteFile(MainMenuCell cell)
    {
        TextMeshProUGUI deleteText = deleteButton.GetComponentInChildren<TextMeshProUGUI>();
        ColourOnHover colourOnHover = deleteButton.gameObject.GetComponentInChildren<ColourOnHover>();

        switch (deleteAttempts) {
            case 0:
                // Hardcoded cause im fucking tired
                colourOnHover.hoverTextColor = Color.black;
                colourOnHover.normalTextColor = Color.white;
                colourOnHover.OnDeselected();
                break;
            case 1:
                Color32 color = new Color32(128,0,0,255);
                colourOnHover.hoverTextColor = color;
                colourOnHover.normalTextColor = color;
                deleteText.color = color;
                break;
            case 2:
                Color32 color2 = new Color32(255,0,0,255);
                colourOnHover.hoverTextColor = color2;
                colourOnHover.normalTextColor = color2;
                deleteText.color = color2;

                break;
            case 3:

                deleteAttempts = 0;
                Debug.LogWarning("DELETED SAVE " + cell.saveSlot);
                SaveManager.Delete(cell.saveSlot);
                cell.UpdateInfo();
                OnCancelPressed(new InputAction.CallbackContext());

                break;
        }
        deleteAttempts++;
        
    }

    void StartNewGame()
    {
        Debug.LogWarning("STARTING NEW GAME!");
        LoadingScreen.GetInstance().LoadScene("TestLevel");
    }

    void TryLoadingScene(int saveSlot)
    {
        playButton.onClick.RemoveAllListeners();
        try
        {
            SceneLoader.GetInstance().LoadFromFile(saveSlot);
        }
        catch (Exception)
        {
            return;
        }

    }

    void OnCancelPressed(InputAction.CallbackContext context)
    {
        saveOptionsUI.SetActive(false);

        foreach (MainMenuCell c in FindObjectsOfType<MainMenuCell>())
        {
            c.loadButton.enabled = true;
            c.gameObject.GetComponent<MainMenuHighlight>().UnHighlight();
        }

        PopulateMainMenu populator = gameObject.GetComponentInChildren<PopulateMainMenu>();

        GameObject firstFile = populator.instantiatedCells.ToArray()[0];
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstFile);

        // this will reset the delete button
        deleteAttempts = 0;
        DeleteFile(null);

    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void LoadButton()
    {
        SceneLoader.GetInstance().LoadFromFile(SaveManager.GetSaveSlot());

        // stop rendering main menu
       gameObject.GetComponent<Canvas>().enabled = false;
    }

}
