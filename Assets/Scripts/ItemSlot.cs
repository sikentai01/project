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
    // Enter（クリック）で使用
    public void OnClickSlot()
    {
        if (currentItem == null) return;

        // シーン内のすべての ItemTrigger を取得
        var triggers = Object.FindObjectsByType<ItemTrigger>(
            FindObjectsSortMode.None
        );

        foreach (var trigger in triggers)
        {
            // プレイヤーが近く、かつこのアイテムが有効なら使う
            if (trigger.IsPlayerNear && trigger.HasPendingGimmick(currentItem))
            {
                Debug.Log($"[ItemSlot] {currentItem.itemName} を {trigger.name} に使用");
                trigger.UseItemOnGimmick(currentItem);
                PauseMenu.Instance.Resume();
                return;
            }
        }

        // どのトリガーにも当てはまらなかったら通常使用
        Debug.Log($"[ItemSlot] {currentItem.itemName} を通常使用！");
        currentItem.Use();

        PauseMenu.Instance.Resume();
        Debug.Log("[ItemSlot] OnClickSlot 終了");
    }
}