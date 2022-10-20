using UnityEngine;
using UnityEngine.UI;

public class MainMenuHighlight : MonoBehaviour
{
    public RawImage[] images;
    bool highlighted = false;

    private void Start()
    {
        OnDeselected();
    }

    public void OnDeselected()
    {

        if (highlighted)
        {
            return;
        }

        foreach (RawImage img in images)
        {
            img.color = new Color32(128, 128, 128, 255);
        }
    }

    public void OnSelected()
    {
        if (highlighted)
        {
            return;
        }

        foreach (RawImage img in images)
        {
            img.color = Color.white;
        }
    }

    public void OnHighlight()
    {
        highlighted = true;
        foreach (MainMenuHighlight others in FindObjectsOfType<MainMenuHighlight>())
        {
            others.OnDeselected();
            others.highlighted = true;
        }

        foreach (RawImage img in images)
        {
            img.color = Color.yellow;
        }
    }

    public void UnHighlight() {
        highlighted = false;
        foreach (MainMenuHighlight others in FindObjectsOfType<MainMenuHighlight>())
        {
            others.highlighted = false;
            others.OnDeselected();
        }
    }
}
