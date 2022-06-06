using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellImage : MonoBehaviour
{
    public int saveSlot = 0;
    public Button loadButton;
    [SerializeField] RawImage thumbnail;
    [SerializeField] TextMeshProUGUI nameLabel;
    [SerializeField] TextMeshProUGUI levelLabel;
    [SerializeField] TextMeshProUGUI timestampLabel;

    public void UpdateInfo()
    {

        Texture2D tex = SaveHelper.getSaveScreenshot(saveSlot);

        if (tex != null)
        {
            thumbnail.texture = tex;
            thumbnail.enabled = true;
        }
        else
        {
            thumbnail.enabled = false;
        }

        SaveObject currentSave = SaveManager.Load(saveSlot);

        if (currentSave != null)
        {
            if (currentSave.name == "") {
                nameLabel.text = "[UNNAMED]";
            } else {
                nameLabel.text = currentSave.name;
            }
            
            levelLabel.text = currentSave.currentScene;
            timestampLabel.text = DateTime.FromFileTime(currentSave.timestamp).ToString();
        }
        else
        {
            nameLabel.text = "[EMPTY FILE #" + (saveSlot + 1 ) + "]";
            levelLabel.text = "---";
            timestampLabel.text = "---";
        }

        // Load save when button is clicked
        loadButton.onClick.AddListener(() => SelectThisSlot());

    }

    void SelectThisSlot()
    {
        gameObject.GetComponent<MainMenuHighlight>().OnHighlight();
        MainMenu menu = GetComponentInParent<MainMenu>();
        menu.SelectCell(this);

    }
}
