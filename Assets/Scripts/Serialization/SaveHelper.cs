using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class SaveHelper : MonoBehaviour
{

    public static SaveObject currentSaveObject()
    {
        int saveSlot = PlayerPrefs.GetInt("CURRENT_SAVE_SLOT", 0);
        SaveObject so = SaveManager.Load(saveSlot);

        return so;
    }

    public static void Save()
    {
        GameObject player;

        SaveObject so = new SaveObject();
        player = GameObject.FindGameObjectWithTag("Player");

        // Add ink json into the save object
        so = DialogueManager.GetInstance().dialogueVariables.SaveVariablesIntoObject(so);

        // Add name for easy access
        so.name = DialogueManager.GetInstance().dialogueVariables.GetFromInkGlobals("player_name").ToString();

        // Add timestamp
        so.timestamp = DateTime.UtcNow.ToFileTime();
        // Add current scene name
        so.currentScene = SceneManager.GetActiveScene().name;

        // Add player and camera's position and rotation
        so.playerPosition = player.transform.position;
        so.playerRotationEuler = player.transform.rotation.eulerAngles;
        so.cameraRotationEuler = player.gameObject.GetComponentInChildren<FirstPersonPlayer>().fpsCam.transform.rotation.eulerAngles;

        Debug.Log("Saved player position as: " + so.playerPosition);
        Debug.Log("Saved camera rotation as: " + so.cameraRotationEuler);

        // Start serialising to a file
        SaveManager.Save(so);
    }

}