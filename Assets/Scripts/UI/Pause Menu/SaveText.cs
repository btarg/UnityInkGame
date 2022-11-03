using UnityEngine;
using System;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SaveText : MonoBehaviour
{
    private void Awake() {

        SaveObject currentSave = SaveHelper.currentSaveObject();

        // Display timestamp or "never"
        string lastSave = "Never";
        if (currentSave != null)
        {
            lastSave = DateTime.FromFileTime(currentSave.timestamp).ToString();
        }
        this.GetComponent<TextMeshProUGUI>().text = "Last save:<br><color=yellow>" + lastSave + "</color>";
    }
}
