using UnityEngine;
using System.IO;
using System;
using System.Text;

public static class SaveManager
{
    public static string directory = "SaveData";
    public static string filename;
    static int saveSlot;

    // Used to update what slot we are on
    public static void SetSaveSlot(int slot) {
        saveSlot = slot;
        filename = "Save" + saveSlot.ToString();
        PlayerPrefs.SetInt("CURRENT_SAVE_SLOT", saveSlot);
    }

    public static int GetSaveSlot() {
        return PlayerPrefs.GetInt("CURRENT_SAVE_SLOT", 0);
    }

    public static void Delete(int slot) {

        // Get the full path name for this slot
        SetSaveSlot(slot);
        if (!SaveExists())
            return;

        // delete the save
        File.Delete(GetFullPath() + ".json");
        File.Delete(GetFullPath() + ".png");

    }

    public static void Save(SaveObject so)
    {
        if (!DirectoryExists())
            Directory.CreateDirectory(GetDirectory());

        // convert to json
        string json = JsonUtility.ToJson(so);

        // convert to bytes and save to file
        FileStream jsonFile = File.Create(GetFullPath() + ".json");
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        jsonFile.Write(bytes, 0, bytes.Length);
        jsonFile.Close();

        Debug.Log("Saved file to: " + GetFullPath());

    }

    public static SaveObject Load(int slot)
    {
        // We need to set this variable before loading so other functions get the right slot
        SetSaveSlot(slot);

        Debug.Log("Attempting to load save from slot " + slot.ToString());
        if (SaveExists())
        {
            try
            {
                // Load from JSON file
                string fileContents = File.ReadAllText(GetFullPath() + ".json");
                Debug.LogWarning(fileContents);
                SaveObject so = JsonUtility.FromJson<SaveObject>(fileContents);

                Debug.Log("Loaded save file from " + DateTime.FromFileTime(so.timestamp));

                return so;
            }
            catch (Exception)
            {
                Debug.LogWarning("Failed to load save file");
            }
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }
        return null;
    }
    public static bool SaveExists()
    {
        return File.Exists(GetFullPath() + ".json");
    }
    private static bool DirectoryExists()
    {
        return Directory.Exists(GetDirectory());
    }
    public static string GetFullPath()
    {
        return Path.Combine(GetDirectory(), filename);
    }
    public static string GetDirectory()
    {
        return Path.Combine(Application.persistentDataPath, directory);
    }
}
