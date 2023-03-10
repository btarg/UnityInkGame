using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveHelper : MonoBehaviour
{
    public static SaveObject currentSaveObject()
    {
        return SaveManager.Load(SaveManager.GetSaveSlot());
    }
    public static Texture2D currentSaveScreenshot()
    {
        return getSaveScreenshot(SaveManager.GetSaveSlot());
    }

    public static Texture2D getSaveScreenshot(int slot)
    {
        try
        {
            SaveManager.SetSaveSlot(slot);
            string path = SaveManager.GetFullPath() + ".png";
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(bytes);
            return tex;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Screenshot not found: " + ex);
            return null;
        }
    }

    public void Save(Action<bool> callback)
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
        so.playerPosition = player.GetComponentInChildren<ExampleCharacterController>().gameObject.transform.position;
        so.playerRotationEuler = player.GetComponentInChildren<ExampleCharacterController>().gameObject.transform.rotation.eulerAngles;
        so.cameraRotationEuler = player.gameObject.GetComponentInChildren<FirstPersonPlayer>().fpsCam.transform.rotation.eulerAngles;
        Debug.Log("Saved player position as: " + so.playerPosition);
        Debug.Log("Saved camera rotation as: " + so.cameraRotationEuler);

        InventoryObject inventoryObject = player.GetComponentInChildren<InventoryObject>();

        // Add inventory
        so.inventory = inventoryObject.Container;
        so.equippedSlot = inventoryObject.getEquippedSlot();
        so.pickedUpItems = inventoryObject.pickedUpItems;

        StartCoroutine(SaveScreenshotToSaveObject(so, callback));
    }

    private IEnumerator SaveScreenshotToSaveObject(SaveObject so, Action<bool> callback)
    {
        List<Canvas> toReenable = new List<Canvas>();

        foreach (Canvas c in FindObjectsOfType<Canvas>())
        {
            if (c.enabled)
            {
                c.enabled = false;
                toReenable.Add(c);
            }

        }
        yield return new WaitForEndOfFrame();

        // Save with the same filename as save data
        ScreenCapture.CaptureScreenshot(SaveManager.GetFullPath() + ".png");

        toReenable.ForEach((Canvas c) => c.enabled = true);

        // Start serialising to a file
        SaveManager.Save(so);

        callback(true);
    }

}