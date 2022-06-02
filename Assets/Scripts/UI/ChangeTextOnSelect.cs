using TMPro;
using UnityEngine;

public class ChangeTextOnSelect : MonoBehaviour
{
    string previousText;
    int selected = 0;

    private void Awake() {
        previousText = GetComponentInChildren<TextMeshProUGUI>().text;
        GetComponentInChildren<TextMeshProUGUI>().text = "<color=#000000>* </color>" + previousText;
    }

    public void OnSelected() {

        if (selected > 0)
            return;

        previousText = GetComponentInChildren<TextMeshProUGUI>().text;
        selected++;

        GetComponentInChildren<TextMeshProUGUI>().text = GetComponentInChildren<TextMeshProUGUI>().text.Replace("<color=#000000>* </color>", "* ");
    }

    public void OnDeselected() {
        GetComponentInChildren<TextMeshProUGUI>().text = GetComponentInChildren<TextMeshProUGUI>().text.Replace("* ", "<color=#000000>* </color>");
        selected--;
    }
}
