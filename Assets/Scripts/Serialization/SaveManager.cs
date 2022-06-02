using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static string directory = "SaveData";
    public static string filename;

    private static BinaryFormatter bf;

    public static void Save(SaveObject so)
    {
        int saveSlot = PlayerPrefs.GetInt("CURRENT_SAVE_SLOT", 0);
        filename = "Save" + saveSlot.ToString() + ".json";

        if (!DirectoryExists())
            Directory.CreateDirectory(Application.persistentDataPath + "/" + directory);

        // convert to json
        string json = JsonUtility.ToJson(so);

        // convert to bytes and save to file
        FileStream jsonFile = File.Create(GetFullPath());
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        jsonFile.Write(bytes, 0, bytes.Length);
        jsonFile.Close();

        Debug.Log("Saved file to: " + GetFullPath());

    }

    public static SaveObject Load(int saveSlot)
    {
        filename = "Save" + saveSlot.ToString() + ".json";

        Debug.Log("Attempting to load save from slot " + saveSlot.ToString());
        if (SaveExists())
        {
            try
            {
                // Load from JSON file
                string fileContents = File.ReadAllText(GetFullPath());
                SaveObject so = JsonUtility.FromJson<SaveObject>(fileContents);

                Debug.Log("Loaded save file from " + DateTime.FromFileTime(so.timestamp));

                return so;
            }
            catch (Exception)
            {
                Debug.Log("Failed to load save file");
            }
        }
        else
        {
            Debug.Log("Save file not found!");
        }
        return null;
    }
    private static bool SaveExists()
    {
        return File.Exists(GetFullPath());
    }
    private static bool DirectoryExists()
    {
        return Directory.Exists(Application.persistentDataPath + "/" + directory);
    }
    private static string GetFullPath()
    {
        return Application.persistentDataPath + "/" + directory + "/" + filename;
    }
}
