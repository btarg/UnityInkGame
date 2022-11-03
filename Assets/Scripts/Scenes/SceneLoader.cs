using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneLoader : MonoBehaviour
{
    GameObject player;
    SaveObject currentSave;

    private static SceneLoader instance;
    public UnityEvent onSceneLoaded;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Scene Loader in the scene");
        }
        instance = this;

        currentSave = SaveHelper.currentSaveObject();
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    public static SceneLoader GetInstance()
    {
        return instance;
    }

    public void QuickLoad()
    {
        int saveSlot = SaveManager.GetSaveSlot();
        LoadFromFile(saveSlot, false);
    }

    public void LoadFromFile(int saveSlot, bool fade=true)
    {
        currentSave = SaveManager.Load(saveSlot);

        if (currentSave == null)
        {
            return;
        }

        string sceneName = currentSave.currentScene;
        Debug.Log("Loading Saved Scene: " + sceneName);

        LoadingScreen.GetInstance().LoadScene(sceneName, fade);

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Make sure we only run this stuff when loading a playable scene
        if (!SaveManager.SaveExists() || scene.name == "MainMenu")
            return;

        // Get the player already in the scene
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null) {
            return;
        }


        Vector3 savedPlayerPosition = currentSave.playerPosition;

        // Convert rotation from euler (vector3)
        Quaternion savedRotation = Quaternion.Euler(currentSave.playerRotationEuler);
        Quaternion savedCameraRotation = Quaternion.Euler(currentSave.cameraRotationEuler);

        Debug.Log("Saved position is " + savedPlayerPosition);
        Debug.Log("Saved rotation is " + savedRotation);
        Debug.Log("Saved cam rotation is " + savedCameraRotation);

        Camera playerCamera = player.gameObject.GetComponentInChildren<FirstPersonPlayer>().fpsCam;

        // Move player and rotate camera
        player.transform.SetPositionAndRotation(savedPlayerPosition, player.transform.rotation);
        playerCamera.transform.SetPositionAndRotation(playerCamera.transform.position, savedCameraRotation);

        InventoryObject inventoryObject = player.GetComponentInChildren<InventoryObject>();

        // Load the full inventory and the equipped item slot
        inventoryObject.Load();

        StatusConsole.Clear();
        onSceneLoaded.Invoke();

    }

}

