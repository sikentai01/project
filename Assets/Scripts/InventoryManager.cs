using System.Collections.Generic;
using UnityEngine;

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

        // ▼ ここで DebugOwned チェック
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
        if (!items.Contains(item)) // 重複登録防止
        {
            items.Add(item);
            RefreshUI();
        }
    }

    /// <summary> アイテム削除 (ID で削除) </summary>
    public void RemoveItemByID(string itemID)
    {
        var target = items.Find(i => i.itemID == itemID); // ← 修正済み: itemID で検索
        if (target != null)
        {
            items.Remove(target);
            RefreshUI();
        }
    }

    /// <summary> アイテム使用 </summary>
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return;

        var target = items[slotIndex];
        if (target != null)
        {
            target.Use(); // ScriptableObject の Use()
        }
    }

    /// <summary> UI 更新 </summary>
    void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
                slots[i].SetItem(items[i]);
            else
                slots[i].ClearSlot();
        }
    }
}