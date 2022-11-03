using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PopulateMainMenu : MonoBehaviour
{
    public GameObject cellPrefab;
    public List<GameObject> instantiatedCells;

    public int numSaveSlots = 3;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numSaveSlots; i++)
        {
            GameObject cell = Instantiate(cellPrefab, gameObject.transform);
            MainMenuCell cellScript = cell.GetComponent<MainMenuCell>();

            // Select the first cell
            if (i == 0 && !EventSystem.current.alreadySelecting) {
                EventSystem.current.SetSelectedGameObject(cell);
            }

            cellScript.saveSlot = i;
            cellScript.UpdateInfo();

            instantiatedCells.Add(cell);
        }
    }
}
