using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public TMP_Text slotText;   // スロット内に表示するテキスト

    private ItemData currentItem;  // ← InventoryItem ではなく ItemData に変更
    private InventoryUI inventoryUI;

    void Start()
    {
        // シーン内にある InventoryUI を探す
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        // このスロットがボタンなら、クリック時に呼ばれるように設定
        GetComponent<Button>().onClick.AddListener(OnClickSlot);
    }

    // アイテムをセットして表示
    public void SetItem(ItemData item)
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
            inventoryUI.ShowDescription(currentItem);
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

        // 使用条件チェック
        if (CanUseItem(currentItem))
        {
            Debug.Log(currentItem.itemName + " を使用！");
            currentItem.Use();

            // 使用したらメニューを閉じる
            PauseMenu.Instance.Resume();
        }
        else
        {
            Debug.Log(currentItem.itemName + " は今使えない！");
        }
    }

    // 使用条件チェック用
    private bool CanUseItem(ItemData item)
    {
        // ここに判定ロジックを入れる（例：鍵が必要、シーン条件など）
        return true;
    }
}