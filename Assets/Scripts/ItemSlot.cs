using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public TMP_Text slotText;   // スロットに表示する名前
    private ItemData currentItem;
    private InventoryUI inventoryUI;

    void Start()
    {
        // シーン内にある InventoryUI を探す
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        // ボタンのクリックイベントに登録
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

    // Enter押したときに使用
    public void OnClickSlot()
    {
        if (currentItem == null) return;

        Debug.Log(currentItem.itemName + " を使用しました！");

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