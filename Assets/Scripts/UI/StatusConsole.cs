using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatusConsole : MonoBehaviour
{
    public static int visibleForSeconds = 4;
    static TextMeshProUGUI consoleText;
    static List<string> consoleBuffer = new List<string>();

    public static StatusConsole instance;

    public static void Clear() {
        consoleBuffer.Clear();
    }

    private void Awake() {
        instance = this;
    }

    public static void PrintToConsole(string message) {
        
        consoleBuffer.Insert(0, message);
        instance.StartCoroutine(RemoveLastMessage());
        
    }

    private static IEnumerator RemoveLastMessage() {
        yield return new WaitForSeconds(visibleForSeconds);
        consoleBuffer.RemoveAt(consoleBuffer.Count - 1);
    }

    private void Update() {

        if (consoleText == null) {
            consoleText = GameObject.FindGameObjectWithTag("StatusConsole").GetComponent<TextMeshProUGUI>();
        }

        consoleText.text = String.Join("\n", consoleBuffer);
    }
}
