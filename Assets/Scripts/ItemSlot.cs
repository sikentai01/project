using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public TMP_Text slotText;   // スロット内に表示するテキスト
    public Image slotIcon;      // アイコン表示（不要ならInspectorで外す）

    private ItemData currentItem;
    private InventoryUI inventoryUI;

    void Start()
    {
        // シーン内にある InventoryUI を探す
        inventoryUI = inventoryUI = FindFirstObjectByType<InventoryUI>();

        // このスロットがボタンなら、クリック時に呼ばれるように設定
        GetComponent<Button>().onClick.AddListener(OnClickSlot);
    }

    // アイテムをセットして表示
    public void SetItem(ItemData item)
    {
        currentItem = item;
        slotText.text = item.itemName;

        if (slotIcon) slotIcon.sprite = item.icon;
    }

    // アイテムがない場合はクリア
    public void ClearSlot()
    {
        currentItem = null;
        slotText.text = "";
        if (slotIcon) slotIcon.sprite = null;
    }

    // このスロットを選んだときに説明欄を更新
    void OnClickSlot()
    {
        if (currentItem != null)
        {
            inventoryUI.ShowDescription(currentItem);
        }
    }
}
