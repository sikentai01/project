using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;  // ISelectHandler を使うために必要

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public TMP_Text slotText;   // スロット内に表示するテキスト

    private ItemData currentItem;
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
        slotText.text = item.itemName;
    }

    // アイテムがない場合はクリア
    public void ClearSlot()
    {
        currentItem = null;
        slotText.text = "";
    }

    // カーソルが合ったときに情報を表示
    public void OnSelectSlot()
    {
        if (currentItem != null)
            inventoryUI.ShowDescription(currentItem);
        else
            inventoryUI.ShowDescription(null);
    }

    // ISelectHandler 実装 → カーソルが乗った瞬間に OnSelectSlot() を呼ぶ
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("カーソルがスロットに乗った: " + (currentItem != null ? currentItem.itemName : "空"));
        OnSelectSlot();
    }

    // Enter（クリック）で使用
    public void OnClickSlot()
    {
        if (currentItem == null) return; // アイテムがなければ何もしない

        Debug.Log(currentItem.itemName + " を使用しました！");

        // アイテムの効果発動
        currentItem.UseEffect();

        // 消耗品なら削除
        if (currentItem.isConsumable)
        {
            InventoryManager.Instance.RemoveItem(currentItem);
        }

        // メニューを閉じる
        FindFirstObjectByType<PauseMenu>().Resume();
    }
}