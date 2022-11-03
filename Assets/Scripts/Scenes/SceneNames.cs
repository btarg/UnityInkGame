using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNames
{
    private static Dictionary<string, string> sceneDictionary;

    private static void Init()
    {
        sceneDictionary = new Dictionary<string, string>();
        sceneDictionary.Add("BSPTest", "Hub World");
        sceneDictionary.Add("TestLevel", "Dialog system testing");

    }
    
    public static string GetSceneName(string sceneName) {

        Init();
        
        string name = sceneName;
        sceneDictionary.TryGetValue(sceneName, out name);

        return name;
    }

}
