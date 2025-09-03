using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    public void ShowDescription(ItemData item)
    {
        if (item == null)
        {
            itemNameText.text = "";
            itemDescriptionText.text = "";
        }
        else
        {
            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.description;
        }
    }
}