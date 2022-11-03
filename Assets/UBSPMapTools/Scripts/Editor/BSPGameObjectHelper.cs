using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPGameObjectHelper
{
    public static string BSPTag = "BSPGenerated";

    public static GameObject CreateTaggedGameObject(string name) {
        GameObject go = new GameObject(name);
        go.tag = BSPTag;
        return go;
    }

    public static GameObject[] GetAllTaggedObjects() {
        List<GameObject> output = new List<GameObject>();
        
        GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(GameObject go in allObjects) {

            // Delete all generated assets including player prefab
            if (go.tag == BSPTag || go.GetComponent<UBSPEntities.UBSPPlayer>()) {
                output.Add(go);
            }
        }

        return output.ToArray();
    }

    public static void RemoveAllTaggedObjects() {
        GameObject[] allObjects = GetAllTaggedObjects();

        foreach(GameObject go in allObjects) {
            UnityEngine.Object.DestroyImmediate(go);
        }
    }
}
