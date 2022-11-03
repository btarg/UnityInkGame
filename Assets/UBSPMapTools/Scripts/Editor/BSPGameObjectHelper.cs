using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPGameObjectHelper
{
    const string BSPTag = "BSPGenerated";

    public static GameObject CreateTaggedGameObject(string name) {
        GameObject go = new GameObject(name);
        go.tag = BSPTag;
        return go;
    }

    public static void RemoveAllTaggedObjects() {
        GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach(GameObject go in allObjects) {

            // Delete all generated assets including player prefab
            if (go.tag == BSPTag || go.GetComponent<UBSPEntities.UBSPPlayer>()) {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }
    }
}
