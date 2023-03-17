using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class InventoryItemCell : MonoBehaviour
{
    [HideInInspector] public int slotIndex = -1;
    int selected = 1;
    public Image itemIconImage;
    public TextMeshProUGUI itemAmount;
    public bool isEquipped = false;

    public Sprite equippedCellSprite;
    public Sprite hoverCellSprite;

    private void Start()
    {
        OnDeselected();
    }

    private void Awake()
    {
        selected = 1;
        OnDeselected();

    }

    private void Update()
    {
        if (isEquipped)
        {
            GetComponent<Image>().sprite = equippedCellSprite;
        }
        else
        {
            GetComponent<Image>().sprite = hoverCellSprite;
        }
    }

    public void OnSelected()
    {
        if (selected > 0)
            return;

        selected++;
        GetComponent<Image>().enabled = true;

        GetComponentInParent<InventoryUIManager>().UpdateItemLabel(slotIndex);
    }
    public void OnDeselected()
    {
        if (!isEquipped)
        {
            GetComponent<Image>().enabled = false;
            GetComponentInParent<InventoryUIManager>().UpdateItemLabel(-1);
        }

        selected--;
    }
}
