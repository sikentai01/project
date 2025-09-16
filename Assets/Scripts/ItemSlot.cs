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

        // 条件を満たす場合だけ確認パネルを出す
        if (CanUseItem(currentItem))
        {
            // 他のパネルを全部閉じる（PauseMenuに任せてもOK）
            PauseMenu.Instance.CloseAllPanels();

            // 確認パネルを開く
            ConfirmPanel.Instance.Show(currentItem);
        }
        else
        {
            Debug.Log(currentItem.itemName + " は今使えない！");
        }
    }

    private bool CanUseItem(ItemData item)
    {
        // 条件判定をここに書く
        // 例：ドアの前じゃないと鍵は使えない とか
        return true;
    }
}