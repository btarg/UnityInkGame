using UnityEngine;
using System.Collections.Generic;

public class PopulateScrollRect : MonoBehaviour
{
    public GameObject cellPrefab;
    public List<GameObject> instantiatedCells;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject cell = Instantiate(cellPrefab, gameObject.transform);
            CellImage cellScript = cell.GetComponent<CellImage>();

            cellScript.saveSlot = i;
            cellScript.UpdateInfo();

            instantiatedCells.Add(cell);
        }
    }
}
