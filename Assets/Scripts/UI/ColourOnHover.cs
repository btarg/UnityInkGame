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
        GetComponentInChildren<TextMeshProUGUI>().color = hoverTextColor;
    }
    public void OnDeselected()
    {
        GetComponent<Image>().color = normalColour;
        GetComponentInChildren<TextMeshProUGUI>().color = normalTextColor;
        selected--;
    }
}
