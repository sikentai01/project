using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    public void ShowDescription(ItemData item)
    {
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
    }
}
