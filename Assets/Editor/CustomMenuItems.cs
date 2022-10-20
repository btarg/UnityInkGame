using System;
using UnityEditor;
using UnityEngine;

public class CustomMenuItems : MonoBehaviour
{

    [MenuItem("Custom/Open Save Data folder", false, 100)]
    public static void openSaveFolder()
    {
        System.Diagnostics.Process.Start("explorer", "C:\\Users\\BenTa\\AppData\\LocalLow\\DefaultCompany\\Dialogue\\SaveData");
    }

}
