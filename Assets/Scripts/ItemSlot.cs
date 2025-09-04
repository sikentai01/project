using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public TMP_Text slotText;   // スロット内に表示するテキスト

    private InventoryManager.InventoryItem currentItem;
    private InventoryUI inventoryUI;

    void Start()
    {
        // シーン内にある InventoryUI を探す
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        // このスロットがボタンなら、クリック時に呼ばれるように設定
        GetComponent<Button>().onClick.AddListener(OnClickSlot);
    }

    // アイテムをセットして表示
    public void SetItem(InventoryManager.InventoryItem item)
    {
        currentItem = item;
        slotText.text = item.itemName; // 名前を表示
    }

    // アイテムがない場合はクリア
    public void ClearSlot()
    {
        currentItem = null;
        slotText.text = "";
    }

    // このスロットを選んだときに説明欄を更新
    public void OnSelectSlot()
    {
        if (currentItem != null)
            inventoryUI.ShowDescription(new ItemData
            {
                itemName = currentItem.itemName,
                description = currentItem.description
            });
        else
            inventoryUI.ShowDescription(null);
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelectSlot();
    }

    // Enter（クリック）で使用
    public void OnClickSlot()
    {
        if (currentItem == null) return;

        Debug.Log(currentItem.itemName + " を使用しました！");

        // InventoryManagerに処理を投げる
        int index = transform.GetSiblingIndex();
        InventoryManager.Instance.UseItem(index);
    }
}