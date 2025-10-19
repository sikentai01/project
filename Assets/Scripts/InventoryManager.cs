using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public List<string> ownedItemIDs = new List<string>();
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI 参照")]
    public Transform slotParent;
    private ItemSlot[] slots;

    // 所持アイテムリスト
    public List<ItemData> items = new List<ItemData>();

    // Debug用に最初から持ってるアイテム
    [Header("全アイテムリスト（デバッグ用）")]
    public List<ItemData> allItems = new List<ItemData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        slots = slotParent.GetComponentsInChildren<ItemSlot>();

        // ▼ DebugOwned チェック
        foreach (var item in allItems)
        {
            if (item != null && item.DebugOwned && !items.Contains(item))
            {
                items.Add(item);
                Debug.Log(item.itemName + " をデバッグ所持として追加");
            }
        }

        RefreshUI();
    }

    /// <summary> アイテム追加 </summary>
    public void AddItem(ItemData item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            RefreshUI();
        }
    }

    /// <summary> アイテム削除 </summary>
    public void RemoveItemByID(string itemID)
    {
        var target = items.Find(i => i.itemID == itemID);
        if (target != null)
        {
            items.Remove(target);
            RefreshUI();
        }
    }

    /// <summary> UI更新 </summary>
    void RefreshUI()
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
                slots[i].SetItem(items[i]);
            else
                slots[i].ClearSlot();
        }
    }

    public bool HasItem(string itemID)
    {
        foreach (var item in items)
        {
            if (item.itemID == itemID)
                return true;
        }
        return false;
    }

    // ===== セーブデータ作成 =====
    public InventorySaveData SaveData()
    {
        var data = new InventorySaveData();

        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.itemID))
                data.ownedItemIDs.Add(item.itemID);
        }

        Debug.Log($"[InventoryManager] {data.ownedItemIDs.Count} 件のアイテムを保存しました");
        return data;
    }

    // ===== セーブデータ読み込み =====
    public void LoadData(InventorySaveData data)
    {
        items.Clear();

        foreach (var id in data.ownedItemIDs)
        {
            var found = allItems.Find(i => i.itemID == id);
            if (found != null)
                items.Add(found);
            else
                Debug.LogWarning($"[InventoryManager] ID {id} のアイテムが見つかりません");
        }

        RefreshUI();
        Debug.Log("[InventoryManager] インベントリデータをロードしました");
    }

    // ===== 初期化 =====
    public void ClearAll()
    {
        items.Clear();
        RefreshUI();
        Debug.Log("[InventoryManager] インベントリを初期化しました");
    }

}
