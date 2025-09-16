using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI 参照")]
    public Transform slotParent;
    private ItemSlot[] slots;

    // 所持アイテムリスト → ScriptableObject 参照に切り替え
    public List<ItemData> items = new List<ItemData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        slots = slotParent.GetComponentsInChildren<ItemSlot>();
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
        var target = items.Find(i => i.name == itemID); // name を ID 代わりに使ってる
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
            target.Use(); // ← ScriptableObject の Use() が呼ばれる
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