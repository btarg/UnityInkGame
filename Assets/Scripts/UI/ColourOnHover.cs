using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ColourOnHover : MonoBehaviour
{
    public Color normalColour;
    public Color hoverColour;
    public Color normalTextColor;
    public Color hoverTextColor;
    public bool showPrefix = false;
    string selectedPrefix = "<sprite name=\"point-hand\"> ";
    int selected = 1;

    private void Start() {
        OnDeselected();
    }

    private void Awake() {
        selected = 1;
        OnDeselected();
    }

    public void OnSelected()
    {
        if (selected > 0)
            return;

        selected++;

        GetComponent<Image>().color = hoverColour;
        TextMeshProUGUI tmpro = GetComponentInChildren<TextMeshProUGUI>();
        tmpro.color = hoverTextColor;

        if (!tmpro.text.StartsWith(selectedPrefix) && showPrefix) {
            tmpro.text = selectedPrefix + tmpro.text;
        }
    }
    public void OnDeselected()
    {
        TextMeshProUGUI tmpro = GetComponentInChildren<TextMeshProUGUI>();
        GetComponent<Image>().color = normalColour;

        if (tmpro.text.StartsWith(selectedPrefix) && showPrefix) {
            tmpro.text = tmpro.text.Replace(selectedPrefix, "");
        }
        GetComponentInChildren<TextMeshProUGUI>().color = normalTextColor;
        selected--;
    }
}
